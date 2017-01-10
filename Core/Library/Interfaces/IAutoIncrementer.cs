using System;

namespace Xamarin.Realm.Service.Interfaces
{
    public interface IAutoIncrementer<in T>
    {
        bool IsAutoIncrementEnabled { get; }

        bool IsAutoIncrementConfigured { get; }

        long? GetPrimaryKey(T item);

        bool PrimaryKeyExists(T item);

        bool AutoIncrementPrimaryKey(T item);

        bool ConfigureAutoIncrement(Func<Func<T, object>, T> getLargestPrimaryKeyQuery);
    }
}
