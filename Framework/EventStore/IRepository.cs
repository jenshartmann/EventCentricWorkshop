using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework.Domain;

namespace Framework.EventStore
{
    public interface IRepository
    {
        long GlobalEventRevision();

        IEnumerable<Event> LoadEvents(Guid aggregateIdentifier);
        IEnumerable<Event> LoadEvents(Guid aggregateIdentifier, long skipEvents, int count);
        IEnumerable<Event> LoadEvents(long skipEvents, int takeEvents);
        void SaveEvents(IEnumerable<Event> newEvents);

        T Load<T>(Guid aggregateID) where T : AggregateRoot, new();
        void Save<T>(T aggregate) where T : AggregateRoot;
    }
}
