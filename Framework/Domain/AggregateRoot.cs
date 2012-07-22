using System;
using System.Collections.Generic;
using System.Linq;

namespace Framework.Domain
{
    public class AggregateRoot
    {
        private readonly Queue<Event> _uncommitedEvents = new Queue<Event>();

        public Guid Id { get; protected set; }

        public IEnumerable<Event> UncommitedEvents
        {
            get { return _uncommitedEvents.ToList(); }
        }

        public void RestoreFrom(IEnumerable<Event> events)
        {
            foreach (var e in events)
            {
                ((dynamic)this).Apply((dynamic)e);
            }
        }
        
        public void Apply(Event e)
        {
        }
      
        public void Commit()
        {
            _uncommitedEvents.Clear();
        }

        protected void Dispatch(Event Event)
        {
            _uncommitedEvents.Enqueue(Event);
            ((dynamic)this).Apply((dynamic)Event);
        }
    }
}