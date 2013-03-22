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
	public class ToUInt16 : ConvertTo {

		public override object Parse(string value) {

			// If a integer, but the there is no text in that position in the line
			if (value.Length == 0) {
				return null;
			}

			UInt16 num;
			if (UInt16.TryParse(value, out num)) {
				return num;
			}
			
			return null;
		}

		public override object Clone() {
			return new ToUInt16(); 
		}
	}

}