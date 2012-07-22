using System;

namespace Framework.EventStore
{
    public sealed class Record
    {
        public readonly Guid AggregateIdentifier;
        public readonly long GlobalRevision;
        public readonly long Revision;
        public readonly byte[] Data;

        public Record(Guid aggregateIdentifier, long revision, long globalRevision, byte[] data)
        {
            AggregateIdentifier = aggregateIdentifier;
            Revision = revision;
            GlobalRevision = globalRevision;
            Data = data;
        }
    }
}