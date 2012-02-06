using Microsoft.VisualBasic;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Serialization;

namespace Lski.Txt.ConvertTo {

	/// <summary>
	/// States the data type of an incoming value and provides a method of parsing the imcoming value into that datatype.
	/// </summary>
	/// <remarks>States the data type of an incoming value and provides a method of parsing the imcoming value into that datatype.
	/// 
	/// If the value can not be parsed for that particular type then it returns null.
	/// </remarks>
	[KnownType("GetKnownTypes")]
	[DataContract()]
	public abstract class ConvertTo : ICloneable {
		
		public abstract string Desc { get; }

		/// <summary>
		/// States the system type this object would map to in a data table
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		public abstract System.Type Type { get; }

		public abstract object Parse(string value);

		public abstract object Clone();

		public static ConvertTo GetDataMapType(Type type) {

			TypeCode tc = Type.GetTypeCode(type);
			
			switch (tc) {
				case TypeCode.DateTime:
					return new ToDate();
				case TypeCode.Decimal:
				case TypeCode.Double:
				case TypeCode.Int64:
				case TypeCode.Single:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
					return new ToDecimal();
				case TypeCode.Byte:
				case TypeCode.UInt16:
				case TypeCode.SByte:
				case TypeCode.Int16:
				case TypeCode.Int32:
					return new ToInteger();
				default:
					return new ToText();
			}

		}

		public static IEnumerable<Type> GetKnownTypes() {

			return new Type[] {typeof(ToText), 
							   typeof(ToDate),
							   typeof(ToDateDMY),
							   typeof(ToDateMDY),
							   typeof(ToDateYMD),
							   typeof(ToDecimal),
							   typeof(ToBool),
							   typeof(ToInteger)};
		}

		/// <summary>
		/// Simply returns a list of each of the supported DataMapTypes
		/// </summary>
		/// <returns></returns>
		/// <remarks></remarks>
		public static List<ConvertTo> GetDataTypeOptionsList() {

			List<ConvertTo> l = new List<ConvertTo>();

			var qry = (from x in GetKnownTypes() orderby x.Name select x);

			foreach (var i in qry) {
				l.Add((ConvertTo)Activator.CreateInstance(i));
			}

			return l;
		}

	}

}
