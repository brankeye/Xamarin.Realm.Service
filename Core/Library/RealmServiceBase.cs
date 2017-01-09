﻿using System;
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
        protected static IAutoIncrementer<T> AutoIncrementer { get; private set; }

        protected Realms.Realm RealmInstance => GetRealmInstance;
        private Realms.Realm _realmInstance;

        protected Func<Realms.Realm> RealmGetter { get; }

        protected RealmServiceBase()
        {
            RealmGetter = () => Realms.Realm.GetInstance();
            InitializeInternal();
        }

        protected RealmServiceBase(RealmConfigurationBase config)
        {
            RealmGetter = () => Realms.Realm.GetInstance(config);
            InitializeInternal();
        }

        protected RealmServiceBase(string databasePath)
        {
            RealmGetter = () => Realms.Realm.GetInstance(databasePath);
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
                AutoIncrementer.ConfigureAutoIncrement(GetLargestPrimaryKeyQuery);
            }
        }

        protected virtual T GetLargestPrimaryKeyQuery(Func<T, object> pkGetter)
        {
            return RealmInstance.All<T>().OrderByDescending(pkGetter).FirstOrDefault();
        }

        protected abstract IAutoIncrementer<T> CreateAutoIncrementer();

        protected virtual Realms.Realm GetRealmInstance => _realmInstance ?? (_realmInstance = RealmGetter.Invoke());

        public abstract RealmConfigurationBase Config { get; }

        public abstract RealmSchema Schema { get; }

        public abstract bool IsClosed { get; }

        public abstract void Write(Action action);

        public abstract Task WriteAsync(Action<RealmService<T>> action);

        public abstract Transaction BeginWrite();

        public abstract void Add(T item);

        public abstract void AddAll(IQueryable<T> list);

        public abstract void AddOrUpdate(T item);

        public abstract void AddOrUpdateAll(IQueryable<T> list);

        public abstract T Find(long? primaryKey);

        public abstract T Find(string primaryKey);

        public abstract T Get(Expression<Func<T, bool>> predicate);

        public abstract IQueryable<T> GetAll();

        public abstract IQueryable<T> GetAll(Expression<Func<T, bool>> predicate);

        public abstract IQueryable<T> GetAllOrdered(Expression<Func<T, bool>> orderPredicate);

        public abstract IQueryable<T> GetAllOrdered(Expression<Func<T, bool>> wherePredicate, Expression<Func<T, bool>> orderPredicate);

        public abstract void Remove(long? primaryKey);

        public abstract void Remove(string primaryKey);

        public abstract void Remove(T item);

        public abstract void RemoveAll();

        public abstract void RemoveAll(IQueryable<T> list);

        public abstract bool Refresh();

        public abstract bool IsSameInstance(Realms.Realm realm);

        public abstract void Dispose();
    }
}
