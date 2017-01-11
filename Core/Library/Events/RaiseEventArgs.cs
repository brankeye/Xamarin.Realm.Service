using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xamarin.Realm.Service.Events
{
    public class RaiseEventArgs : EventArgs
    {
        public RaiseEventArgs(string eventName, EventArgs args)
        {
            EventName = eventName;
            EventArgs = args;
        }

        public string EventName { get; set; }

        public EventArgs EventArgs { get; set; }
    }
}
