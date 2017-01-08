using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xr.service.core.Library.Interfaces
{
    public interface IAutoIncrementer<in T>
    {
        bool IsAutoIncrementEnabled { get; }

        bool IsAutoIncrementConfigured { get; }

        bool AutoIncrementPrimaryKey(T item);

        void ConfigureAutoIncrement(Func<Func<T, object>, T> getLargestPrimaryKeyQuery);
    }
}
