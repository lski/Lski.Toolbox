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
	/// Trys to parse any numeric value and stores it as a decimal
	/// </summary>
	/// <remarks></remarks>
	public class ToSingle : ConvertTo {

		public override object Parse(string value) {

			if (value.Length == 0) {
				return null;
			}

			Single num;
			// If the number can be parsed, then add it otherwise, set it to null
			if (Single.TryParse(value, out num)) {
				return num;
			}

			return null;
		}

		public override object Clone() {
			return new ToSingle(); 
		}
	}

}
