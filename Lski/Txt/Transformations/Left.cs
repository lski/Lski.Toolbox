using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lski.Txt;

namespace Lski.Txt.Transformations {

	/// <summary>
	/// When processing the value passed in, it takes only the amount of characters specified from the left hand side (therefore simulating left
	/// in a database and a maximum length for a field)
	/// </summary>
	/// <remarks></remarks>
	public class Left : IntBasedTransformations {

		public override object Clone() { return new Left { _intValue = this._intValue }; }
		public override string FullDesc { get { return "Sets the maximum amount of characters to return starting from the left"; } }

		public override string Process(string value) {	return value.SubStringAdv(0, _intValue); }
		public override string ShortDesc { get { return "Left"; }}
	}
}
