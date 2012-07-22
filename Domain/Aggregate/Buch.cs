using System;
using Domain.Events;
using Framework.Domain;

namespace Domain.Aggregate
{
    public class Buch : AggregateRoot
    {
        private bool _ausgeliehen;

        public Guid BuchID { get; set; }

        #region Apply

        public void Apply(Buch_wurde_eingepflegt e)
        {
            BuchID = e.BuchID;
        }

        public void Apply(Buch_wurde_ausgeliehen e)
        {
            _ausgeliehen = true;
        }

        public void Apply(Buch_wurde_zurückgestellt e)
        {
            _ausgeliehen = false;
        }

        #endregion

        #region Methods

        public void Einpflegen(Guid buchID, string isbn, string titel, string author)
        {
            if ((buchID != BuchID) && (buchID != Guid.Empty))
            {
                Dispatch(new Buch_wurde_eingepflegt
                {
                    BuchID = buchID,
                    ISBN = isbn,
                    Titel = titel,
                    Author = author
                });
            }
            else
            {
                Dispatch(new Buch_einpflegen_wurde_abgelehnt
                    {
                        BuchID = buchID,
                    });
            }
        }

        public void Ausleihen(Guid buchID)
        {
            if (_ausgeliehen)
                Dispatch(new Buch_ausleihen_wurde_abgelehnt { BuchID = buchID });
            else
                Dispatch(new Buch_wurde_ausgeliehen { BuchID = buchID });
        }

        public void Zurückstellen(Guid buchID)
        {
            if (_ausgeliehen)
                Dispatch(new Buch_wurde_zurückgestellt{BuchID = buchID});
        }

    #endregion

    }
}