using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using Framework.Domain;
using ZMQ;

namespace Framework.EventStore
{
    public class Repository : IRepository
    {
        public long GlobalEventRevision()
        {
            return _allEvents.Length;
        }

        public IEnumerable<Event> LoadEvents(Guid aggregateIdentifier)
        {
            return LoadEvents(aggregateIdentifier, 0, int.MaxValue);
        }

        public IEnumerable<Event> LoadEvents(Guid aggregateIdentifier, long skipEvents, int count)
        {
            Event[] list;
            return _aggregateSpecificEvents.TryGetValue(aggregateIdentifier, out list) ? list : Enumerable.Empty<Event>();
        }

        public IEnumerable<Event> LoadEvents(long skipEvents, int count)
        {
            int i = 0;
            foreach (var Event in _allEvents)
            {
                i++;
                if ((i > skipEvents) && (i <= skipEvents + count))
                    yield return Event;
            }
        }

        public void SaveEvents(IEnumerable<Event> newEvents)
        {
            var records = new List<Record>();
            foreach (var newEvent in newEvents)
            {
                Event[] list;
                var eventVersion = (_aggregateSpecificEvents.TryGetValue(newEvent.AggregateId, out list) ? list : Enumerable.Empty<Event>()).Count() + 1;
                var globalVersion = _allEvents.Count() + 1;
                records.Add(new Record(newEvent.AggregateId, eventVersion, globalVersion, SerializeEvent(newEvent)));
                AddToCaches(newEvent);
                PublishEvent(newEvent);
            }
            _eventStore.Append(records);
        }

        private void PublishEvent(Event newEvent)
        {
            /*
            const string serverAdress = "tcp://127.0.0.1:25559";
            var context = new Context();
            using (Socket socket = context.Socket(SocketType.PUSH))
            {
                socket.Connect(serverAdress);
                socket.Send(SerializeEvent(newEvent));
            }
            */
        }

        public T Load<T>(Guid aggregateID) where T : AggregateRoot, new()
        {
            T aggregate = new T();
            var history = LoadEvents(aggregateID);
            if (history == null)
                throw new ArgumentNullException(string.Format("Keine Ereignisse für das Aggregat {0} vorhanden.", aggregateID));
            aggregate.RestoreFrom(history);

            return aggregate;
        }

        public void Save<T>(T aggregate) where T : AggregateRoot
        {
            SaveEvents(aggregate.UncommitedEvents);
            aggregate.Commit();
        }

        private readonly IEventStore _eventStore;

        public Repository(IEventStore eventStore)
        {
            _eventStore = eventStore;
            LoadCaches();
        }

        void AddToCaches(Event Event)
        {
            _allEvents = ImmutableAdd(_allEvents, Event);
            _aggregateSpecificEvents.AddOrUpdate(Event.AggregateId, s => new[] { Event }, (s, Events) => ImmutableAdd(Events, Event));
        }

        static T[] ImmutableAdd<T>(T[] source, T item)
        {
            var copy = new T[source.Length + 1];
            Array.Copy(source, copy, source.Length);
            copy[source.Length] = item;
            return copy;
        }

        readonly BinaryFormatter _formatter = new BinaryFormatter();

        byte[] SerializeEvent(Event e)
        {
            using (var mem = new MemoryStream())
            {
                _formatter.Serialize(mem, e);
                return mem.ToArray();
            }
        }

        Event DeserializeEvent(byte[] data)
        {
            using (var mem = new MemoryStream(data))
            {
                return (Event)_formatter.Deserialize(mem);
            }
        }

        readonly ConcurrentDictionary<Guid, Event[]> _aggregateSpecificEvents = new ConcurrentDictionary<Guid, Event[]>();
        Event[] _allEvents = new Event[0];

        readonly ReaderWriterLockSlim _thread = new ReaderWriterLockSlim();

        public void LoadCaches()
        {
            try
            {
                _thread.EnterWriteLock();
                foreach (var record in _eventStore.EnumerateHistory())
                {
                    AddToCaches(DeserializeEvent(record.Data));
                }
            }
            finally
            {
                _thread.ExitWriteLock();
            }
        }

    }
}