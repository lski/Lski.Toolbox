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

namespace Lski.Txt.Transformations {

	/// <summary>
	/// The super class to each of the alter value classes. When 
	/// </summary>
	/// <remarks></remarks>
	[DataContract()]
	[KnownType("GetKnownTypes")]
	public abstract class Transformation : ICloneable {

		// Hidden for serialisation
		protected Transformation() { 
			this._formatting = ""; 
		}
		
		public Transformation(string formatting) { 
			this._formatting = formatting;
		}

		public abstract string ShortDesc { get; }

		public abstract string FullDesc { get; }

		protected string _formatting;
		[DataMember()]
		public virtual string Formatting {
			get { return _formatting; }
			set { _formatting = value; }
		}

		/// <summary>
		/// Converts the passed value using the Formatting supplied before returning it.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public abstract string Process(string value);
		public abstract object Clone();

		public static IEnumerable<Type> GetKnownTypes() {

			return new Type[] {typeof(Left), typeof(Right), typeof(MaxLength), typeof(StripChars), typeof(Trim), typeof(TrimStart), typeof(TrimEnd), typeof(ReplaceText), typeof(RearrangeText), typeof(InsertText), typeof(FillText), typeof(ToTitleCase), typeof(ToUpperCase), typeof(ToLowerCase), typeof(NumericsOnly), typeof(StripSpaces)};
		}

		/// <summary>
		/// Simply returns a list of each of the supported DataMapTypes
		/// </summary>
		/// <returns></returns>
		/// <remarks></remarks>
		public static List<Transformation> GetTransformationOptionsList() {

			List<Transformation> l = new List<Transformation>();

			var qry = (from x in GetKnownTypes() orderby x.Name select x);

			foreach (var i in qry) {
				l.Add((Transformation)Activator.CreateInstance(i));
			}

			return l;
		}

	}
}
