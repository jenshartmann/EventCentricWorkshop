using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Domain;
using Framework.Domain;
using Framework.EventStore;
using Framework.EventStore.Implementations.Files;
using Framework.EventStore.Implementations.InMemory;
using ZMQ;

namespace TheBookThingAppserver
{
    internal class Bookserver
    {
        private static IEventStore _eventStore;
        public static IRepository Repository;

        private static readonly BinaryFormatter Formatter = new BinaryFormatter();

        private static Command DeserializeCommand(byte[] data)
        {
            using (var mem = new MemoryStream(data))
            {
                return (Command) Formatter.Deserialize(mem);
            }
        }

        private static void Main(string[] args)
        {
            const string serverAdress = "tcp://*:25558";

            Console.WriteLine("The Book Thing Server @" + serverAdress);

            _eventStore = new FileEventStore(Path.Combine(Directory.GetCurrentDirectory(), "store"));
            //_eventStore = new MsSqlEventStore("Data Source=sqlServer;Initial Catalog=;User Id=;Password=;");
            //_eventStore = new InMemoryEventStore();

            Repository = new Repository(_eventStore);
            DomainLayer.Initialize(Repository);

            var context = new Context();

            using (Socket socket = context.Socket(SocketType.PULL))
            {
                socket.Bind(serverAdress);
                while (true)
                {
                    byte[] message = socket.Recv();
                    if (message != null)
                    {
                        Command command = DeserializeCommand(message);
                        Console.WriteLine("");
                        Console.Write("Working on " + command + " -> ");
                        DomainLayer.CommandRouter.Route(command);
                        Console.WriteLine("done");
                    }
                    else
                    {
                        Console.WriteLine(".");
                    }
                }
            }
        }

    }
}
