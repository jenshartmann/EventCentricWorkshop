using System.Collections.Generic;

namespace Framework.EventStore
{
    public interface IEventStore
    {
        IEnumerable<Record> EnumerateHistory();
        void Append(IEnumerable<Record> newRecords);
    }
}