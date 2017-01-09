using Realms;
using Xamarin.Realm.Service.Attributes;

namespace Library.Droid.Models
{
    public class Person : RealmObject
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }
    }
}