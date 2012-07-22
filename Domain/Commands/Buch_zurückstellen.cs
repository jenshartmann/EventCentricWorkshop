using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework.Domain;

namespace Domain.Commands
{
    [Serializable]
    public class Buch_zurückstellen : Command
    {
        public Guid BuchID { get; set; }

    }
}
