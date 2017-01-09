using System;

namespace Xamarin.Realm.Service.Interfaces
{
    public interface IAutoIncrementer<in T>
    {
        bool IsAutoIncrementEnabled { get; }

        bool IsAutoIncrementConfigured { get; }

        bool AutoIncrementPrimaryKey(T item);

        void ConfigureAutoIncrement(Func<Func<T, object>, T> getLargestPrimaryKeyQuery);
    }
}
