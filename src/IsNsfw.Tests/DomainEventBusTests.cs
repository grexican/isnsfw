using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IsNsfw.ServiceInterface.EventHandlers;
using IsNsfw.ServiceInterface.Events;
using NUnit.Framework;
using ServiceStack;

namespace IsNsfw.Tests
{
    public class DomainEventBusTests
    {
        private class Event1 : IEvent
        {

        }

        private class Event1Handler1 : IEventHandler<Event1>
        {
            public void Handle(Event1 @event)
            {
            }
        }

        private class Event1Handler2 : IEventHandler<Event1>
        {
            public void Handle(Event1 @event)
            {
            }
        }

        private class Event2 : IEvent
        {

        }

        [EventHandlerPriority(5)]
        private class Event2Handler1 : IEventHandler<Event2>
        {
            public void Handle(Event2 @event)
            {
            }
        }

        [EventHandlerPriority(-5)]
        private class Event2Handler2 : IEventHandler<Event2>
        {
            public void Handle(Event2 @event)
            {
            }
        }

        [EventHandlerPriority()]
        private class Event2Handler3 : IEventHandler<Event2>
        {
            public void Handle(Event2 @event)
            {
            }
        }

        private class Event2Handler4 : IEventHandler<Event2>
        {
            public void Handle(Event2 @event)
            {
            }
        }

        private SimpleInjector.Container GetContainer()
        {
            var container = new SimpleInjector.Container();
            container.Collection.Register(typeof(IEventHandler<>), this.GetType().Assembly);

            return container;
        }

        [Test]
        public void SimpleInjectorRegistersAllHandlers()
        {
            var sut = GetContainer();
            Assert.That(sut.GetAllInstances<IEventHandler<Event1>>().Count(), Is.EqualTo(2));
        }

        [Test]
        public void GetHandlersGetsAllHandlers()
        {
            var container = GetContainer();
            var sut = new DomainEventBus(container);

            Assert.That(sut.GetHandlers<Event1>().Count, Is.EqualTo(2));
        }

        [Test]
        public void GetHandlersHasCorrectPriortyOrder()
        {
            var container = GetContainer();
            var utilities = new DomainEventBus(container);
            var sut = utilities.GetHandlers<Event2>();

            Assert.That(sut.Count, Is.EqualTo(4));

            Assert.That(sut[0].GetType(), Is.EqualTo(typeof(Event2Handler2)), "Negative priority should be first");
            Assert.That(sut[1].GetType(), Is.EqualTo(typeof(Event2Handler3)), "Zero priority should be second");
            Assert.That(sut[2].GetType(), Is.EqualTo(typeof(Event2Handler1)), "Positive priority should be third");
            Assert.That(sut[3].GetType(), Is.EqualTo(typeof(Event2Handler4)), "Default priority (100) should be last");
        }
    }
}
