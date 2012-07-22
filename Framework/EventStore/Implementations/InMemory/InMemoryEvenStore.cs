using System;
using System.Collections.Generic;

namespace Framework.EventStore.Implementations.InMemory
{
    public class InMemoryEventStore : IEventStore
    {
        public IEnumerable<Record> EnumerateHistory()
        {
            return _allRecords;
        }

        public void Append(IEnumerable<Record> newRecords)
        {
            if (newRecords != null)
            {
                foreach (var record in newRecords)
                {
                    _allRecords = ImmutableAdd(_allRecords, record);
                }
            }
        }

        static T[] ImmutableAdd<T>(T[] source, T item)
        {
            var copy = new T[source.Length + 1];
            Array.Copy(source, copy, source.Length);
            copy[source.Length] = item;
            return copy;
        }

        Record[] _allRecords = new Record[0];
    }
}