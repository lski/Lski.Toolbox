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
	/// Represents each type of string objects are being imported, this is the default type
	/// </summary>
	/// <remarks></remarks>
	public class ToText : ConvertTo {

		public override object Clone() { 
			return new ToText();
		}

		public override object Parse(string value) { 
			return value; 
		}
	}
}
