using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lski.Txt;

namespace Lski.Txt.Transformations {

	public class StripChars : CharBasedTransformations {

		public override string ShortDesc { get { return "Remove Characters"; } }
		public override string FullDesc { get { return "Removes all characters stated in formatting from the selected value"; } }

		public override string Process(string value) { return value.Strip(_charList).ToString(); }
		public override object Clone() { return new StripChars { _charList = this._charList }; }

	}
}


