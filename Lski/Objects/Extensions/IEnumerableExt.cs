using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lski.Objects.Extensions {
	
	public static class IEnumerableExt {

		/// <summary>
		/// Adds the ability to ask for the to call Max, without exception if the enumerable is empty. On empty the default value of T is returned
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="e"></param>
		/// <returns></returns>
		public static T MaxOrDefault<T>(this IEnumerable<T> e) {

			T result = default(T);

			foreach (var item in e) {
				
				//If reaches here its gone to the first item so contains items, so simply call max
				return e.Max();
			}

			return result;
		}

		/// <summary>
		/// Adds the ability to ask for the to call Min, without exception if the enumerable is empty. On empty the default value of T is returned
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="e"></param>
		/// <returns></returns>
		public static T MinOrDefault<T>(this IEnumerable<T> e) {

			T result = default(T);

			foreach (var item in e) {

				//If reaches here its gone to the first item so contains items, so simply call max
				return e.Min();
			}

			return result;
		}

	}
}
