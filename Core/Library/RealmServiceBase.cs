using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Realms;
using xr.service.core.Library.Interfaces;

namespace xr.service.core.Library
{
    public abstract class RealmServiceBase<T>
        where T : RealmObject
    {
        public RealmServiceBase()
        {
            
        }

        protected abstract IAutoIncrementer<T> CreateAutoIncrementer();
    }
}
