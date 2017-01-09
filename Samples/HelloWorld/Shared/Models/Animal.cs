using Realms;
using Xamarin.Realm.Service.Attributes;

namespace xr.service.samples.helloworld.Shared.Models
{
    public class Animal : RealmObject
    {
        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }

        public string Name { get; set; }
    }
}
