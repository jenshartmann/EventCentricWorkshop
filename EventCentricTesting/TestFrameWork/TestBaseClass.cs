using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;
using Framework.Domain;
using Framework.EventStore;
using Framework.EventStore.Implementations.InMemory;
using NUnit.Framework;

namespace EventCentricTesting.TestFrameWork
{
    public class TestBaseClass
    {
        public IRepository Repository;
        private InMemoryEventStore _inMemoryEventStore;
        private long historicalEventCount = 0;


        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            _inMemoryEventStore = new InMemoryEventStore();
            Repository = new Repository(_inMemoryEventStore);
            DomainLayer.Initialize(Repository);
        }

        protected void Given(params Event[] givenEvents)
        {
            Repository.SaveEvents(givenEvents);
            historicalEventCount = Repository.GlobalEventRevision();
        }

        public void Then_Expect(params Event[] expectedEvents)
        {
            var generatedEvents = Repository.LoadEvents(historicalEventCount, int.MaxValue).ToList();

            Assert.AreEqual(expectedEvents.Length, generatedEvents.Count,
                            "Die Anzahl der aufgetretenen Ereignisse stimmt nicht mit der Anzahl der erwarteten überein.");

            foreach (var e in expectedEvents)
            {
                Assert.IsTrue(e.IsPartOf(generatedEvents), "Erwartet: \n" + expectedEvents.Aggregate<Event, string>(null, (current, expectedEvent) => current + expectedEvent.ToString())
                              + "\n\nPassiert:\n"
                              + generatedEvents.Aggregate<Event, string>(null, (current, generatedEvent) => current + generatedEvent.ToString())
                              + "\n\n");

            }
        }

        public void When(Command command)
        {
            DomainLayer.CommandRouter.Route(command);
        }
    }
}
