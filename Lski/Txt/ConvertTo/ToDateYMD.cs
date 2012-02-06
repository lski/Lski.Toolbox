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
	/// Trys to parse a date that is specified as coming in using format of year first (E.g. database format)
	/// </summary>
	/// <remarks></remarks>
	public class ToDateYMD : BaseDataMapDate {

		[XmlIgnore()]
		public override string Desc { get { return "Date (YMD)"; } }

		public override string[] Formats {
			get {
				_formats = _formats ?? new string[] {
										"yyMMdd",
										"yyyyMMdddd",
										"yy/MM/dd",
										"yyyy/MM/dd",
										"yy-MM-dd",
										"yyyy-MM-dd",
										"yy.MM.dd",
										"yyyy.MM.dd",
										"yyyy\\MM\\dd",
										"yy\\MM\\dd",
										"yyyy:MM:dd",
										"yy:MM:dd"
									};

				return _formats;
			}
		}

		public override object Clone() { return new ToDateYMD(); }
	}

}
