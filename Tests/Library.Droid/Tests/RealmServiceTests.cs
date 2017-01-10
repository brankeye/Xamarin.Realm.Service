using System;
using System.Linq;
using System.Threading.Tasks;
using Library.Droid.Helpers;
using Library.Droid.Interfaces;
using Library.Droid.Models;
using Library.Droid.Services.Realms;
using NUnit.Framework;
using Xamarin.Realm.Service;

namespace Library.Droid.Tests
{
    [TestFixture]
    public class RealmServiceTests : IRealmServiceTests
    {
        [Test]
        public void AddOrUpdateTests()
        {
            // No Primary Key
            var countriesService = RealmService.GetInstance<Models.Country>();
            countriesService.Write(() =>
            {
                countriesService.RemoveAll();
                countriesService.AddOrUpdate(new Country { Name = "Canada" });
            });
            var country1 = countriesService.Get(x => x.Name == "Canada");
            Assert.That(country1, Is.Not.Null);

            // Primary Key, AutoIncrement Disabled
            var animalsService = RealmService.GetInstance<Models.Animal>();
            animalsService.Write(() =>
            {
                animalsService.RemoveAll();
                animalsService.AddOrUpdate(new Animal { Id = 3, Name = "Bertha" });
            });
            var animal1 = animalsService.Find(3);
            Assert.That(animal1, Is.Not.Null);
            Assert.That(animal1.Id, Is.EqualTo(3));
            Assert.That(animal1.Name, Is.EqualTo("Bertha"));
            animalsService.Write(() =>
            {
                animalsService.AddOrUpdate(new Animal { Id = 3, Name = "Greg" });
            });
            animal1 = animalsService.Find(3);
            Assert.That(animal1, Is.Not.Null);
            Assert.That(animal1.Id, Is.EqualTo(3));
            Assert.That(animal1.Name, Is.EqualTo("Greg"));
            animalsService.Write(() =>
            {
                animalsService.RemoveAll();
            });

            // Primary Key, AutoIncrement Enabled
            var personsService = RealmService.GetInstance<Models.Person>();
            personsService.Write(() =>
            {
                personsService.RemoveAll();
                personsService.AddOrUpdate(new Person { Name = "Gregory" });
            });
            var person1 = personsService.Get(x => x.Name == "Gregory");
            var id = person1.Id;
            Assert.That(person1, Is.Not.Null);
            Assert.That(person1.Name, Is.EqualTo("Gregory"));
            personsService.Write(() =>
            {
                personsService.AddOrUpdate(new Person { Id = id, Name = "Jim" });
            });
            person1 = personsService.Find(id);
            var persons = personsService.GetAll();
            Assert.That(person1.Name, Is.EqualTo("Jim"));
            personsService.Write(() =>
            {
                personsService.RemoveAll();
            });
        }

        [Test]
        public void AddTests()
        {
            // No Primary Key
            var countriesService = RealmService.GetInstance<Models.Country>();
            countriesService.Write(() =>
            {
                countriesService.RemoveAll();
                countriesService.Add(new Country { Name = "Canada" });
            });
            var country1 = countriesService.Get(x => x.Name == "Canada");
            Assert.That(country1, Is.Not.Null);
            countriesService.Write(() =>
            {
                countriesService.RemoveAll();
            });

            // Primary Key, AutoIncrement Disabled
            var animalsService = RealmService.GetInstance<Models.Animal>();
            animalsService.Write(() =>
            {
                animalsService.RemoveAll();
                animalsService.Add(new Animal { Name = "Gregory" });
                animalsService.Add(new Animal { Id = 3, Name = "Bertha" });
            });
            var animal1 = animalsService.Get(x => x.Name == "Gregory");
            var animal2 = animalsService.Get(x => x.Name == "Bertha");
            Assert.That(animal1, Is.Not.Null);
            Assert.That(animal1.Id, Is.EqualTo(0));
            Assert.That(animal2, Is.Not.Null);
            Assert.That(animal2.Id, Is.EqualTo(3));
            animalsService.Write(() =>
            {
                animalsService.RemoveAll();
            });
            
            // Primary Key, AutoIncrement Enabled
            var personsService = RealmService.GetInstance<Models.Person>();
            personsService.Write(() =>
            {
                personsService.RemoveAll();
                personsService.Add(new Person { Name = "Gregory" });
                personsService.Add(new Person { Id = -1, Name = "Bertha" });
            });
            var person1 = personsService.Get(x => x.Name == "Gregory");
            var person2 = personsService.Get(x => x.Name == "Bertha");
            Assert.That(person1, Is.Not.Null);
            Assert.That(person1.Id, Is.GreaterThan(0));
            Assert.That(person2, Is.Not.Null);
            Assert.That(person2.Id, Is.GreaterThan(0));
            personsService.Write(() =>
            {
                personsService.RemoveAll();
            });
        }

        [Test]
        public void RemoveTests()
        {
            // No Primary Key
            var countriesService = RealmService.GetInstance<Models.Country>();
            countriesService.Write(() =>
            {
                countriesService.RemoveAll();
                countriesService.Add(new Country { Name = "Canada" });
            });
            var country1 = countriesService.Get(x => x.Name == "Canada");
            Assert.That(country1, Is.Not.Null);
            countriesService.Write(() =>
            {
                countriesService.Remove(countriesService.Get(x => x.Name == "Canada"));
            });
            country1 = countriesService.Get(x => x.Name == "Canada");
            Assert.That(country1, Is.Null);

            // Primary Key, AutoIncrement Disabled
            var animalsService = RealmService.GetInstance<Models.Animal>();
            animalsService.Write(() =>
            {
                animalsService.RemoveAll();
                animalsService.Add(new Animal { Id = 3, Name = "Bertha" });
            });
            var animal1 = animalsService.Get(x => x.Name == "Bertha");
            Assert.That(animal1, Is.Not.Null);
            animalsService.Write(() =>
            {
                animalsService.Remove(animalsService.Get(x => x.Name == "Bertha"));
            });
            animal1 = animalsService.Get(x => x.Name == "Bertha");
            Assert.That(animal1, Is.Null);
            animalsService.Write(() =>
            {
                animalsService.RemoveAll();
            });

            // Primary Key, AutoIncrement Enabled
            var personsService = RealmService.GetInstance<Models.Person>();
            personsService.Write(() =>
            {
                personsService.RemoveAll();
                personsService.Add(new Person { Name = "Gregory" });
            });
            var person1 = personsService.Get(x => x.Name == "Gregory");
            Assert.That(person1, Is.Not.Null);
            personsService.Write(() =>
            {
                personsService.Remove(personsService.Get(x => x.Name == "Gregory"));
            });
            person1 = personsService.Get(x => x.Name == "Gregory");
            Assert.That(person1, Is.Null);
            personsService.Write(() =>
            {
                personsService.RemoveAll();
            });
        }
        
        [Test]
        public void WriteAsyncTests()
        {
            var personsRealm = new PersonsRealmService();
            personsRealm.Write(() => personsRealm.RemoveAll());
            RunAsyncServiceTests().Wait();
            personsRealm.Refresh();
            var sorted = personsRealm.GetAll().OrderBy(x => x.Id).ToList();
            Assert.That(sorted.Count, Is.EqualTo(50000));
            var firstId = sorted.First().Id;
            var lastId = sorted.Last().Id;
            var total = lastId - firstId + 1;
            Assert.That(total, Is.EqualTo(50000));
            personsRealm.Write(() => personsRealm.RemoveAll());
        }

        [Test]
        public void WriteAsyncTimeTest()
        {
            // Got 8.79 secs, 9.19 secs, and 10.227 secs for 50000 small items added using autoincrementer.
            var personsRealm = new PersonsRealmService();
            personsRealm.Write(() => personsRealm.RemoveAll());
            RunAsyncServiceTests().Wait();
            Assert.Pass();
        }

        protected async Task RunAsyncServiceTests()
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
                RunAsyncTest());
        }

        protected Task RunAsyncTest()
        {
            var personsRealm = new PersonsRealmService();
            return personsRealm.WriteAsync(realmService =>
            {
                for (var i = 0; i < 1000; i++)
                {
                    realmService.Add(new Person { Name = "Jan" });
                    realmService.Add(new Person { Name = "June" });
                    realmService.Add(new Person { Name = "Jinny" });
                    realmService.Add(new Person { Name = "Jane" });
                    realmService.Add(new Person { Name = "Jessica" });
                }
            });
        }
    }
}
