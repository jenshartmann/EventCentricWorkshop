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
    public class Buch_ausleihen_Tests : TestBaseClass
    {
        [Test]
        public void Ein_eingepflegtes_Buch_kann_ausgeliehen_werden()
        {
            var NeueBuchID = Guid.NewGuid();
            Given(new Event[]{
                                new Buch_wurde_eingepflegt
                                    {
                                        BuchID = NeueBuchID, ISBN = "12345", Titel = "CQRS Handbuch", Author = "Greg Young"
                                    }
                             });

            When(new Buch_ausleihen { BuchID = NeueBuchID });

            Then_Expect(new Event[]
                            {
                                new Buch_wurde_ausgeliehen{BuchID = NeueBuchID}
                            });
        }

        [Test]
        public void Ein_ausgeliehenes_Buch_kann_nicht_ausgeliehen_werden()
        {
            var NeueBuchID = Guid.NewGuid();
            Given(new Event[]{
                                new Buch_wurde_eingepflegt
                                    {
                                        BuchID = NeueBuchID, ISBN = "12345", Titel = "CQRS Handbuch", Author = "Greg Young"
                                    },
                                    new Buch_wurde_ausgeliehen
                                    {
                                        BuchID = NeueBuchID
                                    }
                             });

            When(new Buch_ausleihen { BuchID = NeueBuchID });

            Then_Expect(new Event[]
                            {
                                new Buch_ausleihen_wurde_abgelehnt{BuchID = NeueBuchID}
                            });
        }

        [Test]
        public void Ein_zurückgestelltes_Buch_kann_ausgeliehen_werden()
        {
            var NeueBuchID = Guid.NewGuid();
            Given(new Event[]{
                                new Buch_wurde_eingepflegt
                                    {
                                        BuchID = NeueBuchID, ISBN = "12345", Titel = "CQRS Handbuch", Author = "Greg Young"
                                    },
                                    new Buch_wurde_ausgeliehen
                                    {
                                        BuchID = NeueBuchID
                                    },
                                    new Buch_wurde_zurückgestellt
                                    {
                                        BuchID = NeueBuchID
                                    }
                             });

            When(new Buch_ausleihen { BuchID = NeueBuchID });

            Then_Expect(new Event[]
                            {
                                new Buch_wurde_ausgeliehen{BuchID = NeueBuchID}
                            });
        }
    }

}
