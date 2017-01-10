using Realms;

namespace Library.Droid.Models
{
    public class Painting : RealmObject
    {
        [PrimaryKey]
        public string Id { get; set; }

        public string Name { get; set; }
    }
}