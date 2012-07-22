using System;
using Framework;
using Framework.Domain;

namespace Domain.Events
{
    [Serializable]
    public class Buch_wurde_eingepflegt : Event
    {
        public Guid BuchID { get; set; }
        public string Titel { get; set; }
        public string ISBN { get; set; }
        public string Author { get; set; }

        public override string ToString()
        {
            return string.Format("{0}: {1}", GetType().Name.Replace("_", " "), Titel);
        }

        public override Guid AggregateId
        {
            get { return BuchID; }
        }
    }
}
