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
	/// Trys to parse a date in the same method as DateTime in .Net
	/// </summary>
	/// <remarks></remarks>
	public class ToDateTime : BaseDataMapDate {

		public override string[] Formats { 
			get { 
				return null; 
			}
		}

		public override object Parse(string value) {

			// If a date, but the there is no text in that position in the line set the date to DBNull.Value
			if (value.Length == 0) {
				return null;
			}

			DateTime dat = default(DateTime);

			// If the date can be parsed, then add it otherwise, set it to null
			if (DateTime.TryParse(value, out dat)) {
				return dat;
			}

			return null;
		}

		public override object Clone() { 
			return new ToDateTime(); 
		}
	}
}
