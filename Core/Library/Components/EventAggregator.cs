using System;
using System.Collections.Generic;
using System.Reflection;
using Xamarin.Realm.Service.Interfaces;

namespace Xamarin.Realm.Service.Components
{
    public class EventAggregator<T> : IEventAggregator<T>
        where T : IEventRaiser
    {
        public static IEventAggregator<T> Current { get; internal set; }

        private IDictionary<string, FieldInfo> Fields { get; } = new Dictionary<string, FieldInfo>();

        private IList<WeakReference> Sources { get; } = new List<WeakReference>();

        public EventAggregator()
        {
            RemoveDeadSources();
            AddEventsInternal();
        }

        public virtual void AddTarget(object target)
        {
            Sources.Add(new WeakReference(target));
        }

        public void Raise<TArgs>(string eventName, TArgs eventArgs)
            where TArgs : EventArgs
        {
            foreach (var source in Sources)
            {
                if (source.IsAlive)
                {
                    RaiseEvent(source.Target, eventName, eventArgs);
                }
            }
        }

        protected virtual void RaiseEvent<TArgs>(object target, string eventName, TArgs eventArgs)
            where TArgs : EventArgs
        {
            FieldInfo fieldInfo;
            Fields.TryGetValue(eventName, out fieldInfo);
            if (fieldInfo != null)
            {
                var eventDelegate = (MulticastDelegate)fieldInfo.GetValue(target);
                if (eventDelegate != null)
                {
                    foreach (var handler in eventDelegate.GetInvocationList())
                    {
                        handler.GetMethodInfo().Invoke(handler.Target, new[] { target, eventArgs });
                    }
                }
            }
        }

        protected void RemoveDeadSources()
        {
            (Sources as List<WeakReference>)?.RemoveAll(x => !x.IsAlive);
        }

        private void AddEventsInternal()
        {
            AddEvents();
        }

        protected void AddEvents()
        {
            var typeInfo = typeof(T).GetTypeInfo();
            var enumerator = typeInfo.DeclaredEvents;
            foreach (var eventInfo in enumerator)
            {
                var eventName = eventInfo.Name;
                var fieldInfo = typeInfo.GetDeclaredField(eventName);
                Fields.Add(eventName, fieldInfo);
            }
        }
    }
}
