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
	/// Works asa a base type for dates, not to be used itself, but inherited from
	/// </summary>
	/// <remarks></remarks>
	public abstract class BaseDataMapDate : ConvertTo {

		public override System.Type Type { get { return typeof(DateTime); } }

		protected string[] _formats;
		public abstract string[] Formats { get; }

		public override object Parse(string value) {

			// If a date, but the there is no text in that position in the line
			// Set the date to DBNull.Value
			if (value.Length == 0) return DBNull.Value;

			DateTime dat = default(DateTime);

			// If the date can be parsed, then add it otherwise, set it to null
			if (DateTime.TryParseExact(value, Formats, CultureInfo.InvariantCulture, DateTimeStyles.AllowInnerWhite, out dat)) {
				return dat;
			}

			return DBNull.Value;
		}

	}
}
