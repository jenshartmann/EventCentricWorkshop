using Framework.Domain;

namespace Framework.Messaging
{
    public interface IHandle<T> where T : Command
    {
        void Handle(T command);
    }
}