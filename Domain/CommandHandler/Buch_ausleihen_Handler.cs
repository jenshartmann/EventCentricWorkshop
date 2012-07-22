using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Aggregate;
using Domain.Commands;
using Framework.Messaging;

namespace Domain.CommandHandler
{
    public class Buch_ausleihen_Handler : IHandle<Buch_ausleihen>
    {
        public void Handle(Buch_ausleihen command)
        {
            var buch = DomainLayer.Repository.Load<Buch>(command.BuchID);
            buch.Ausleihen(command.BuchID);
            DomainLayer.Repository.Save<Buch>(buch);
        }
    }
}
