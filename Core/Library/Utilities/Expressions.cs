using System;
using System.Linq.Expressions;
using System.Reflection;

namespace xr.service.core.Library.Utilities
{
    public static class Expressions
    {
        public static Func<T, object> CreatePropertyGetter<T>(PropertyInfo propertyInfo)
        {
            var parameter = Expression.Parameter(typeof(T));
            var body = Expression.Convert(Expression.Property(parameter, propertyInfo.Name), typeof(object));
            var lambda = Expression.Lambda<Func<T, object>>(body, parameter);
            return lambda.Compile();
        }
    }
}
