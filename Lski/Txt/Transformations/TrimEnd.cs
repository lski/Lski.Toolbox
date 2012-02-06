using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lski.Txt.Transformations {

	public class TrimEnd : CharBasedTransformations {

		public override string ShortDesc { get { return "Right Trim"; } }
		public override string FullDesc { get { return "All the spaces to the right of the text are removed"; } }
		
		public override string Process(string value) { return value.TrimEnd(_charList); }
		public override object Clone() { return new TrimEnd { _charList = this._charList }; }

	}

}
