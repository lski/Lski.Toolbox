using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lski.Toolbox.Txt {

	public static class StringFunctions {

		/// <summary>
		/// Converts a numeric to an Alpha e.g. 1 => A, 27 => AA etc
		/// </summary>
		public static string ConvertToAlpha(int columnNumber) {

			var dividend = columnNumber;
			var alpha = new StringBuilder();

			// Loop through the number winding it down by dividing it by 26 and using the mod operator to work the remainder
			// The remainder is then used to create an uppercase letter which is added to those
			while (dividend > 0) {

				var modulo = (dividend - 1) % 26;

				alpha.Insert(0, Convert.ToChar(65 + modulo));

				dividend = (dividend - modulo) / 26;
			}

			return alpha.ToString();
		}

		/// <summary>
		/// Accepts a list of strings and returns them joined together with a spearator. Ignores values that are null or empty strings
		/// and does not add a separator for that empty value.
		/// </summary>
		public static string ConcatWs(string separator, params Object[] objs) {
			return ConcatWs(objs, separator);
		}

		/// <summary>
		/// Accepts a list of strings and returns them joined together with a spearator. Ignores values that are null or empty strings
		/// and does not add a separator for that empty value.
		/// </summary>
		public static string ConcatWs(IEnumerable<string> objs, string separator) {

			return string.Join(separator, (from s in objs where !string.IsNullOrEmpty(s) select s));
		}

		/// <summary>
		/// Accepts a list of strings and returns them joined together with a spearator. Ignores values that are null or empty strings
		/// and does not add a separator for that empty value.
		/// </summary>
		/// <param name="objs">todo: describe objs parameter on ConcatWs</param>
		/// <param name="separator">todo: describe separator parameter on ConcatWs</param>
		public static string ConcatWs(IEnumerable<Object> objs, string separator) {

			return string.Join(separator, (from o in objs
										   where o != null
										   let s = o.ToString() // Convert it once only
										   where s.Length > 0
										   select s));
		}

		/// <summary>
		/// A version of trim for strings, that handles null string values as well as initialised strings.
		/// </summary>
		/// <param name="str">The string to trim</param>
		/// <param name="emptyStringOnNull">If true and the string is null, then an empty string is returned, otherwise null</param>
		/// <returns></returns>
		public static string Trim(string str, bool emptyStringOnNull = true) {

			if (str == null) {

				if (emptyStringOnNull) {
					return String.Empty;
				}

				return null;
			}

			return str.Trim();
		}
	}
}