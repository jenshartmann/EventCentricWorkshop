using System;

namespace Framework.Domain
{
    [Serializable]
    public abstract class Command
    {
        public override string ToString()
        {
            return string.Format("{0}", GetType().Name.Replace("_", " "));
        }
    }
}