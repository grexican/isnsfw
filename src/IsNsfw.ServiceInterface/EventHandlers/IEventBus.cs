using System;
using System.Collections.Generic;
using System.Text;
using IsNsfw.ServiceInterface.Events;

namespace IsNsfw.ServiceInterface.EventHandlers
{
    public interface IEventBus
    {
        void Handle<TEvent>(TEvent @event) where TEvent : IEvent;
    }
}
