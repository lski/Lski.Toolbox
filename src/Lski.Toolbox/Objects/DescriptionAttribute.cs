using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lski.Toolbox.Objects {

	/// <summary>
	/// This allows a string to be added to an enumeration value for display
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class DescriptionAttribute : Attribute {

		/// <summary>
		/// Holds the stringvalue for a value in an enum.
		/// </summary>
		public String Value { get; set; }

		/// <summary>
		/// Constructor used to init a StringValue Attribute
		/// </summary>
		/// <param name="value"></param>
		public DescriptionAttribute(string value) {
			Value = value;
		}

	}

	public static class DecriptionAttributeExtension {

		/// <summary>
		/// Caching to prevent recalling
		/// </summary>
		private static Dictionary<Enum, string> _stringValues = new Dictionary<Enum, string>();

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

			}
			else {
				//Look for our 'DescriptionAttribute' in the field's custom attributes
				Type type = e.GetType();

				var fi = type.GetField(e.ToString());
				var attrs = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

				output = attrs.Length > 0 ? attrs[0].Value : e.ToString();

				// Add to cache
				_stringValues.Add(e, output);

			}

			return output;

		}
	}
}
