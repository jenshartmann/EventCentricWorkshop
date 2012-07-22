using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Domain.Events;
using Framework.Domain;
using ZMQ;

namespace TheBookThingReadModels
{
    class BookReadModel
    {


        private static readonly BinaryFormatter Formatter = new BinaryFormatter();

        private static Event DeserializeCommand(byte[] data)
        {
            using (var mem = new MemoryStream(data))
            {
                return (Event)Formatter.Deserialize(mem);
            }
        }

        static void Main(string[] args)
        {
            const string serverAdress = "tcp://*:25559";
                        var context = new Context();

            using (Socket socket = context.Socket(SocketType.PULL))
            {
                socket.Bind(serverAdress);
                while (true)
                {
                    byte[] message = socket.Recv();
                    if (message != null)
                    {
                        Event Event = DeserializeCommand(message);
                        ProjeziereEreignis((dynamic)Event);
                    }
                    else
                    {
                        Console.WriteLine(".");
                    }
                }
            }
        }

        private static void ProjeziereEreignis(Event Event)
        {
        }
        const string allBooks = "AlleEingepflegtenBuecher.txt";
        const string availableBooks = "AllevorliegendenBuecher.txt";
        private long LastRevision = 0;
        private static void ProjeziereEreignis(Buch_wurde_eingepflegt Event)
        {
            using (var r = new StreamWriter(allBooks, true))
            {
                r.WriteLine(Event.AggregateId + ";" + Event.Titel + " von " + Event.Author);
            }

            using (var r = new StreamWriter(availableBooks,true))
            {
                r.WriteLine(Event.AggregateId + ";" + Event.Titel + " von " + Event.Author);
            }
        }
        private static void ProjeziereEreignis(Buch_wurde_ausgeliehen Event)
        {
            //Entferne das Buch aus der Textdatei der vorliegenden Bücher

        }
        private static void ProjeziereEreignis(Buch_wurde_zurückgestellt Event)
        {
            //Füge das Buch in die Textdatei der vorliegenden Bücher

        }

    }
}
