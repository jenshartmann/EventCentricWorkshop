using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using Domain.Commands;
using Domain.Events;
using Framework.Domain;
using ZMQ;

namespace TheBookThingConsoleClient
{
    internal class BookClient
    {
        private static readonly BinaryFormatter Formatter = new BinaryFormatter();

        private static byte[] SerializeCommand(Command e)
        {
            using (var mem = new MemoryStream())
            {
                Formatter.Serialize(mem, e);
                return mem.ToArray();
            }
        }

        private static string _screen;

        private static Command command;

        private static string serverAdress = "tcp://127.0.0.1:25558";

        private static void Main(string[] args)
        {
            #region ReadModelinitialisieren
            /*
            ParameterizedThreadStart pts = new ParameterizedThreadStart(ProjectReadModels);
            Thread thread = new Thread(pts);
            thread.Start();
            */
            #endregion

            var context = new Context();

            SetScreen("welcome");
            using (Socket socket = context.Socket(SocketType.PUSH))
            {
                socket.Connect(serverAdress);
                bool running = true;
                while (running)
                {
                    var key = Console.ReadLine();

                    switch (key.ToLower())
                    {
                        case "q":
                            _screen = "goodbye";
                            running = false;
                            break;
                        case "e":
                            _screen = "einpflegen";
                            break;
                        case "a":
                            _screen = "ausleihen";
                            break;
                        case "z":
                            _screen = "zurückstellen";
                            break;
                    }

                    SetScreen(_screen);

                    if (command != null)
                    {
                        socket.Send(SerializeCommand(command));
                        command = null;
                    }
                }
            }
        }

        private static void SetScreen(string screenAuswahl)
        {
            Console.Clear();
            int buchNr;
            switch (screenAuswahl)
            {
                case "welcome":
                    Console.WriteLine("Das Bücherregal");
                    Console.WriteLine("");
                    Console.WriteLine("Sie haben folgende optionen:");
                    Console.WriteLine("'e'inpflegen / 'a'usleihen / 'z'urückstellen / 'q'uit");
                    break;

                case "einpflegen":
                    string Titel;
                    string Author;
                    string ISBN;

                    Console.Clear();
                    Console.WriteLine("Buch einpflegen");
                    Console.WriteLine("#######################");
                    Console.Write("# Titel: ");
                    Titel = Console.ReadLine();
                    Console.Write("# Author: ");
                    Author = Console.ReadLine();
                    Console.Write("# ISBN: ");
                    ISBN = Console.ReadLine();
                    Console.WriteLine("#######################");

                    command = new Buch_einpflegen
                    {
                        Author = Author,
                        BuchID = Guid.NewGuid(),
                        ISBN = ISBN,
                        Titel = Titel
                    };

                    SetScreen("welcome");
                    break;

                case "ausleihen":
                    /*foreach (var eintrag in _buecherImSystem)
                    {
                        Console.WriteLine(eintrag.Key.ToString() + ": " + eintrag.Value.ToString());
                    }
                     */
                    Console.Write("Das wievielte Buch ausleihen? ");
                    buchNr = int.Parse(Console.ReadLine());

                    //Dictionary<Guid, string>.Enumerator enumerator = _buecherImSystem[buchNr].GetEnumerator();

                    //command = new Buch_ausleihen()
                    //{
                    //    BuchID = enumerator.Current.Key
                    //};
                    SetScreen("welcome");
                    break;
                case "zurückstellen":
                    //Liste der ausgeliehenen Bücher anzeigen
                    Console.Write("Welches Buch zurückstellen? BuchNr.: ");
                    buchNr = int.Parse(Console.ReadLine());

                    command = new Buch_ausleihen()
                    {
                        BuchID = Guid.NewGuid()
                    };

                    SetScreen("welcome");
                    break;
                case "goodbye":
                    break;
            }

        }

        #region ReadModel
        /*
        private static Dictionary<int, Dictionary<Guid, string>> _buecherImSystem =
            new Dictionary<int, Dictionary<Guid, string>>();

        private static Dictionary<int, Dictionary<Guid, string>> _buecherAusgeliehen =
            new Dictionary<int, Dictionary<Guid, string>>();

        private static Dictionary<int, Dictionary<Guid, string>> _buecherImRegal =
            new Dictionary<int, Dictionary<Guid, string>>();

        private static Event DeserializeEvent(byte[] data)
        {
            using (var mem = new MemoryStream(data))
            {
                return (Event)Formatter.Deserialize(mem);
            }
        }

        private static void ProjectReadModels(object o)
        {
            const string ReadModelAdress = "tcp://*:25559";

            var context = new Context();

            using (Socket socket = context.Socket(SocketType.PULL))
            {
                socket.Bind(ReadModelAdress);
                while (true)
                {
                    byte[] message = socket.Recv();
                    if (message != null)
                    {
                        var Event = DeserializeEvent(message);
                        Console.WriteLine("");
                        Console.Write("Projesziere " + Event + " -> ");
                        Projesziere((dynamic)Event);
                        Console.WriteLine("done");
                    }
                }
            }
        }

        static void Projesziere(Buch_wurde_ausgeliehen Event)
        {
            Dictionary<int, Dictionary<Guid, string>>.Enumerator enumerator = _buecherImSystem.GetEnumerator();
            for (int i = 0; i < _buecherImSystem.Count; i++)
            {
                Dictionary<Guid, string>.Enumerator innerEnumerator = enumerator.Current.Value.GetEnumerator();

                if (innerEnumerator.Current.Key == Event.AggregateId)
                {
                    Dictionary<Guid, string> buch = new Dictionary<Guid, string>();
                    buch.Add(innerEnumerator.Current.Key, innerEnumerator.Current.Value);

                    _buecherAusgeliehen.Add(_buecherImRegal.Count + 1, buch);
                }
            }
        }

        static void Projesziere(Buch_wurde_eingepflegt Event)
        {
            Dictionary<Guid, string> Buch = new Dictionary<Guid, string>();
            Buch.Add(Event.AggregateId, string.Format("{0} von {1}", Event.Titel, Event.Author));
            _buecherImSystem.Add(_buecherImSystem.Count + 1, Buch);
            _buecherImRegal.Add(_buecherImSystem.Count + 1, Buch);
        }

        static void Projesziere(Buch_wurde_zurückgestellt Event)
        {
            Dictionary<int, Dictionary<Guid, string>>.Enumerator enumerator = _buecherImSystem.GetEnumerator();
            for (int i = 0; i < _buecherImSystem.Count; i++)
            {
                Dictionary<Guid, string>.Enumerator innerEnumerator = enumerator.Current.Value.GetEnumerator();

                if (innerEnumerator.Current.Key == Event.AggregateId)
                {
                    Dictionary<Guid, string> buch = new Dictionary<Guid, string>();
                    buch.Add(innerEnumerator.Current.Key, innerEnumerator.Current.Value);

                    _buecherImRegal.Add(_buecherImRegal.Count + 1, buch);
                }
            }
        }

        static void Projesziere(Event Event)
        {
        }
        */
        #endregion
    }
}
