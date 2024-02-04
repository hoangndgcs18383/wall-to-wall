using System;

namespace Hzeff.Events
{
    public class EventBinding<T> : IEventBinding<T> where T : IEvent
    {
        Action<T> onEvent = _ => { };
        Action onEventWithoutArgs = delegate { };

        Action<T> IEventBinding<T>.OnEvent
        {
            get => onEvent;
            set => onEvent = value;
        }

        Action IEventBinding<T>.OnEventWithoutArgs
        {
            get => onEventWithoutArgs;
            set => onEventWithoutArgs = value;
        }

        public EventBinding(Action<T> onEvent) => this.onEvent = onEvent;

        public EventBinding(Action onEventWithoutArgs) => this.onEventWithoutArgs = onEventWithoutArgs;

        public void AddListener(Action<T> listener) => onEvent += listener;
        public void AddListener(Action listener) => onEventWithoutArgs += listener;

        public void RemoveListener(Action<T> listener) => onEvent -= listener;
        public void RemoveListener(Action listener) => onEventWithoutArgs -= listener;
    }
}