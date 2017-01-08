using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using xr.service.core.Library.Extensions;
using xr.service.core.Library.Interfaces;
using xr.service.core.Library.Utilities;

namespace xr.service.core.Library.Helpers
{
    public class AutoIncrementer<T> : IAutoIncrementer<T>
    {
        public bool IsAutoIncrementEnabled { get; protected set; }

        public bool IsAutoIncrementConfigured { get; protected set; }
        
        protected PropertyInfo PrimaryKeyProperty { get; set; }

        private long _lastId;

        public AutoIncrementer(Type primaryKeyAttrType, Type autoIncrementAttrType)
        {
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

        public virtual bool AutoIncrementPrimaryKey(T item)
        {
            if (IsAutoIncrementEnabled && IsAutoIncrementConfigured)
            {
                var itemPrimaryKey = (long)Convert.ChangeType(PrimaryKeyProperty.GetValue(item), typeof(long));
                if (itemPrimaryKey == 0)
                {
                    PrimaryKeyProperty.SetValue(item, Convert.ChangeType(GetNextId(), PrimaryKeyProperty.PropertyType));
                }
                return true;
            }
            return false;
        }

        public virtual void ConfigureAutoIncrement(Func<Func<T, object>, T> getLargestPrimaryKeyQuery)
        {
            if (!IsAutoIncrementConfigured && IsAutoIncrementEnabled)
            {
                GetLastId(getLargestPrimaryKeyQuery);
            }
            IsAutoIncrementConfigured = true;
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
            if (item != null) _lastId = (long)Convert.ChangeType(PrimaryKeyProperty.GetValue(item), typeof(long));
        }

        private long GetNextId()
        {
            var nextId = Interlocked.Increment(ref _lastId);
            return nextId;
        }
    }
}
