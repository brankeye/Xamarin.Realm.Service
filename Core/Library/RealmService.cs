using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Realms;
using Xamarin.Realm.Service.Attributes;
using Xamarin.Realm.Service.Events;
using Xamarin.Realm.Service.Extensions;
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
        internal static event EventHandler<RaiseEventArgs> RaiseEvent;
        public override event EventHandler AddOrUpdateCollectionOccurred;
        public override event EventHandler RemoveCollectionOccurred;
        public override event EventHandler WriteFinished;

        protected RealmService(RealmConfigurationBase config = null) : base(config)
        {
            RaiseEvent += OnRaiseEvent;
        }

        protected RealmService(string databasePath) : base(databasePath)
        {
            RaiseEvent += OnRaiseEvent;
        }

        private void OnRaiseEvent(object sender, RaiseEventArgs raiseEventArgs)
        {
            this.Raise(raiseEventArgs.EventName, raiseEventArgs.EventArgs);
        }

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
            //return new AutoIncrementer<T>(typeof(PrimaryKeyAttribute), typeof(AutoIncrementAttribute));
            if (AutoIncrementer<T>.Current == null) new AutoIncrementer<T>(typeof(PrimaryKeyAttribute), typeof(AutoIncrementAttribute));
            return AutoIncrementer<T>.Current;
        }

        public override void Write(Action action)
        {
            RealmInstance.Write(action);
            RaiseEvent?.Invoke(this, new RaiseEventArgs(nameof(WriteFinished), EventArgs.Empty));
        }

        public override Task WriteAsync(Action<RealmService<T>> action, Action callback = null)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            return Task.Run(() =>
            {
                var realmService = new RealmService<T>(RealmInstance.Config);
                using (var transaction = realmService.BeginWrite())
                {
                    //AttachEvents(realmService);
                    action(realmService);
                    transaction.Commit();
                    //DetachEvents(realmService);
                }
                RaiseEvent?.Invoke(this, new RaiseEventArgs(nameof(WriteFinished), EventArgs.Empty));
                callback?.BeginInvoke(callback.EndInvoke, null);
            });
        }

        public override Transaction BeginWrite()
        {
            return RealmInstance.BeginWrite();
        }

        public override T Add(T item)
        {
            if (IsAutoIncrementEnabled)
                AutoIncrementer.AutoIncrementPrimaryKey(item);
            var result = RealmInstance.Add(item, false);
            //RaiseEvent?.Invoke(this, new RaiseEventArgs(nameof(AddOrUpdateOccurred), EventArgs.Empty));
            return result;
        }

        public override IQueryable<T> AddAll(IQueryable<T> list)
        {
            var result = new List<T>();
            foreach (var item in list)
            {
                result.Add(Add(item));
            }
            RaiseEvent?.Invoke(this, new RaiseEventArgs(nameof(AddOrUpdateCollectionOccurred), EventArgs.Empty));
            return result.AsQueryable();
        }

        public override T AddOrUpdate(T item)
        {
            if (IsAutoIncrementEnabled)
            {
                if (!AutoIncrementer.PrimaryKeyExists(item))
                {
                    AutoIncrementer.AutoIncrementPrimaryKey(item);
                }
            }
            var result = RealmInstance.Add(item, true);
            return result;
        }

        public override IQueryable<T> AddOrUpdateAll(IQueryable<T> list)
        {
            var result = new List<T>();
            foreach (var item in list)
            {
                result.Add(AddOrUpdate(item));
            }
            RaiseEvent?.Invoke(this, new RaiseEventArgs(nameof(AddOrUpdateCollectionOccurred), EventArgs.Empty));
            return result.AsQueryable();
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
        }

        public override void Remove(string primaryKey)
        {
            var item = Find(primaryKey);
            Remove(item);
        }

        public override void Remove(T item)
        {
            RealmInstance.Remove(item);
        }

        public override void RemoveAll()
        {
            RealmInstance.RemoveAll<T>();
            RaiseEvent?.Invoke(this, new RaiseEventArgs(nameof(RemoveCollectionOccurred), EventArgs.Empty));
        }

        public override void RemoveAll(IQueryable<T> list)
        {
            RealmInstance.RemoveRange(list);
            RaiseEvent?.Invoke(this, new RaiseEventArgs(nameof(RemoveCollectionOccurred), EventArgs.Empty));
        }

        public override bool RefreshRealmInstance()
        {
            return RealmInstance.Refresh();
        }

        public override bool IsSameRealmInstance(Realms.Realm other)
        {
            return RealmInstance.IsSameInstance(other);
        }

        public override bool IsSameRealmInstance(IRealmService<T> other)
        {
            return RealmInstance.IsSameInstance(other.RealmInstance);
        }

        public override void DisposeRealmInstance()
        {
            RealmInstance.Dispose();
        }
    }
}