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
	public class ToDouble : ConvertTo {

		public override System.Type Type { 
			get { return typeof(double); } 
		}

		public override object Parse(string value) {

			if (value.Length == 0) {
				return null;
			}

			double num;
			// If the number can be parsed, then add it otherwise, set it to null
			if (double.TryParse(value, out num)) {
				return num;
			}

			return null;
		}

		public override object Clone() { 
			return new ToDouble(); 
		}
	}

}
