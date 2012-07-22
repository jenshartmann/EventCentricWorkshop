using System;
using System.Collections.Generic;
using System.Linq;

namespace Framework.Domain
{
    [Serializable]
    public abstract class Event
    {
        public abstract Guid AggregateId { get; }

        public bool IsPartOf(IList<Event> EventList)
        {
            return EventList.Any(e => DoAllPropertiesMatch(e));
        }

        private bool DoAllPropertiesMatch(Event e)
        {
            if (this.GetType() != e.GetType())
                return false;

            var myProperties = this.GetType().GetProperties().Where(_ => _.Name != "Id");
            var otherProperties = e.GetType().GetProperties().Where(_ => _.Name != "Id");

            if (myProperties.Count() != otherProperties.Count())
                return false;

            foreach (var myProperty in myProperties)
            {
                var otherProperty = otherProperties.SingleOrDefault(_ => _.Name == myProperty.Name);

                if (otherProperty == null)
                    return false;

                if (!myProperty.GetValue(this, null).Equals(otherProperty.GetValue(e, null)))
                    return false;
            }

            return true;
        }

        public override string ToString()
        {
            return string.Format("{0}", GetType().Name.Replace("_", " "));
        }
    }
}