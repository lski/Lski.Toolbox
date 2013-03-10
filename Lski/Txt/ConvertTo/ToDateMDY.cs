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
	/// Trys to parse a date that is specified as coming in using format of date second month first (E.g. en-us)
	/// </summary>
	/// <remarks></remarks>
	public class ToDateMDY : BaseDataMapDate {

		public override string[] Formats {
			get {
				_formats = _formats ?? new string[] {
						"MMddyy",
						"MMddyyyy",
						"MM/dd/yy",
						"MM/dd/yyyy",
						"MM-dd-yy",
						"MM-dd-yyyy",
						"MM.dd.yy",
						"MM.dd.yyyy",
						"MM\\dd\\yy",
						"MM\\dd\\yyyy",
						"MM:dd:yy",
						"MM:dd:yyyy",
						"Mddyy",
						"Mddyyyy"
					};

				return _formats;
			}
		}

		public override object Clone() { return new ToDateMDY(); }
	}
}
