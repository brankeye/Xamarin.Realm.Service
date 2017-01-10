using Realms;

namespace Library.Droid.Models
{
    public class Animal : RealmObject
    {
        [PrimaryKey]
        public long Id { get; set; }

        public string Name { get; set; }
    }
}
