using Realms;

namespace xr.service.samples.helloworld.Shared.Models
{
    public class Person : RealmObject
    {
        [PrimaryKey]
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
