using System;
using System.Threading.Tasks;

namespace ConfigServer.Server
{
    internal interface IEventService
    {
        Task Publish<TEvent>(TEvent arg) where TEvent : IEvent;
    }

    internal class EventService : IEventService
    {
        private readonly IServiceProvider serviceProvider;

        public EventService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public Task Publish<TEvent>(TEvent arg) where TEvent : IEvent
        {
            var handler = (IEventHandler<TEvent>)serviceProvider.GetService(typeof(IEventHandler<TEvent>));
            return handler.Handle(arg);
        }
    }
}
