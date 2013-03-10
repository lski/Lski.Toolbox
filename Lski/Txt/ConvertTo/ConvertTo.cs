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
		
		/// <summary>
		/// States the system type this object would map to in a data table
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		public abstract System.Type Type { get; }

		public abstract object Parse(string value);

		public abstract object Clone();

		public static ConvertTo GetConverter(Type type) {

			// Handle potential nullable versions of scalar types
			TypeCode tc = (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) ? Type.GetTypeCode(type.GetGenericArguments()[0]) : Type.GetTypeCode(type));

			// Should change this to a dynamic loader
			switch (tc) {
				case TypeCode.DateTime:
					return new ToDateTime();
				case TypeCode.Decimal:
					return new ToDecimal();
				case TypeCode.Double:
					return new ToDouble();
				case TypeCode.Single:
					return new ToSingle();
				case TypeCode.Int16:
					return new ToInt16();
				case TypeCode.Int32:
					return new ToInt32();
				case TypeCode.Int64:
					return new ToInt64();
				case TypeCode.UInt16:
					return new ToUInt16();
				case TypeCode.UInt32:
					return new ToUInt32();
				case TypeCode.UInt64:
					return new ToUInt64();
				case TypeCode.Byte:
					return new ToByte();
				case TypeCode.SByte:
					return new ToSByte();				
				default:
					return new ToText();
			}

		}

		public static IEnumerable<Type> GetKnownTypes() {

			return new Type[] {typeof(ToText), 
							   typeof(ToDateTime),
							   typeof(ToDateDMY),
							   typeof(ToDateMDY),
							   typeof(ToDateYMD),
							   typeof(ToDecimal),
							   typeof(ToBool),
							   typeof(ToInt32)};
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
