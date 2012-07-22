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
    public class Buch_einpflegen_Handler : IHandle<Buch_einpflegen>
    {
        public void Handle(Buch_einpflegen command)
        {
            var buch = DomainLayer.Repository.Load<Buch>(command.BuchID);
            buch.Einpflegen(command.BuchID,
                                command.ISBN,
                                command.Titel,
                                command.Author);
            DomainLayer.Repository.Save<Buch>(buch);
        }
    }
}
