using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Realms;
using Xamarin.Realm.Service.Attributes;
using Xamarin.Realm.Service.Helpers;
using Xamarin.Realm.Service.Interfaces;

namespace Xamarin.Realm.Service
{
    public static class RealmService
    {
        public static IRealmService<T> GetInstance<T>(RealmConfigurationBase config = null)
            where T : RealmObject
        {
            return RealmService<T>.GetInstance(config);
        }

        public static IRealmService<T> GetInstance<T>(string databasePath)
            where T : RealmObject
        {
            return RealmService<T>.GetInstance(databasePath);
        }

        public static void ConfigureAutoIncrement<T>()
            where T : RealmObject
        {
            RealmService<T>.GetInstance();
        }

        public static bool Compact(RealmConfigurationBase config = null)
        {
            return Realms.Realm.Compact(config);
        }

        public static bool Compact(string databasePath)
        {
            var realmInstance = Realms.Realm.GetInstance(databasePath);
            return Realms.Realm.Compact(realmInstance.Config);
        }

        public static void Delete(RealmConfigurationBase config = null)
        {
            if (config != null)
            {
                Realms.Realm.DeleteRealm(config);
            }
            else
            {
                var realmInstance = Realms.Realm.GetInstance();
                Realms.Realm.DeleteRealm(realmInstance.Config);
            }
        }

        public static void Delete(string databasePath)
        {
            var realmInstance = Realms.Realm.GetInstance(databasePath);
            Realms.Realm.DeleteRealm(realmInstance.Config);
        }
    }

    public class RealmService<T> : RealmServiceBase<T>
        where T : RealmObject
    {
        public event EventHandler AddOrUpdateOccurred;

        public event EventHandler AddOrUpdateCollectionOccurred;

        public event EventHandler RemoveOccurred;

        public event EventHandler RemoveCollectionOccurred;

        protected RealmService(RealmConfigurationBase config = null) : base(config) { }

        protected RealmService(string databasePath) : base(databasePath) { }

        public override RealmConfigurationBase Config => RealmInstance.Config;

        public override RealmSchema Schema => RealmInstance.Schema;

        public override bool IsClosed => RealmInstance.IsClosed;

        protected internal static IRealmService<T> GetInstance(RealmConfigurationBase config = null)
        {
            return new RealmService<T>(config);
        }

        protected internal static IRealmService<T> GetInstance(string databasePath)
        {
            return new RealmService<T>(databasePath);
        }

        protected override IAutoIncrementer<T> CreateAutoIncrementer()
        {
            return new AutoIncrementer<T>(typeof(PrimaryKeyAttribute), typeof(AutoIncrementAttribute));
        }

        public override void Write(Action action)
        {
            RealmInstance.Write(action);
        }

        public override Task WriteAsync(Action<RealmService<T>> action)
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

        public override Transaction BeginWrite()
        {
            return RealmInstance.BeginWrite();
        }

        public override void Add(T item)
        {
            if (IsAutoIncrementEnabled)
                AutoIncrementer.AutoIncrementPrimaryKey(item);
            RealmInstance.Add(item);
            AddOrUpdateOccurred?.Invoke(this, EventArgs.Empty);
        }

        public override void AddAll(IQueryable<T> list)
        {
            foreach (var item in list)
            {
                Add(item);
            }
        }

        public override void AddOrUpdate(T item)
        {
            if (IsAutoIncrementEnabled)
            {
                if (!AutoIncrementer.PrimaryKeyExists(item))
                {
                    
                }
                    AutoIncrementer?.AutoIncrementPrimaryKey(item);
            }
            RealmInstance.Add(item, true);
            AddOrUpdateOccurred?.Invoke(this, EventArgs.Empty);
        }

        public override void AddOrUpdateAll(IQueryable<T> list)
        {
            foreach (var item in list)
            {
                AddOrUpdate(item);
            }
            AddOrUpdateCollectionOccurred?.Invoke(this, EventArgs.Empty);
        }

        public override T Find(long? primaryKey)
        {
            return RealmInstance.Find<T>(primaryKey);
        }

        public override IQueryable<T> FindAll(IQueryable<long?> primaryKeys)
        {
            var list = new List<T>();
            foreach (var pk in primaryKeys)
            {
                var item = Find(pk);
                if(item != null) list.Add(item);
            }
            return list.AsQueryable();
        }

        public override T Find(string primaryKey)
        {
            return RealmInstance.Find<T>(primaryKey);
        }

        public override IQueryable<T> FindAll(IQueryable<string> primaryKeys)
        {
            var list = new List<T>();
            foreach (var pk in primaryKeys)
            {
                var item = Find(pk);
                if (item != null) list.Add(item);
            }
            return list.AsQueryable();
        }

        public override T Get(Expression<Func<T, bool>> predicate)
        {
            return RealmInstance.All<T>().FirstOrDefault(predicate);
        }

        public override IQueryable<T> GetAll()
        {
            return RealmInstance.All<T>();
        }

        public override IQueryable<T> GetAll(Expression<Func<T, bool>> predicate)
        {
            return RealmInstance.All<T>().Where(predicate);
        }

        public override void Remove(long? primaryKey)
        {
            var item = Find(primaryKey);
            Remove(item);
            RemoveOccurred?.Invoke(this, EventArgs.Empty);
        }

        public override void Remove(string primaryKey)
        {
            var item = Find(primaryKey);
            Remove(item);
            RemoveOccurred?.Invoke(this, EventArgs.Empty);
        }

        public override void Remove(T item)
        {
            RealmInstance.Remove(item);
            RemoveOccurred?.Invoke(this, EventArgs.Empty);
        }

        public override void RemoveAll()
        {
            RealmInstance.RemoveAll<T>();
            RemoveCollectionOccurred?.Invoke(this, EventArgs.Empty);
        }

        public override void RemoveAll(IQueryable<T> list)
        {
            RealmInstance.RemoveRange(list);
            RemoveCollectionOccurred?.Invoke(this, EventArgs.Empty);
        }

        public override bool Refresh()
        {
            return RealmInstance.Refresh();
        }

        public override bool IsSameRealmInstance(Realms.Realm realm)
        {
            return RealmInstance.IsSameInstance(realm);
        }

        public override bool IsSameRealmInstance(IRealmService<T> realmService)
        {
            return RealmInstance.IsSameInstance(realmService.RealmInstance);
        }

        public override void DisposeRealmInstance()
        {
            RealmInstance.Dispose();
        }
    }
}
