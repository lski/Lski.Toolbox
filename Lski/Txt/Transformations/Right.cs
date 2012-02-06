using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lski.Txt;

namespace Lski.Txt.Transformations {

	/// <summary>
	/// When processing the value passed in, it takes only the amount of characters specified from the left hand side (therefore simulating 
	/// 'Right' function in a database)
	/// </summary>
	/// <remarks></remarks>
	public class Right : IntBasedTransformations {

		public override object Clone() { return new Right { _intValue = this._intValue }; }
		public override string FullDesc { get { return "Sets the maximum amount of characters to return starting from the right, going to the left"; } }

		public override string Process(string value) { return value.SubStringAdv(value.Length - _intValue); }
		public override string ShortDesc { get { return "Right"; } }
	}
}
