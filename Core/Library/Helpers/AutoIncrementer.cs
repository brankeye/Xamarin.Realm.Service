using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using Realms;
using Realms.Schema;
using xr.service.core.Library.Attributes;
using xr.service.core.Library.Extensions;
using xr.service.core.Library.Utilities;

namespace xr.service.core.Library.Helpers
{
    public class AutoIncrementer<T>
        where T : RealmObject
    {
        public bool IsAutoIncrementEnabled { get; protected set; }

        public bool IsAutoIncrementConfigured { get; protected set; }
        
        private PropertyInfo PrimaryKeyProperty { get; set; }

        private long _lastId;

        public void Initialize()
        {
            FindPrimaryKeyProperty(typeof(T));
            IsAutoIncrementEnabled = PrimaryKeyProperty != null &&
                                     PrimaryKeyProperty.IsDefined(typeof(AutoIncrementAttribute)) &&
                                     PrimaryKeyProperty.PropertyType.IsIntegral();
        }

        public PropertyInfo FindPrimaryKeyProperty(Type type)
        {
            PrimaryKeyProperty = type.GetRuntimeProperties().FirstOrDefault(x => x.IsDefined(typeof(PrimaryKeyAttribute)));
            return PrimaryKeyProperty;
        }

        public void ConfigureAutoIncrement(Realm realm)
        {
            if (!IsAutoIncrementConfigured && IsAutoIncrementEnabled)
            {
                GetLastId(realm);
            }
            IsAutoIncrementConfigured = true;
        }

        public void GetLastId(Realm realm)
        {
            if (_lastId != 0) return;
            var primaryKeyGetter = Expressions.CreatePropertyGetter<T>(PrimaryKeyProperty);
            var item = realm.All<T>().OrderByDescending(primaryKeyGetter).FirstOrDefault();
            if (item != null) _lastId = (long)Convert.ChangeType(PrimaryKeyProperty.GetValue(item), typeof(long));
        }

        public void AutoIncrementPrimaryKey(T item)
        {
            var itemPrimaryKey = (long)Convert.ChangeType(PrimaryKeyProperty.GetValue(item), typeof(long));
            if (itemPrimaryKey == 0)
            {
                PrimaryKeyProperty.SetValue(item, Convert.ChangeType(GetAutoId(), PrimaryKeyProperty.PropertyType));
            }
        }

        private long GetAutoId()
        {
            return GetNextId();
        }

        private long GetNextId()
        {
            var nextId = Interlocked.Increment(ref _lastId);
            return nextId;
        }
    }
}
