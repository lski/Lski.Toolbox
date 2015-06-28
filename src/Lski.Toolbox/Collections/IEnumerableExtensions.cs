using System.Collections.Generic;
using System.Linq;

namespace Lski.Toolbox.Collections {

    public static class IEnumerableExtensions {

        /// <summary>
        /// Adds the ability to ask for the to call Max, without exception if the enumerable is empty. On empty the default value of T is returned
        /// </summary>
        public static T MaxOrDefault<T>(this IEnumerable<T> e) {

            foreach (var i in e) {

                //If reaches here its gone to the first item so contains items, so simply call max
                return e.Max();
            }

            return default(T);
        }

        /// <summary>
        /// Adds the ability to ask for the to call Min, without exception if the enumerable is empty. On empty the default value of T is returned
        /// </summary>
        public static T MinOrDefault<T>(this IEnumerable<T> e) {

            foreach (var i in e) {

                //If reaches here its gone to the first item so contains items, so simply call min
                return e.Min();
            }

            return default(T);
        }
    }
}