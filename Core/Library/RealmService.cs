using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Realms;
using xr.service.core.Library.Attributes;
using xr.service.core.Library.Extensions;
using xr.service.core.Library.Utilities;

namespace xr.service.core.Library
{
    public static class RealmService
    {
        public static RealmService<T> GetInstance<T>()
            where T : RealmObject
        {
            return new RealmService<T>();
        }

        public static RealmService<T> GetInstance<T>(RealmConfigurationBase config)
            where T : RealmObject
        {
            return new RealmService<T>(config);
        }

        public static RealmService<T> GetInstance<T>(string databasePath)
            where T : RealmObject
        {
            return new RealmService<T>(databasePath);
        }

        public static bool Compact(RealmConfigurationBase config = null)
        {
            return Realm.Compact(config);
        }

        public static bool Compact(string databasePath)
        {
            var realmInstance = Realm.GetInstance(databasePath);
            return Realm.Compact(realmInstance.Config);
        }

        public static void Delete(RealmConfigurationBase config = null)
        {
            if (config != null)
            {
                Realm.DeleteRealm(config);
            }
            else
            {
                var realmInstance = Realm.GetInstance();
                Realm.DeleteRealm(realmInstance.Config);
            }
        }

        public static void Delete(string databasePath)
        {
            var realmInstance = Realm.GetInstance(databasePath);
            Realm.DeleteRealm(realmInstance.Config);
        }
    }

    public class RealmService<T>
        where T : RealmObject
    {
        public RealmConfigurationBase Config => RealmInstance.Config;

        public RealmSchema Schema => RealmInstance.Schema;

        public bool IsClosed => RealmInstance.IsClosed;

        protected static PropertyInfo Property { get; set; }

        protected static bool IsAutoIncrementEnabled { get; }

        protected static bool IsAutoIncrementConfigured { get; private set; }

        protected Realm RealmInstance => _realmInstance ?? (_realmInstance = RealmGetter.Invoke());
        private Realm _realmInstance;

        private static Func<Realm> RealmGetter { get; set; }

        private static long _lastId;

        static RealmService()
        {
            var type = typeof(T);
            Property = type.GetRuntimeProperties().FirstOrDefault(x => x.IsDefined(typeof(PrimaryKeyAttribute)));
            if (Property != null)
            {
                if (Property.PropertyType.IsIntegral())
                {
                    if (Property.IsDefined(typeof(AutoIncrementAttribute))) IsAutoIncrementEnabled = true;
                }
            }
        }

        public RealmService()
        {
            RealmGetter = () => Realm.GetInstance();
            ConfigureAutoIncrement(RealmInstance);
        }

        public RealmService(RealmConfigurationBase config)
        {
            RealmGetter = () => Realm.GetInstance(config);
            ConfigureAutoIncrement(RealmInstance);
        }

        public RealmService(string databasePath)
        {
            RealmGetter = () => Realm.GetInstance(databasePath);
            ConfigureAutoIncrement(RealmInstance);
        }

        public virtual void Write(Action action)
        {
            RealmInstance.Write(action);
        }

        public Task WriteAsync(Action<RealmService<T>> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            return Task.Run(() =>
            {
                var realmService = new RealmService<T>();
                using (var transaction = realmService.BeginWrite())
                {
                    action(realmService);
                    transaction.Commit();
                }
            });
        }

        public Transaction BeginWrite()
        {
            return RealmInstance.BeginWrite();
        }

        public virtual void Add(T item)
        {
            if (IsAutoIncrementEnabled) AutoIncrementPrimaryKey(item);
            RealmInstance.Add(item);
        }

        public virtual void AddAll(IQueryable<T> list)
        {
            foreach (var item in list)
            {
                Add(item);
            }
        }

        public virtual void AddOrUpdate(T item)
        {
            if (IsAutoIncrementEnabled) AutoIncrementPrimaryKey(item);
            RealmInstance.Add(item, true);
        }

        public virtual void AddOrUpdateAll(IQueryable<T> list)
        {
            foreach (var item in list)
            {
                AddOrUpdate(item);
            }
        }

        public virtual T CreateObject(string className)
        {
            var item = RealmInstance.CreateObject(className);
            if (IsAutoIncrementEnabled) AutoIncrementPrimaryKey(item);
            return item;
        }

        public virtual T Find(long? primaryKey)
        {
            return RealmInstance.Find<T>(primaryKey);
        }

        public virtual T Find(string primaryKey)
        {
            return RealmInstance.Find<T>(primaryKey);
        }

        public virtual T Find(Expression<Func<T, bool>> predicate)
        {
            return RealmInstance.All<T>().FirstOrDefault(predicate);
        }

        public virtual IQueryable<T> GetAll()
        {
            return RealmInstance.All<T>();
        }

        public virtual IQueryable<T> GetAll(Expression<Func<T, bool>> predicate)
        {
            return RealmInstance.All<T>().Where(predicate);
        }

        public virtual IQueryable<T> GetAllOrdered(Expression<Func<T, bool>> orderPredicate)
        {
            return RealmInstance.All<T>().OrderBy(orderPredicate);
        }

        public virtual IQueryable<T> GetAllOrdered(Expression<Func<T, bool>> wherePredicate, Expression<Func<T, bool>> orderPredicate)
        {
            return RealmInstance.All<T>().Where(wherePredicate).OrderBy(orderPredicate);
        }

        public virtual void Remove(long? primaryKey)
        {
            var item = Find(primaryKey);
            Remove(item);
        }

        public virtual void Remove(string primaryKey)
        {
            var item = Find(primaryKey);
            Remove(item);
        }

        public virtual void Remove(T item)
        {
            RealmInstance.Remove(item);
        }

        public virtual void RemoveAll()
        {
            RealmInstance.RemoveAll<T>();
        }

        public virtual void RemoveAll(IQueryable<T> list)
        {
            RealmInstance.RemoveRange(list);
        }

        public virtual bool Refresh()
        {
            return RealmInstance.Refresh();
        }

        public virtual bool IsSameInstance(Realm realm)
        {
            return RealmInstance.IsSameInstance(realm);
        }

        public virtual void Dispose()
        {
            RealmInstance.Dispose();
            _realmInstance = null;
        }

        private void AutoIncrementPrimaryKey(T item)
        {
            var itemPrimaryKey = (long)Convert.ChangeType(Property.GetValue(item), typeof(long));
            if (itemPrimaryKey == 0)
            {
                Property.SetValue(item, Convert.ChangeType(GetAutoId(), Property.PropertyType));
            }
        }

        private static long GetAutoId()
        {
            return GetNextId();
        }

        private static long GetNextId()
        {
            var nextId = Interlocked.Increment(ref _lastId);
            return nextId;
        }

        private static void ConfigureAutoIncrement(Realm realm)
        {
            if (!IsAutoIncrementConfigured && IsAutoIncrementEnabled)
            {
                GetLastId(realm);
            }
            IsAutoIncrementConfigured = true;
        }

        private static void GetLastId(Realm realm)
        {
            if (_lastId != 0) return;
            var primaryKeyGetter = Expressions.CreatePropertyGetter<T>(Property);
            var item = realm.All<T>().OrderByDescending(primaryKeyGetter).FirstOrDefault();
            if(item != null) _lastId = (long) Convert.ChangeType(Property.GetValue(item), typeof(long));
        }
    }
}
