using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Reflection;

namespace Lski.Objects.Extensions {

	/// <summary>
	/// This provides an extension method for enumerations so they can have a string value associated with the value too.
	/// </summary>
	public static class EnumExt {

		/// <summary>
		/// Used with enumerations containing the flags attribute, to see if a variable of that type contains a particular value... (saves having to remember
		/// the format for checking!)
		/// </summary>
		/// <param name="value">The enumerated value to check for</param>
		/// <returns>True if found, false if not</returns>
		public static bool Has<T>(this Enum e, T value) {
			
			try {
				return (System.Convert.ToInt64(e) & System.Convert.ToInt64(value)) == System.Convert.ToInt64(value);
			}
			catch {
				return false;
			}
		}

		/// <summary>
		/// Used with enumerations containing the flags attribute, to see if the passed value is the only value in the enumeration variable.
		/// </summary>
		/// <param name="value"></param>
		/// <returns>True if found, false if not</returns>
		public static bool Is<T>(this Enum type, T value) {
			try {
				return (Int64)(object)type == (Int64)(object)value;
			}
			catch {
				return false;
			}
		}
	}

}