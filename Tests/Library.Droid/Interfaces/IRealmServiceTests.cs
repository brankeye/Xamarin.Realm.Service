using System.Threading.Tasks;
using NUnit.Framework;

namespace Library.Droid.Interfaces
{
    public interface IRealmServiceTests
    {
        void WriteAsyncTests();

        void AddTests();

        void AddOrUpdateTests();

        void RemoveTests();
    }
}