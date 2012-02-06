using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lski.Txt.Transformations {

	public class Trim : CharBasedTransformations {

		public override string ShortDesc { get { return "Trim Both"; } }
		public override string FullDesc { get { return "All the spaces to the right and left of the text are removed"; } }

		public override string Process(string value) { return value.Trim(_charList); }
		public override object Clone() { return new Trim { _charList = this._charList }; }
	}

}
