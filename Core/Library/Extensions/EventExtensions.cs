using System;
using System.Reflection;
using Xamarin.Realm.Service.Interfaces;

namespace Xamarin.Realm.Service.Extensions
{
    public static class EventExtensions
    {
        internal static void Raise<TEventArgs>(this IEventRaiser source, string eventName, TEventArgs eventArgs) where TEventArgs : EventArgs
        {
            var fieldInfo = source.GetType().GetTypeInfo().GetDeclaredField(eventName);
            if (fieldInfo != null)
            {
                var eventDelegate = (MulticastDelegate) fieldInfo.GetValue(source);
                var sourceObj = source as object;
                if (eventDelegate != null)
                {
                    foreach (var handler in eventDelegate.GetInvocationList())
                        handler.GetMethodInfo().Invoke(handler.Target, new [] { sourceObj, eventArgs });
                }
            }
        }
    }
}
