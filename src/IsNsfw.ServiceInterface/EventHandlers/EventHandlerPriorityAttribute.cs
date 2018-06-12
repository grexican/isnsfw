using System;
using System.Collections.Generic;
using System.Text;

namespace IsNsfw.ServiceInterface.EventHandlers
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EventHandlerPriorityAttribute : Attribute
    {
        public const int UnRegisteredHandlerPriority = 100;

        public int Priority { get; set; }

        public EventHandlerPriorityAttribute(int priority = 0)
        {
            Priority = priority;
        }
    }
}
