using System;

namespace Hzeff.Events
{
    internal interface IEventBinding<T>
    {
        public Action<T> OnEvent { get; set; }
        public Action OnEventWithoutArgs { get; set; }
    }
}