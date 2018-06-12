using IsNsfw.Model;

namespace IsNsfw.ServiceInterface.Events
{
    public class LinkEventCreatedEvent : IEvent
    {
        public LinkEvent LinkEvent { get; }

        public LinkEventCreatedEvent(LinkEvent linkEvent)
        {
            LinkEvent = linkEvent;
        }
    }
}