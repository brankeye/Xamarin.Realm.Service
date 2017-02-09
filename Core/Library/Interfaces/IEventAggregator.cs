using System;

namespace Xamarin.Realm.Service.Interfaces
{
    public interface IEventAggregator<T>
        where T : IEventRaiser
    {
        void Raise<TArgs>(string eventName, TArgs eventArgs) where TArgs : EventArgs;

        void AddTarget(object target);
    }
}
