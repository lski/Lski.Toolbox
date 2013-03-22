using Microsoft.VisualBasic;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Xml.Serialization;

namespace Lski.Txt.Conversion {

	/// <summary>
	/// Trys to parse any numeric value and stores it as a decimal
	/// </summary>
	/// <remarks></remarks>
	public class ToBool : ConvertTo {

		private static string[] _trueValues;
		private static string[] TrueValues {
			get {
				_trueValues = _trueValues ?? new string[] {
					"y",
					"yes",
					"t",
					"true"
				};
				return _trueValues;
			}
		}

		private static string[] _falseValues;
		private static string[] FalseValues {
			get {
				_falseValues = _falseValues ?? new string[] {
					"n",
					"no",
					"f",
					"false"
				};
				return _falseValues;
			}
		}

		public override string ToString(object obj) {
			return (obj == null ? (string)null : ((bool)obj ? "true" : "false"));
		}

		/// <summary>
		/// Converts a string value into a boolean, if the value can not be converted NULL is returned to allow the subscriber to give a default. True can be:
		/// y, yes, t, true, 1 and is not case sensitive. The opposites are valid as false values.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public override object Parse(string value) {

			if (string.IsNullOrEmpty(value)) {
				return null;
			}

			// Start by trying to see if this value is a number, if so convert to a number before parsing
			byte tmpNumber;

			// Now try and parse with the number converted
			if (byte.TryParse(value, out tmpNumber)) {
				return Convert.ToBoolean(tmpNumber);
			}

			// Make sure that case remove the
			String tmpValue = value.Trim().ToLowerInvariant();

			if (TrueValues.Contains(tmpValue)) {
				return true;
			}

			if (FalseValues.Contains(tmpValue)) {
				return false;
			}

			throw new ArgumentException(String.Format("The value '{0}' could not parsed to a boolean value", value));
		}

		public override object Clone() { 
			return new ToBool(); 
		}

		/// <summary>
		/// A static version used to Parse Booleans outside of the IO context
		/// </summary>
		/// <param name="value"></param>
		public static Boolean? ParseBoolean(string value) {

			return (new ToBool()).Parse(value) as Boolean?;
		}
	}

}
