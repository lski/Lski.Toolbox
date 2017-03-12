using System;
using System.Collections.Generic;
using System.Linq;

namespace Lski.Toolbox.Collections
{
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Adds the ability to ask for the to call Max, without exception if the enumerable is empty. On empty the default value of T is returned
        /// </summary>
        public static T MaxOrDefault<T>(this IEnumerable<T> e)
        {
            foreach (var i in e)
            {
                //If reaches here its gone to the first item so contains items, so simply call max
                return e.Max();
            }

            return default(T);
        }

        /// <summary>
        /// Adds the ability to ask for the to call Min, without exception if the enumerable is empty. On empty the default value of T is returned
        /// </summary>
        public static T MinOrDefault<T>(this IEnumerable<T> e)
        {
            foreach (var i in e)
            {
                //If reaches here its gone to the first item so contains items, so simply call min
                return e.Min();
            }

            return default(T);
        }

        /// <summary>
        /// Works the same as string.IndexOf, returning the index of the item found or -1 if not found
        /// </summary>
        public static int IndexOf<T>(this IEnumerable<T> enumerable, T find)
        {
            int i = 0;

            foreach (var item in enumerable)
            {
                if (item.Equals(find))
                {
                    return i;
                }

                i++;
            }

            return -1;
        }

        /// <summary>
        /// Gives a way of 
        /// </summary>
        public static bool Any<T>(this IEnumerable<T> enumerable, T value)
        {
            return IndexOf(enumerable, value) > -1;
        }

        /// <summary>
        /// Covert an enumerable so it can be used in a using statement <see cref="DisposableEnumerator"/>
        /// </summary>
        public static DisposableEnumerator<T> AsDisposable<T>(this IEnumerable<T> enumerable) where T : IDisposable
        {
            return new DisposableEnumerator<T>(enumerable);
        }
    }
}