using System.Threading.Tasks;

namespace ConfigServer.Server
{
    interface IEventHandler<TEvent> where TEvent : IEvent
    {
        Task Handle(TEvent arg);
    }
}
