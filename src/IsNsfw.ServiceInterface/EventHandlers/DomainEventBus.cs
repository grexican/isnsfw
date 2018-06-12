using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using IsNsfw.ServiceInterface.Events;
using SimpleInjector;
using ServiceStack;

namespace IsNsfw.ServiceInterface.EventHandlers
{
    public class DomainEventBus : IEventBus
    {
        private readonly Container _container;
        private readonly ConcurrentDictionary<Type, object> _cache = new ConcurrentDictionary<Type, object>();

        public DomainEventBus(Container container)
        {
            _container = container;
        }

        public IList<IEventHandler<TEvent>> GetHandlers<TEvent>() where TEvent: IEvent
        {
            if(!_cache.TryGetValue(typeof(TEvent), out var h))
            {
                h = _container.GetAllInstances<IEventHandler<TEvent>>()
                    .OrderBy(m => m.GetType().HasAttribute<EventHandlerPriorityAttribute>() 
                        ? ((EventHandlerPriorityAttribute)m.GetType().GetCustomAttribute(typeof(EventHandlerPriorityAttribute))).Priority 
                        : EventHandlerPriorityAttribute.UnRegisteredHandlerPriority)
                    .ToList();

                _cache[typeof(TEvent)] = h;
            }

            return (IList<IEventHandler<TEvent>>)h;
        }

        public void Handle<TEvent>(TEvent @event) where TEvent : IEvent
        {
            var handlers = GetHandlers<TEvent>();

            foreach(var handler in handlers)
            {
                handler.Handle(@event);
            }
        }
    }
}
