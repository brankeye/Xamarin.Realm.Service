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
        Realms.Realm RealmInstance { get; }

        void Write(Action action);

        Task WriteAsync(Action<RealmService<T>> action);

        Transaction BeginWrite();

        void Add(T item);

        void AddAll(IQueryable<T> list);

        void AddOrUpdate(T item);

        void AddOrUpdateAll(IQueryable<T> list);

        T Find(long? primaryKey);

        T Find(string primaryKey);

        IQueryable<T> FindAll(IQueryable<long?> primaryKeys);

        IQueryable<T> FindAll(IQueryable<string> primaryKeys);

        T Get(Expression<Func<T, bool>> predicate);

        IQueryable<T> GetAll();

        IQueryable<T> GetAll(Expression<Func<T, bool>> predicate);

        void Remove(long? primaryKey);

        void Remove(string primaryKey);

        void Remove(T item);

        void RemoveAll();

        void RemoveAll(IQueryable<T> list);

        bool Refresh();

        bool IsSameRealmInstance(Realms.Realm realm);

        bool IsSameRealmInstance(IRealmService<T> realmService);

        void DisposeRealmInstance();
    }
}
