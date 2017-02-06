using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Realm.Service.Events;

namespace Xamarin.Realm.Service.Interfaces
{
    public interface IEventAggregator<T>
        where T : IEventRaiser
    {
        void Raise<TArgs>(string eventName, TArgs eventArgs) where TArgs : EventArgs;

        void AddEvent(string eventName);

        bool RemoveEvent(string eventName);
    }
}
