using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Realms;

namespace Xamarin.Realm.Service.Interfaces
{
    public interface IRealmService<T>
        where T : RealmObject
    {
        RealmConfigurationBase Config { get; }

        RealmSchema Schema { get; }

        bool IsClosed { get; }

        void Write(Action action);

        Task WriteAsync(Action<RealmService<T>> action);

        Transaction BeginWrite();

        void Add(T item);

        void AddAll(IQueryable<T> list);

        void AddOrUpdate(T item);

        void AddOrUpdateAll(IQueryable<T> list);

        T Find(long? primaryKey);

        T Find(string primaryKey);

        T Get(Expression<Func<T, bool>> predicate);

        IQueryable<T> GetAll();

        IQueryable<T> GetAll(Expression<Func<T, bool>> predicate);

        IQueryable<T> GetAllOrdered(Expression<Func<T, bool>> orderPredicate);

        IQueryable<T> GetAllOrdered(Expression<Func<T, bool>> wherePredicate, Expression<Func<T, bool>> orderPredicate);

        void Remove(long? primaryKey);

        void Remove(string primaryKey);

        void Remove(T item);

        void RemoveAll();

        void RemoveAll(IQueryable<T> list);

        bool Refresh();

        bool IsSameInstance(Realms.Realm realm);

        void Dispose();
    }
}
