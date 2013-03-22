using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Text;
using System.Runtime.Serialization;
using System.Globalization;
using System.Reflection;

namespace Lski.Txt.Transformations {

	/// <summary>
	/// The super class to each of the alter value classes. When 
	/// </summary>
	/// <remarks></remarks>
	public abstract class Transformation : ICloneable {

		/// <summary>
		/// Converts the passed value using the Formatting supplied before returning it.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public abstract string Process(string value);
		public abstract object Clone();

		private static IEnumerable<Type> _knownTypes;
		/// <summary>
		/// Enables the data contract serialiser to known what possible subclasses are available
		/// </summary>
		/// <returns></returns>
		public static IEnumerable<Type> GetKnownTypes() {

			return _knownTypes ?? (_knownTypes = (from a in AppDomain.CurrentDomain.GetAssemblies() from t in a.GetTypes() where t.IsAssignableFrom(typeof(Transformation)) select t));
		}

	}
}
