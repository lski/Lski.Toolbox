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
	/// Trys to parse a date that is specified as coming in using format of date first month second (E.g. en-gb)
	/// </summary>
	/// <remarks></remarks>
	public class ToDateDMY : BaseDataMapDate {

		[XmlIgnore()]
		public override string Desc { get { return "Date (DMY)"; } }

		public override string[] Formats {
			get {
				if (_formats == null) {
					_formats = new string[] {
						"ddMMyy",
						"ddMMyyyy",
						"dd/MM/yy",
						"dd/MM/yyyy",
						"dd-MM-yy",
						"dd-MM-yyyy",
						"dd.MM.yy",
						"dd.MM.yyyy",
						"dd\\MM\\yy",
						"dd\\MM\\yyyy",
						"dd:MM:yy",
						"dd:MM:yyyy",
						"dMMyy",
						"dMMyyyy"
					};
				}

				return _formats;
			}
		}

		public override object Clone() { return new ToDateDMY(); }
	}
}
