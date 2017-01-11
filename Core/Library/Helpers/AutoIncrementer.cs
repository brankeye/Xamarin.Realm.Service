using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using Xamarin.Realm.Service.Extensions;
using Xamarin.Realm.Service.Interfaces;
using Xamarin.Realm.Service.Utilities;

namespace Xamarin.Realm.Service.Helpers
{
    public class AutoIncrementer<T> : IAutoIncrementer<T>
    {
        public static IAutoIncrementer<T> Current { get; private set; }

        public bool IsAutoIncrementEnabled { get; protected set; }

        public bool IsAutoIncrementConfigured { get; protected set; }
        
        public PropertyInfo PrimaryKeyProperty { get; protected set; }

        private long _lastId;

        public AutoIncrementer(Type primaryKeyAttrType, Type autoIncrementAttrType)
        {
            if(Current == null) Current = this;
            InitializeInternal(primaryKeyAttrType, autoIncrementAttrType);
        }

        private void InitializeInternal(Type primaryKeyAttrType, Type autoIncrementAttrType)
        {
            Initialize(primaryKeyAttrType, autoIncrementAttrType);
        }

        protected virtual void Initialize(Type primaryKeyAttrType, Type autoIncrementAttrType)
        {
            PrimaryKeyProperty = FindPropertyWithAttribute(typeof(T), primaryKeyAttrType);
            IsAutoIncrementEnabled = IsAutoIncrementerEnabled(autoIncrementAttrType);
        }

        public virtual long? GetPrimaryKey(T item)
        {
            if (IsAutoIncrementEnabled && IsAutoIncrementConfigured)
            {
                var pk = PrimaryKeyProperty.GetValue(item);
                return Convert.ToInt64(pk);
            }
            return null;
        }

        public virtual bool PrimaryKeyExists(T item)
        {
            var pk = PrimaryKeyProperty.GetValue(item);
            var pkLong = Convert.ToInt64(pk);
            return pkLong > 0 && pkLong <= _lastId;
        }

        public virtual bool AutoIncrementPrimaryKey(T item)
        {
            if (IsAutoIncrementEnabled && IsAutoIncrementConfigured)
            {
                PrimaryKeyProperty.SetValue(item, Convert.ChangeType(GetNextId(), PrimaryKeyProperty.PropertyType));
                return true;
            }
            return false;
        }

        public virtual bool ConfigureAutoIncrement(Func<Func<T, object>, T> getLargestPrimaryKeyQuery)
        {
            if (!IsAutoIncrementConfigured && IsAutoIncrementEnabled)
            {
                GetLastId(getLargestPrimaryKeyQuery);
                IsAutoIncrementConfigured = true;
            }
            return IsAutoIncrementConfigured;
        }

        protected virtual PropertyInfo FindPropertyWithAttribute(Type modelType, Type attrType)
        {
            return modelType.GetRuntimeProperties().FirstOrDefault(x => x.IsDefined(attrType));
        }

        protected virtual bool IsAutoIncrementerEnabled(Type autoIncrementAttrType)
        {
            var result = PrimaryKeyProperty != null &&
                         PrimaryKeyProperty.IsDefined(autoIncrementAttrType) &&
                         PrimaryKeyProperty.PropertyType.IsIntegral();
            return result;
        }

        private void GetLastId(Func<Func<T, object>, T> getLargestPrimaryKeyQuery)
        {
            var primaryKeyGetter = Expressions.CreatePropertyGetter<T>(PrimaryKeyProperty);
            var item = getLargestPrimaryKeyQuery.Invoke(primaryKeyGetter);
            if (item != null) _lastId = Convert.ToInt64(PrimaryKeyProperty.GetValue(item));
        }

        private long GetNextId()
        {
            var nextId = Interlocked.Increment(ref _lastId);
            return nextId;
        }
    }
}
