using Realms;
using Xamarin.Realm.Service.Interfaces;

namespace Library.Droid.Helpers
{
    public class RealmHelper
    {
        public static void Clear<T>(IRealmService<T> realmService)
            where T : RealmObject
        {
            Realm.DeleteRealm(realmService.Config);
        }
    }
}