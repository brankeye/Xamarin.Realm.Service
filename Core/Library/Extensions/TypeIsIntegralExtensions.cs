﻿using System;

namespace Xamarin.Realm.Service.Extensions
{
    public static class TypeIsIntegralExtensions
    {
        public static bool IsIntegral(this Type type)
        {
            if (type == null)
            {
                return false;
            }

            if (type == typeof(sbyte) ||
                type == typeof(byte) ||
                type == typeof(char) ||
                type == typeof(short) ||
                type == typeof(ushort) ||
                type == typeof(int) ||
                type == typeof(uint) ||
                type == typeof(long) ||
                type == typeof(ulong))
            {
                return true;
            }

            return false;
        }
    }
}