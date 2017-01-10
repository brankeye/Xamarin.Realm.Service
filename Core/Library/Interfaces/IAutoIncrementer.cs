using System;
using System.Reflection;

namespace Xamarin.Realm.Service.Interfaces
{
    public interface IAutoIncrementer<in T>
    {
        PropertyInfo PrimaryKeyProperty { get; }

        bool IsAutoIncrementEnabled { get; }

        bool IsAutoIncrementConfigured { get; }

        long? GetPrimaryKey(T item);

        bool PrimaryKeyExists(T item);

        bool AutoIncrementPrimaryKey(T item);

        bool ConfigureAutoIncrement(Func<Func<T, object>, T> getLargestPrimaryKeyQuery);
    }
}
