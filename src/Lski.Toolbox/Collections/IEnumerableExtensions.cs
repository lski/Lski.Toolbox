using System;
using System.Collections.Generic;
using System.Linq;

namespace Lski.Toolbox.Collections
{
    /// <summary>
    /// A selection of extension methods to IEnumberable
    /// </summary>
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Loops through each element in the IEnumerable, useful for chaining. Does not modify the original IEnumerable.
        /// </summary>
        /// <remarks>
        /// There are normally much better ways to work with enumerables, such as for loops or using Select to create a new IEnumerable, however as this function
        /// returns the original enumerable it is useful for chaining.
        /// </remarks>
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var item in enumerable)
            {
                action(item);
            }

            return enumerable;
        }


        /// <summary>
        /// Allows switching of the OrderBy direction based on a bool flag
        /// </summary>
        public static IOrderedEnumerable<TInput> OrderByEither<TInput, TComparer>(this IEnumerable<TInput> enumerable, Func<TInput, TComparer> func, bool ascending)
        {
            return ascending ? enumerable.OrderBy(func) : enumerable.OrderByDescending(func);
        }

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
        /// Gives a convenient way of testing for the presence of a value
        /// </summary>
        public static bool Any<T>(this IEnumerable<T> enumerable, T value)
        {
            return enumerable.Any(i => i.Equals(value));
        }

        /// <summary>
        /// Equivalent to string.IsNullOrEmpty("") true if null or has no elements
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            return enumerable == null || !enumerable.Any();
        }

        /// <summary>
        /// Covert an enumerable so it can be used in a using statement <see cref="DisposableEnumerator{T}"/>
        /// </summary>
        public static DisposableEnumerator<T> AsDisposable<T>(this IEnumerable<T> enumerable) where T : IDisposable
        {
            return new DisposableEnumerator<T>(enumerable);
        }
    }
}