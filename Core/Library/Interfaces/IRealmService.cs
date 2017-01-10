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

        /// <summary>
        /// Adds the items in <param name="list"></param> to the Realm using the <see cref="Add"/> method for each item.
        /// </summary>
        /// <param name="list"></param>
        /// <returns>A queryable collection of the items added without further filtering.</returns>
        IQueryable<T> AddAll(IQueryable<T> list);

        /// <summary>
        /// This <see cref="Realms.Realm"/> will start managing a <see cref="RealmObject"/> which has been created as a standalone object.
        /// </summary>
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
        /// Cyclic graphs (<c>Parent</c> has <c>Child</c> that has a <c>Parent</c>) will result in undefined behavior.
        /// You have to break the cycle manually and assign relationships after all object have been managed.
        /// </remarks>
        /// <returns>The passed object, so that you can write <c>var person = realm.Add(new Person { Id = 1 });</c></returns>
        T AddOrUpdate(T item);

        /// <summary>
        /// Adds or updates the items in <param name="list"></param> to the Realm using the <see cref="AddOrUpdate"/> method for each item.
        /// </summary>
        /// <param name="list"></param>
        /// <returns>A queryable collection of the items added or updated without further filtering.</returns>
        IQueryable<T> AddOrUpdateAll(IQueryable<T> list);

        /// <summary>
        /// Fast lookup of an object from a class which has a PrimaryKey property.
        /// </summary>
        /// <typeparam name="T">The Type T must be a <see cref="RealmObject"/>.</typeparam>
        /// <param name="primaryKey">
        /// Primary key to be matched exactly, same as an == search.
        /// An argument of type <c>long?</c> works for all integer properties, supported as PrimaryKey.
        /// </param>
        /// <returns><c>null</c> or an object matching the primary key.</returns>
        /// <exception cref="RealmClassLacksPrimaryKeyException">
        /// If the <see cref="RealmObject"/> class T lacks <see cref="PrimaryKeyAttribute"/>.
        /// </exception>
        T Find(long? primaryKey);

        /// <summary>
        /// Fast lookup of an object from a class which has a PrimaryKey property.
        /// </summary>
        /// <typeparam name="T">The Type T must be a <see cref="RealmObject"/>.</typeparam>
        /// <param name="primaryKey">Primary key to be matched exactly, same as an == search.</param>
        /// <returns><c>null</c> or an object matching the primary key.</returns>
        /// <exception cref="RealmClassLacksPrimaryKeyException">
        /// If the <see cref="RealmObject"/> class T lacks <see cref="PrimaryKeyAttribute"/>.
        /// </exception>
        T Find(string primaryKey);

        /// <summary>
        /// Fast lookup of all objects with the given list of primary keys.
        /// </summary>
        /// <param name="primaryKeys"></param>
        /// <returns>A queryable collection of the items added or updated without further filtering.</returns>
        IQueryable<T> FindAll(IQueryable<long?> primaryKeys);

        /// <summary>
        /// Fast lookup of all objects with the given list of primary keys.
        /// </summary>
        /// <param name="primaryKeys"></param>
        /// <returns>A queryable collection of the items added or updated without further filtering.</returns>
        IQueryable<T> FindAll(IQueryable<string> primaryKeys);

        /// <summary>
        /// Retrieves a RealmObject by querying for the item using the <param name="predicate"></param>.
        /// </summary>
        /// <param name="predicate">A predicate to search with.</param>
        /// <returns></returns>
        T Get(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Extract an iterable set of objects for direct use or further query.
        /// </summary>
        /// <typeparam name="T">The Type T must be a <see cref="RealmObject"/>.</typeparam>
        /// <returns>A queryable collection that without further filtering, allows iterating all objects of class T, in this <see cref="Realms.Realm"/>.</returns>
        IQueryable<T> GetAll();

        /// <summary>
        /// Extract an iterable set of objects for direct use or further query.
        /// </summary>
        /// <typeparam name="T">The Type T must be a <see cref="RealmObject"/>.</typeparam>
        /// <param name="predicate">A predicate to search with.</param>
        /// <returns>A queryable collection that without further filtering, allows iterating all objects of class T, in this <see cref="Realms.Realm"/>.</returns>
        IQueryable<T> GetAll(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Removes the persistent object from this Realm, effectively deleting it.
        /// </summary>
        /// <param name="primaryKey">Must be the primary key of the RealmObject persisted in this realm.</param>
        /// <exception cref="RealmInvalidTransactionException">
        /// If you invoke this when there is no write <see cref="Transaction"/> active on the <see cref="Realms.Realm"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">If <c>obj</c> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If you pass a standalone object.</exception>
        void Remove(long? primaryKey);

        /// <summary>
        /// Removes the persistent object from this Realm, effectively deleting it.
        /// </summary>
        /// <param name="primaryKey">Must be the primary key of the RealmObject persisted in this realm.</param>
        /// <exception cref="RealmInvalidTransactionException">
        /// If you invoke this when there is no write <see cref="Transaction"/> active on the <see cref="Realms.Realm"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">If <c>obj</c> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If you pass a standalone object.</exception>
        void Remove(string primaryKey);

        /// <summary>
        /// Removes the persistent object from this Realm, effectively deleting it.
        /// </summary>
        /// <param name="item">Must be an object persisted in this Realm.</param>
        /// <exception cref="RealmInvalidTransactionException">
        /// If you invoke this when there is no write <see cref="Transaction"/> active on the <see cref="Realms.Realm"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">If <c>obj</c> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If you pass a standalone object.</exception>
        void Remove(T item);

        /// <summary>
        /// Remove all objects of type <typeparam name="T"></typeparam> from the Realm.
        /// </summary>
        /// <typeparam name="T">Type of the objects to remove.</typeparam>
        /// <exception cref="RealmInvalidTransactionException">
        /// If you invoke this when there is no write <see cref="Transaction"/> active on the <see cref="Realms.Realm"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the type T is not part of the limited set of classes in this Realm's <see cref="Realms.Schema"/>.
        /// </exception>
        void RemoveAll();

        /// <summary>
        /// Remove objects matching a query from the Realm.
        /// </summary>
        /// <typeparam name="T">Type of the objects to remove.</typeparam>
        /// <param name="list">The query to match for.</param>
        /// <exception cref="RealmInvalidTransactionException">
        /// If you invoke this when there is no write <see cref="Transaction"/> active on the <see cref="Realm"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <c>range</c> is not the result of <see cref="GetAll()"/> or subsequent LINQ filtering.
        /// </exception>
        /// <exception cref="ArgumentNullException">If <c>range</c> is <c>null</c>.</exception>
        void RemoveAll(IQueryable<T> list);

        /// <summary>
        /// Update the <see cref="RealmInstance"/> instance and outstanding objects to point to the most recent persisted version.
        /// </summary>
        /// <returns>
        /// Whether the <see cref="RealmInstance"/> had any updates. Note that this may return true even if no data has actually changed.
        /// </returns>
        bool RefreshRealmInstance();

        /// <summary>
        /// Determines whether this instance is the same core instance as the passed in argument.
        /// </summary>
        /// <remarks>
        /// You can, and should, have multiple instances open on different threads which have the same path and open the same Realm.
        /// </remarks>
        /// <returns><c>true</c> if this instance is the same core instance; otherwise, <c>false</c>.</returns>
        /// <param name="other">The Realm to compare with the current Realm.</param>
        bool IsSameRealmInstance(Realms.Realm other);

        /// <summary>
        /// Determines whether this instance is the same core instance as the RealmInstance of the passed in argument.
        /// </summary>
        /// <remarks>
        /// You can, and should, have multiple instances open on different threads which have the same path and open the same Realm.
        /// </remarks>
        /// <returns><c>true</c> if this instance is the same core instance; otherwise, <c>false</c>.</returns>
        /// <param name="other">The Realm to compare with the current Realm.</param>
        bool IsSameRealmInstance(IRealmService<T> other);

        /// <summary>
        ///  Dispose automatically closes the RealmInstance if not already closed.
        /// </summary>
        void DisposeRealmInstance();
    }
}
