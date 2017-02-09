using System;
using System.Linq;
using System.Threading.Tasks;
using xr.service.samples.helloworld.Shared.Models;
using xr.service.samples.helloworld.Shared.Services.Realm;
using Xamarin.Forms;
using Xamarin.Realm.Service;
using Xamarin.Realm.Service.Components;

namespace xr.service.samples.helloworld.Shared
{
    public class App : Application
    {
        //protected WeakReference service, aggregator;

        public App()
        {
            RunEventTests();
            //RunAsyncServiceTests();

            MainPage = new ContentPage();
        }

        protected void RunEventTests()
        {
            var realmService = RealmService.GetInstance<Models.Person>();
            var realmServiceTwo = RealmService.GetInstance<Models.Person>();

            realmService.WriteFinished += (sender, args) =>
            {
                var msg = "Woohoo!";
            };

            realmServiceTwo.WriteFinished += (sender, args) =>
            {
                var msg = "Woohoo!";
            };

            realmService.Write(() =>
            {
                realmService.Add(new Person { Name = "Greg" });
                realmService.Add(new Person { Name = "Jim" });
                realmService.Add(new Person { Name = "Bob" });
            });

            realmServiceTwo.Write(() =>
            {
                realmService.Add(new Person { Name = "Greg" });
                realmService.Add(new Person { Name = "Jim" });
                realmService.Add(new Person { Name = "Bob" });
            });
        }

        protected void RunServiceTests()
        {
            var realmService = RealmService.GetInstance<Models.Person>();

            realmService.Write(() =>
            {
                realmService.Add(new Person { Name = "Greg" });
                realmService.Add(new Person { Name = "Jim" });
                realmService.Add(new Person { Name = "Bob" });
            });

            // OR

            var personsRealm = new PersonsRealmService();
            personsRealm.Write(() =>
            {
                personsRealm.Add(new Person { Name = "Jan" });
                personsRealm.Add(new Person { Name = "June" });
                personsRealm.Add(new Person { Name = "Jinny" });
            });

            var persons = personsRealm.GetAll().ToList();

            personsRealm.Write(() =>
            {
                personsRealm.RemoveAll();
            });
        }

        protected async void RunAsyncServiceTests()
        {
            await Task.WhenAll(
                RunAsyncTest(),
                RunAsyncTest(),
                RunAsyncTest(),
                RunAsyncTest(),
                RunAsyncTest(),
                RunAsyncTest(),
                RunAsyncTest(),
                RunAsyncTest(),
                RunAsyncTest(),
                RunAsyncTest(),
                RunAsyncTest(),
                RunAsyncTest());

            var personsRealm = new PersonsRealmService();
            var persons = personsRealm.GetAll().ToList();

            personsRealm.Write(() =>
            {
                personsRealm.RemoveAll();
            });
        }

        protected Task RunAsyncTest()
        {
            var personsRealm = new PersonsRealmService();
            return personsRealm.WriteAsync(realmService =>
            {
                realmService.Add(new Person { Name = "Jan" });
                realmService.Add(new Person { Name = "June" });
                realmService.Add(new Person { Name = "Jinny" });
                realmService.Add(new Person { Name = "Jan" });
                realmService.Add(new Person { Name = "June" });
                realmService.Add(new Person { Name = "Jinny" });
                realmService.Add(new Person { Name = "Jan" });
                realmService.Add(new Person { Name = "June" });
                realmService.Add(new Person { Name = "Jinny" });
                realmService.Add(new Person { Name = "Jan" });
                realmService.Add(new Person { Name = "June" });
                realmService.Add(new Person { Name = "Jinny" });
                realmService.Add(new Person { Name = "Jan" });
                realmService.Add(new Person { Name = "June" });
                realmService.Add(new Person { Name = "Jinny" });
                realmService.Add(new Person { Name = "Jan" });
                realmService.Add(new Person { Name = "June" });
                realmService.Add(new Person { Name = "Jinny" });
                realmService.Add(new Person { Name = "Jan" });
                realmService.Add(new Person { Name = "June" });
                realmService.Add(new Person { Name = "Jinny" });
                realmService.Add(new Person { Name = "Jan" });
                realmService.Add(new Person { Name = "June" });
                realmService.Add(new Person { Name = "Jinny" });
            });
        }

        protected override void OnStart()
        {
            // Handle when your app starts
            //var realmService = new PersonsRealmService();
            //service = new WeakReference(realmService);
            //realmService = null;

            //var agg = new EventAggregator<PersonsRealmService>();
            //aggregator = new WeakReference(agg);
            //agg = null;

            //GC.Collect();
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
            //GC.Collect();
            //var serviceIsAlive = service.IsAlive;
            //var aggIsAlive = aggregator.IsAlive;
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
