using System;
using System.Collections.Generic;
using System.Text;

namespace GeneralPurposeBot
{
    public enum Event
    {
        UserVoiceStateUpdated,
        MessageReceived,
        MessageUpdated
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class EventListenerAttribute : Attribute
    {
        public Event Event { get; }
        public EventListenerAttribute(Event ev)
        {
            Event = ev;
        }
    }
}
