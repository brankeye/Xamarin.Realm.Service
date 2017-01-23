using System;

namespace Xamarin.Realm.Service.Extensions
{
    internal static class TypeIsIntegralExtensions
    {
        internal static bool IsIntegral(this Type type)
        {
            if (type == null)
            {
                return false;
            }

            if (type == typeof(byte) ||
                type == typeof(char) ||
                type == typeof(short) ||
                type == typeof(int) ||
                type == typeof(long))
            {
                return true;
            }

            return false;
        }
    }
}
