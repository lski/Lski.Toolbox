using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lski.Txt.Transformations {

	public class TrimStart : CharBasedTransformations {

		public override string ShortDesc { 
			get { return "Left Trim"; } 
		}
		
		public override string FullDesc { 
			get { return "All the spaces to the left of the text are removed"; } 
		}
		
		public override string Process(string value) { 
			return value.TrimStart(_charList); 
		}

		public override object Clone() {
			return new TrimStart() { _charList = this._charList };
		}
	}
}
