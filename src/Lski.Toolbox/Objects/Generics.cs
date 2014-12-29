using System;
using System.Collections.Generic;
using System.Reflection;

namespace Lski.Toolbox.Objects {

    /// <summary>
    /// Functions for handling generic objects
    /// </summary>
    public static class Generics {

        /// <summary>
        /// Returns true or false depending on whether the passed type if a Nullable type
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool IsNullableType(Type t) {

            var ti = t.GetTypeInfo();
            return (ti.IsGenericType && ti.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        /// <summary>
        /// Takes in an object of any type and checks to see if it is null, first against null, finally whether it is nullable and has no value.
        /// </summary>
        public static bool IsNull(Object o) {

            if (o == null) {
                return true;
            }

            var t = o.GetType();

            if (IsNullableType(t)) {
                if (!(Boolean)t.GetProperty("HasValue").GetValue(o, null)) {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Creates a new list of the passed type, containing the objects passed. Useful for creating annoymous objects and then creating a list of that type
        /// </summary>
        public static List<T> MakeList<T>(params T[] initialItems) {

            return new List<T>(initialItems);
        }

        /// <summary>
        /// Calls a SHARED method of the class
        /// </summary>
        public static T InvokeSharedFunction<T>(Type t, string method, object[] argList = null) {

            return (T)t.GetTypeInfo().GetMethod(method).Invoke(Activator.CreateInstance(t), argList);
        }
    }
}