using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Framework.Domain;

namespace Framework.Messaging
{
    public class CommandRouter : ICommandRouter
    {
        private List<dynamic> handlers;

        public CommandRouter(Assembly handlerAssembly)
        {
            var handlerTypes =
                handlerAssembly.GetTypes().Where(_ => _.GetInterface(typeof (IHandle<>).Name) != null).ToList();
            handlers = handlerTypes.Select(_ => Activator.CreateInstance(_)).ToList();
        }

        public void Route(Command c)
        {
            var commandType = c.GetType();

            dynamic h =
                handlers.Where(_ => _.GetType().ToString().EndsWith(commandType.Name + "_Handler")).SingleOrDefault();

            if (h != null)
                h.Handle((dynamic) c);
        }
    }
}