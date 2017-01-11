using System;
using System.Reflection;

namespace Xamarin.Realm.Service.Extensions
{
    public static class EventExtensions
    {
        internal static void Raise<TEventArgs>(this object source, string eventName, TEventArgs eventArgs) where TEventArgs : EventArgs
        {
            var fieldInfo = source.GetType().GetTypeInfo().GetDeclaredField(eventName);
            if (fieldInfo != null)
            {
                var eventDelegate = (MulticastDelegate) fieldInfo.GetValue(source);
                // If there's any subscribed events...
                if (eventDelegate != null)
                {
                    // Invoke their raise methods
                    foreach (var handler in eventDelegate.GetInvocationList())
                        handler.GetMethodInfo().Invoke(handler.Target, new [] { source, eventArgs });
                }
            }
        }
    }
}
