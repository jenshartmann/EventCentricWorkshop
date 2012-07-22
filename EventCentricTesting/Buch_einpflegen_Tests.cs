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
    public class Buch_einpflegen_Tests : TestBaseClass
    {
        [Test]
        public void Ein_Buch_erfolgreich_einpflegen()
        {
            var neueBuchID = Guid.NewGuid();

            When(new Buch_einpflegen
                {
                    BuchID = neueBuchID,
                    ISBN = "12345",
                    Titel = "CQRS Handbuch",
                    Author = "Greg Young"
                });

            Then_Expect(new Buch_wurde_eingepflegt
                {
                    BuchID = neueBuchID,
                    ISBN = "12345",
                    Titel = "CQRS Handbuch",
                    Author = "Greg Young"
                });

        }

        [Test]
        public void Ein_eingepflegtes_Buch_kann_nicht_eingepflegt_werden()
        {
            var neueBuchID = Guid.NewGuid();

            Given(new Buch_wurde_eingepflegt
                {
                    BuchID = neueBuchID,
                    ISBN = "12345",
                    Titel = "CQRS Handbuch",
                    Author = "Greg Young"
                });
            When(new Buch_einpflegen
                {
                    BuchID = neueBuchID,
                    ISBN = "12345",
                    Titel = "CQRS Handbuch",
                    Author = "Greg Young"
                });

            Then_Expect(new Buch_einpflegen_wurde_abgelehnt {BuchID = neueBuchID});
        }

        [Test]
        public void Ein_gleiches_Buch_kann_eingepflegt_werden()
        {
            var alteBuchID = Guid.NewGuid();
            var neueBuchID = Guid.NewGuid();

            Given(new Buch_wurde_eingepflegt
                {
                    BuchID = alteBuchID,
                    ISBN = "12345",
                    Titel = "CQRS Handbuch",
                    Author = "Greg Young"
                });
            When(new Buch_einpflegen
                {
                    BuchID = neueBuchID,
                    ISBN = "12345",
                    Titel = "CQRS Handbuch",
                    Author = "Greg Young"
                });

            Then_Expect(new Buch_wurde_eingepflegt
            {
                BuchID = neueBuchID,
                ISBN = "12345",
                Titel = "CQRS Handbuch",
                Author = "Greg Young"
            });
        }


    }
}
