using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Commands;
using Domain.Events;
using EventCentricTesting.TestFrameWork;
using Framework.Domain;
using NUnit.Framework;

namespace EventCentricTesting
{
    [TestFixture]
    public class Buch_zurückstellen_Tests : TestBaseClass
    {
        [Test]
        public void Ein_ausgeliehenes_Buch_erfolgreich_zurückstellen()
        {
            var neueBuchID = Guid.NewGuid();
            Given(new Event[]{
                                new Buch_wurde_eingepflegt
                                    {
                                        BuchID = neueBuchID, ISBN = "12345", Titel = "CQRS Handbuch", Author = "Greg Young"
                                    },
                                    new Buch_wurde_ausgeliehen
                                    {
                                        BuchID = neueBuchID
                                    }
                             });

            When(new Buch_zurückstellen { BuchID = neueBuchID });

            Then_Expect(new Event[]
                            {
                                new Buch_wurde_zurückgestellt{BuchID = neueBuchID}
                            });
        }

        [Test]
        public void Buch_zurückstellen_hebt_das_auslehein_auf()
        {
            var neueBuchID = Guid.NewGuid();
            Given(new Event[]{
                                new Buch_wurde_eingepflegt
                                    {
                                        BuchID = neueBuchID, ISBN = "12345", Titel = "CQRS Handbuch", Author = "Greg Young"
                                    },
                                    new Buch_wurde_ausgeliehen
                                    {
                                        BuchID = neueBuchID
                                    }
                             });

            When(new Buch_zurückstellen { BuchID = neueBuchID });

            Then_Expect(new Event[]
                            {
                                new Buch_wurde_zurückgestellt{BuchID = neueBuchID}
                            });
        }
    }
}
