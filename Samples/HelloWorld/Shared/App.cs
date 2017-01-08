using System.Linq;
using Realms;
using xr.service.core.Library;
using xr.service.samples.helloworld.Shared.Models;
using xr.service.samples.helloworld.Shared.Services.Realm;
using Xamarin.Forms;

namespace xr.service.samples.helloworld.Shared
{
    public class App : Application
    {
        public App()
        {
            TestService();
        }

        protected void TestService()
        {
            var realmService = RealmService.GetInstance<Models.Person>();

            realmService.Write(() =>
            {
                realmService.RemoveAll();
                realmService.Add(new Person { Name = "Greg" });
                realmService.Add(new Person { Name = "Jim" });
                realmService.Add(new Person { Name = "Bob" });
            });

            var persons = realmService.GetAll().ToList();

            // OR

            var personsRealm = new PersonsRealm();
            personsRealm.Write(() =>
            {
                personsRealm.Add(new Person { Name = "Jan" });
                personsRealm.Add(new Person { Name = "June" });
                personsRealm.Add(new Person { Name = "Jinny" });
            });

            var persons2 = personsRealm.GetAll().ToList();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
