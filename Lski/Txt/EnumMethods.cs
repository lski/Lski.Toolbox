using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Reflection;

namespace Lski.Txt {

	/// <summary>
	/// This provides an extension method for enumerations so they can have a string value associated with the value too.
	/// </summary>
	public static class EnumMethods {

		/// <summary>
		/// Caching to prevent recalling
		/// </summary>
		private static Dictionary<Enum, string> _stringValues = new Dictionary<Enum, string>();
		private static Dictionary<Enum, object> _objects = new Dictionary<Enum, object>();

		/// <summary>
		/// Returns the value
		/// </summary>
		/// <param name="e"></param>
		/// <returns></returns>
		public static String GetDescription(this Enum e) {

			string output = null;

			// Check first in our cached results...
			if (_stringValues.ContainsKey(e)) {
				output = _stringValues[e];

			} else {
				//Look for our 'DescriptionAttribute' in the field's custom attributes
				Type type = e.GetType();

				FieldInfo fi = type.GetField(e.ToString());
				DescriptionAttribute[] attrs = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

				output = attrs.Length > 0 ? attrs[0].Value : e.ToString();

				// Add to cache
				_stringValues.Add(e, output);

			}

			return output;

		}

		public static object GetObject(this Enum e) {

			object output = null;
			Type type = e.GetType();

			// Check first in our cached results...

			if (_objects.ContainsKey(e)) {
				output = (_objects[e] as EnumObjectAttribute).Value;

			} else {
				//Look for our 'ObjectAttribute' in the field's custom attributes

				FieldInfo fi = type.GetField(e.ToString());
				EnumObjectAttribute[] attrs = fi.GetCustomAttributes(typeof(EnumObjectAttribute), false) as EnumObjectAttribute[];

				if (attrs.Length > 0) {
					// Ensure the value and enum are added to the static cache
					_objects.Add(e, attrs[0]);
					// Set the value to be returned
					output = attrs[0].Value;
				}
			}

			return output;
		}

		/// <summary>
		/// Used with enumerations containing the flags attribute, to see if a variable of that type contains a particular value... (saves having to remember
		/// the format for checking!)
		/// </summary>
		/// <param name="value">The enumerated value to check for</param>
		/// <returns>True if found, false if not</returns>
		public static bool Has<T>(this Enum e, T value) {

			try {
				return (System.Convert.ToInt64(e) & System.Convert.ToInt64(value)) == System.Convert.ToInt64(value);
			} catch {
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
			} catch {
				return false;
			}
		}

	}

	/// <summary>
	/// This allows a string to be added to an enumeration value for display
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class DescriptionAttribute : Attribute {

		#region "Properties"

		private String _value;
		/// <summary>
		/// Holds the stringvalue for a value in an enum.
		/// </summary>
		public String Value {
			get { return _value; }
			set { _value = value; }
		}


		#endregion

		#region "Constructor"

		/// <summary>
		/// Constructor used to init a StringValue Attribute
		/// </summary>
		/// <param name="value"></param>
		public DescriptionAttribute(string value) {
			this._value = value;
		}

		#endregion

	}

	/// <summary>
	/// This allows an additional property to be added to an enumeration
	/// </summary>
	public class EnumObjectAttribute : Attribute {

		#region "Properties"

		private object _value;
		/// <summary>
		/// Holds the stringvalue for a value in an enum.
		/// </summary>
		public object Value {
			get { return _value; }
			set { _value = value; }
		}


		#endregion

		#region "Constructor"

		/// <summary>
		/// Constructor used to init a StringValue Attribute
		/// </summary>
		/// <param name="value"></param>
		public EnumObjectAttribute(object value) {
			this._value = value;
		}

		#endregion

	}


}