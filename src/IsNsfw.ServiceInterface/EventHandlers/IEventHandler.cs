using IsNsfw.ServiceInterface.Events;

namespace IsNsfw.ServiceInterface.EventHandlers
{
    public interface IEventHandler<in TEvent> where TEvent : IEvent
    {
        void Handle(TEvent @event);
    }
}
