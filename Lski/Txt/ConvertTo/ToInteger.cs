using Microsoft.VisualBasic;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Xml.Serialization;

namespace Lski.Txt.ConvertTo {

	/// <summary>
	/// Trys to parse any numeric value and stores it as a decimal
	/// </summary>
	/// <remarks></remarks>
	public class ToInteger : ConvertTo {

		[XmlIgnore()]
		public override string Desc { 
			get { return "Integer"; } 
		}

		public override System.Type Type { 
			get { return typeof(Int32); } 
		}

		public override object Parse(string value) {

			// If a integer, but the there is no text in that position in the line
			// Set the date to DBNull.Value
			if (value.Length == 0) return null; // DBNull.Value

			Int32 num = default(Int32);
			Boolean result = Int32.TryParse(value, out num);

			// If the number can be parsed, then add it otherwise, set it to null
			if (result)
				return num;
			else
				return null; // DBNull.Value
		}

		public override object Clone() { return new ToInteger(); }
	}

}