using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Realms;

namespace Xamarin.Realm.Service.Interfaces
{
    /// <summary>
    /// A Realm Service represents a Realm service layer to a specific RealmObject.
    /// </summary>
    /// <remarks> A Realm Service defines a Realm instance per thread.
    /// You must call <see cref="RealmService.GetInstance()"/> on each thread in which you want to interact with the Realm Service. 
    /// </remarks>
    public interface IRealmService<T>
        where T : RealmObject
    {
        /// <summary>
        /// A Realm instance (also referred to as a Realm) represents a Realm database.
        /// </summary>
        /// <remarks>
        /// <b>Warning</b>: Realm instances are not thread safe and can not be shared across threads.
        /// You must call <see cref="RealmService.GetInstance()"/> on each thread in which you want to interact with the Realm. 
        /// </remarks>
        Realms.Realm RealmInstance { get; }

        /// <summary>
        /// Execute an action inside a temporary <see cref="Transaction"/>. If no exception is thrown, the <see cref="Transaction"/> 
        /// will be committed.
        /// </summary>
        /// <remarks>
        /// Creates its own temporary <see cref="Transaction"/> and commits it after running the lambda passed to <c>action</c>. 
        /// Be careful of wrapping multiple single property updates in multiple <see cref="Write"/> calls. 
        /// It is more efficient to update several properties or even create multiple objects in a single <see cref="Write"/>,
        /// unless you need to guarantee finer-grained updates.
        /// </remarks>
        /// <param name="action">
        /// Action to perform inside a <see cref="Transaction"/>, creating, updating or removing objects.
        /// </param>
        void Write(Action action);

        /// <summary>
        /// Execute an action inside a temporary <see cref="Transaction"/> on a worker thread. If no exception is thrown,
        /// the <see cref="Transaction"/> will be committed.
        /// </summary>
        /// <remarks>
        /// Opens a new instance of a Realm Service on a worker thread and executes <c>action</c> inside a write <see cref="Transaction"/>.
        /// <see cref="Realms.Realm"/>s and <see cref="RealmObject"/>s are thread-affine, so capturing any such objects in 
        /// the <c>action</c> delegate will lead to errors if they're used on the worker thread.
        /// </remarks>
        /// <param name="action">
        /// Action to perform inside a <see cref="Transaction"/>, creating, updating, or removing objects.
        /// </param>
        /// <returns>An awaitable <see cref="Task"/>.</returns>
        Task WriteAsync(Action<RealmService<T>> action);

        /// <summary>
        /// Factory for a write <see cref="Transaction"/>. Essential object to create scope for updates.
        /// </summary>
        /// <returns>A transaction in write mode, which is required for any creation or modification of objects persisted in a <see cref="Realm"/>.</returns>
        Transaction BeginWrite();

        /// <summary>
        /// This <see cref="Realms.Realm"/> will start managing a <see cref="RealmObject"/> which has been created as a standalone object.
        /// </summary>
        /// <typeparam name="T">
        /// The Type T must not only be a <see cref="RealmObject"/> but also have been processed by the Fody weaver,
        /// so it has persistent properties.
        /// </typeparam>
        /// <param name="item">Must be a standalone object, <c>null</c> not allowed.</param>
        /// <exception cref="RealmInvalidTransactionException">
        /// If you invoke this when there is no write <see cref="Transaction"/> active on the <see cref="Realms.Realm"/>.
        /// </exception>
        /// <exception cref="RealmObjectManagedByAnotherRealmException">
        /// You can't manage an object with more than one <see cref="Realms.Realm"/>.
        /// </exception>
        /// <remarks>
        /// If the object is already managed by this <see cref="Realms.Realm"/>, this method does nothing.
        /// This method modifies the object in-place, meaning that after it has run, <c>obj</c> will be managed. 
        /// Returning it is just meant as a convenience to enable fluent syntax scenarios.
        /// Cyclic graphs (<c>Parent</c> has <c>Child</c> that has a <c>Parent</c>) will result in undefined behavior.
        /// You have to break the cycle manually and assign relationships after all object have been managed.
        /// </remarks>
        /// <returns>The passed object, so that you can write <c>var person = realm.Add(new Person { Id = 1 });</c></returns>
        T Add(T item);

        IQueryable<T> AddAll(IQueryable<T> list);

        T AddOrUpdate(T item);

        IQueryable<T> AddOrUpdateAll(IQueryable<T> list);

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
