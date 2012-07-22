using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework.EventStore;
using Framework.Messaging;

namespace Domain
{
    public class DomainLayer
    {
        public static IRepository Repository { get; private set; }
        public static ICommandRouter CommandRouter { get; private set; }

        public static void Initialize(IRepository repository)
        {
            Repository = repository;
            CommandRouter = new CommandRouter(typeof(DomainLayer).Assembly);
        }
    }
}
