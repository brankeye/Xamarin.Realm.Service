using Library.Droid.Helpers;
using Library.Droid.Models;
using Library.Droid.Services.Realms;
using NUnit.Framework;

namespace Library.Droid
{
    [TestFixture]
    public class TestService
    {
        [Test]
        public void TestApi()
        {
            var personsRealm = new PersonsRealmService();
            personsRealm.Write(() =>
            {
                personsRealm.Add(new Person { Name = "Jan" });
            });
            var name = personsRealm.Find(1).Name;
            Assert.That(name, Is.EqualTo("Jan"));

            RealmHelper.Clear(personsRealm);
        }
    }
}
