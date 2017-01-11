# Xamarin.Realm Service (XRS)
XRS is a service layer for Xamarin.Realm that provides threadsafe AutoIncrement support for primary keys, as well as an improved and extended API for working with RealmObjects.

NOTE: XRS may change as Xamarin.Realm development continues development toward Version 1.0.0.

## Nuget (coming)

## Usage
XRS is easy to use and works the same way under the hood as Xamarin.Realm.

### Using the AutoIncrement feature
Apply the AutoIncrement Attribute to any RealmObject with a primary key of an integral type (short, int, long, etc).

If the AutoIncrement attribute is applied to the primary key, the Add/AddOrUpdate service functions will autoincrement the primary key of the given RealmObject.
```csharp
public class Person : RealmObject
{
	[PrimaryKey, AutoIncrement]
	public int Id { get; set; }
}
```

### Using the service
Call `RealmService.GetInstance<T>()` to retrieve an `IRealmService<T>` instance.
```csharp
var personsRealmService = RealmService.GetInstance<Models.Person>();
var personsRealmServiceConfigured = RealmService.GetInstance<Models.Person>(realmConfiguration);
var personsRealmServiceByDB = RealmService.GetInstance<Models.Person>("path/to/database");
```

Or subclass the RealmService for each ```RealmObject``` as follows. This also allows you to override specific functions in the service should they not fit your needs.
```csharp
public class PersonsRealm : RealmService<Person> {
	public PersonsRealm(RealmConfigurationBase config = null) 
		: base(config) {}
}
```

### Writing and so on
Use XRS the same way you used Xamarin.Realm. See the API section below to view the functions currently implemented by the service.
```csharp
public void XRSTest() {
	var personsRealmService = RealmService.GetInstance<Person>();
	personsRealmService.Write(() =>
	{
		personsRealmService.Add(new Person { Name = "Greg" }); // Id = 1
		personsRealmService.Add(new Person { Name = "Jim" }); // Id = 2
		personsRealmService.Add(new Person { Name = "Bob" }); // Id = 3
		
		var homesRealmService = RealmService.GetInstance<Home>();
		homesRealmService.Remove(1); // Removes Home of Id = 1
	});
}
```

### Data-changing events
XRS provides four events that are invoked on data-changing methods in the service.

```WriteFinished``` is raised when ```Write``` or ```WriteAsync``` have finished.

```AddOrUpdateCollectionOccurred``` is raised when ```AddAll``` or ```AddOrUpdateAll``` have finished.

```RemoveCollectionOccurred``` is raised when ```RemoveAll``` has finished (both methods).



## API
The API follows Xamarin.Realm as closely as possible, while taking the opportunity to improve on the naming of certain functions.
For example, the `Add` function has been split into two distinct functions, `Add` and `AddOrUpdate`.

You can override each method and add your own logic or events as you please.

```csharp
public interface IRealmService<T> where T : RealmObject
{
	event EventHandler AddOrUpdateCollectionOccurred;
	event EventHandler RemoveCollectionOccurred;
	event EventHandler WriteFinished;

	Realms.Realm RealmInstance { get; }

	void Write(Action action);

	Task WriteAsync(Action<RealmService<T>> action, Action callback = null);

	Transaction BeginWrite();

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

	bool RefreshRealmInstance();

	bool IsSameRealmInstance(Realms.Realm other);
	
	bool IsSameRealmInstance(IRealmService<T> other);
	
	void DisposeRealmInstance();
}
```

## Extensibility
XRS is designed so that developers can customize the service if there are portions that do not work as expected.
All of the properties and functions defined in the API section above can be overridden and reimplemented.

The AutoIncrementer instance, used by the service to perform autoincrements on primary keys, can be switched out
for your own by overriding the `CreateAutoIncrementer()` function on the service, so long as your new AutoIncrementer implements the `IAutoIncrementer<T>` interface.

Have a look around the source code to see what can be changed.

## Feedback/Bugs/Suggestions
Feel free to add new issues for bugs, suggestions, API requests, etc. 

Feedback is highly appreciated!

## Xamarin.Realm
Realm Xamarin enables you to efficiently write your app’s model layer in a safe, persisted, and fast way.

See the [docs](https://realm.io/docs/xamarin/latest/) to learn how Xamarin.Realm works.

See the [changelog](https://github.com/realm/realm-dotnet/blob/master/CHANGELOG.md) to learn about recent updates.

See their [repository](https://github.com/realm/realm-dotnet) on GitHub.
