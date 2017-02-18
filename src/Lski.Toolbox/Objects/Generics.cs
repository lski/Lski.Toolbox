using System;
using System.Collections.Generic;
using System.Reflection;

namespace Lski.Toolbox.Objects
{
    /// <summary>
    /// Functions for handling generic objects
    /// </summary>
    public static class Generics
    {
        /// <summary>
        /// Returns true or false depending on whether the passed type if a Nullable type
        /// </summary>
        public static bool IsNullableType(Type t) => IsNullableType(t.GetTypeInfo());

        /// <summary>
        /// Returns true or false depending on whether the passed type if a Nullable type
        /// </summary>
        public static bool IsNullableType(TypeInfo ti) => (ti.IsGenericType && ti.GetGenericTypeDefinition() == typeof(Nullable<>));

        /// <summary>
        /// Takes in an object of any type and checks to see if it is null, first against null, finally whether it is nullable and has no value.
        /// </summary>
        public static bool IsNull(Object o)
        {
            if (o == null)
            {
                return true;
            }

            var ti = o.GetType().GetTypeInfo();

            if (IsNullableType(ti))
            {
                if (!(Boolean)ti.GetDeclaredProperty("HasValue").GetValue(o, null))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Creates a new list of the passed type, containing the objects passed. Useful for creating annoymous objects and then creating a list of that type
        /// </summary>
        public static List<T> MakeList<T>(params T[] initialItems) => new List<T>(initialItems);

        /// <summary>
        /// Calls a SHARED method of the class
        /// </summary>
        public static T InvokeSharedFunction<T>(TypeInfo t, string methodName, object[] argList = null)
        {
            var method = FindMethod(t, methodName);

            if (method == null)
            {
                throw new ArgumentException("method could not be found");
            }

            return (T)method.Invoke(null, argList);
        }

        /// <summary>
        /// Calls a SHARED method of the class
        /// </summary>
        public static T InvokeSharedFunction<T>(Type t, string methodName, object[] argList = null) => InvokeSharedFunction<T>(t.GetTypeInfo(), methodName, argList);

        private static MethodInfo FindMethod(TypeInfo typeInfo, string methodName)
        {
            var method = typeInfo.GetDeclaredMethod(methodName);
            if (method == null && typeInfo.BaseType != null)
            {
                return FindMethod(typeInfo.BaseType.GetTypeInfo(), methodName);
            }
            return method;
        }
    }
}