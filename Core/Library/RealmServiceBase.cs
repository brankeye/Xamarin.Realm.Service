using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Realms;
using Xamarin.Realm.Service.Interfaces;

namespace Xamarin.Realm.Service
{
    public abstract class RealmServiceBase<T> : IRealmService<T>
        where T : RealmObject
    {
        public Realms.Realm RealmInstance { get; }

        protected bool IsAutoIncrementEnabled => AutoIncrementer != null && AutoIncrementer.IsAutoIncrementEnabled;

        protected static IAutoIncrementer<T> AutoIncrementer { get; private set; }

        protected RealmServiceBase(RealmConfigurationBase config = null)
        {
            RealmInstance = Realms.Realm.GetInstance(config);
            InitializeInternal();
        }

        protected RealmServiceBase(string databasePath)
        {
            RealmInstance = Realms.Realm.GetInstance(databasePath);
            InitializeInternal();
        }

        private void InitializeInternal()
        {
            Initialize();
        }

        protected virtual void Initialize()
        {
            if (AutoIncrementer == null)
            {
                AutoIncrementer = CreateAutoIncrementer();
                if (!AutoIncrementer.IsAutoIncrementEnabled) AutoIncrementer = null;
                AutoIncrementer?.ConfigureAutoIncrement(GetLargestPrimaryKeyQuery);
            }
        }

        protected virtual T GetLargestPrimaryKeyQuery(Func<T, object> pkGetter)
        {
            return RealmInstance.All<T>().OrderByDescending(pkGetter).FirstOrDefault();
        }

        protected abstract IAutoIncrementer<T> CreateAutoIncrementer();

        public abstract RealmConfigurationBase Config { get; }

        public abstract RealmSchema Schema { get; }

        public abstract bool IsClosed { get; }

        public abstract void Write(Action action);

        public abstract Task WriteAsync(Action<RealmService<T>> action);

        public abstract Transaction BeginWrite();
        
        public abstract T Add(T item);

        public abstract IQueryable<T> AddAll(IQueryable<T> list);

        public abstract T AddOrUpdate(T item);

        public abstract IQueryable<T> AddOrUpdateAll(IQueryable<T> list);

        public abstract T Find(long? primaryKey);

        public abstract T Find(string primaryKey);

        public abstract IQueryable<T> FindAll(IQueryable<long?> primaryKey);

        public abstract IQueryable<T> FindAll(IQueryable<string> primaryKey);

        public abstract T Get(Expression<Func<T, bool>> predicate);

        public abstract IQueryable<T> GetAll();

        public abstract IQueryable<T> GetAll(Expression<Func<T, bool>> predicate);

        public abstract void Remove(long? primaryKey);

        public abstract void Remove(string primaryKey);

        public abstract void Remove(T item);

        public abstract void RemoveAll();

        public abstract void RemoveAll(IQueryable<T> list);

        public abstract bool RefreshRealmInstance();

        public abstract bool IsSameRealmInstance(Realms.Realm other);

        public abstract bool IsSameRealmInstance(IRealmService<T> other);

        public abstract void DisposeRealmInstance();
    }
}
