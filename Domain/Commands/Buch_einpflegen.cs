using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework.Domain;

namespace Domain.Commands
{
    [Serializable]
    public class Buch_einpflegen : Command
    {
        public Guid BuchID { get; set; }
        public string ISBN { get; set; }
        public string Titel { get; set; }
        public string Author { get; set; }

        public override string ToString()
        {
            return string.Format("{0}: {1}", GetType().Name.Replace("_", " "), Titel);
        }
    }
}
