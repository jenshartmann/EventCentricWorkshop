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
    public class Buch_zurückstellen_Handler : IHandle<Buch_zurückstellen>
    {
        public void Handle(Buch_zurückstellen command)
        {
            var buch = DomainLayer.Repository.Load<Buch>(command.BuchID);
            buch.Zurückstellen(command.BuchID);
            DomainLayer.Repository.Save<Buch>(buch);
        }
    }
}
