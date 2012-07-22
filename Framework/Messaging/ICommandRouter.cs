using Framework.Domain;

namespace Framework.Messaging
{
    public interface ICommandRouter
    {
        void Route(Command c);
    }
}