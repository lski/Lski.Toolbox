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
	/// Represents each type of string objects are being imported, this is the default type
	/// </summary>
	/// <remarks></remarks>
	public class ToText : ConvertTo {

		[XmlIgnore()]
		public override System.Type Type { get { return typeof(string); } }

		public override object Clone() { return new ToText();}

		public override object Parse(string value) { return value; }
	}
}
