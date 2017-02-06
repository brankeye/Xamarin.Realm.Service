using System;
using System.Collections.Generic;
using System.Reflection;
using Xamarin.Realm.Service.Events;
using Xamarin.Realm.Service.Interfaces;

namespace Xamarin.Realm.Service.Components
{
    public class EventAggregator<T> : IEventAggregator<T>, IDisposable
        where T : IEventRaiser
    {
        protected static IDictionary<string, FieldInfo> Fields { get; set; } = new Dictionary<string, FieldInfo>();

        protected static event EventHandler<RaiseEventArgs> RaiseEvent;

        private bool _disposed;

        private T Source { get; }

        public EventAggregator(T source)
        {
            Source = source;
            RaiseEvent += OnRaiseEvent;
        }

        private void OnRaiseEvent(object sender, RaiseEventArgs raiseEventArgs)
        {
            RaiseEvents(raiseEventArgs.EventName, raiseEventArgs.EventArgs);
        }

        public void Raise<TArgs>(string eventName, TArgs eventArgs)
            where TArgs : EventArgs
        {
            RaiseEvent?.Invoke(Source, new RaiseEventArgs(eventName, eventArgs));
        }

        public void AddEvent(string eventName)
        {
            if (!Fields.ContainsKey(eventName))
            {
                var fieldInfo = Source.GetType().GetTypeInfo().GetDeclaredField(eventName);
                Fields.Add(eventName, fieldInfo);
            }
        }

        public bool RemoveEvent(string eventName)
        {
            return Fields.Remove(eventName);
        }

        protected virtual void RaiseEvents<TArgs>(string eventName, TArgs eventArgs)
            where TArgs : EventArgs
        {
            FieldInfo fieldInfo;
            Fields.TryGetValue(eventName, out fieldInfo);
            if (fieldInfo != null)
            {
                var eventDelegate = (MulticastDelegate)fieldInfo.GetValue(Source);
                var sourceObj = Source as object;
                if (eventDelegate != null)
                {
                    foreach (var handler in eventDelegate.GetInvocationList())
                    {
                        handler.GetMethodInfo().Invoke(handler.Target, new[] { sourceObj, eventArgs });
                    }
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                
            }

            RaiseEvent -= OnRaiseEvent;

            _disposed = true;
        }
    }
}
