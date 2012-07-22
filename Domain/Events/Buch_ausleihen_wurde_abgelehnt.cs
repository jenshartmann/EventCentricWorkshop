using System;
using Framework;
using Framework.Domain;

namespace Domain.Events
{
    [Serializable]
    public class Buch_ausleihen_wurde_abgelehnt : Event
    {
        public Guid BuchID { get; set; }

        public override Guid AggregateId
        {
            get { return BuchID; }
        }
    }
}
