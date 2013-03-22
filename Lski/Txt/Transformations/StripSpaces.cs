using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lski.Txt;

namespace Lski.Txt.Transformations {

	/// <summary>
	/// Removes all spaces from the selected value
	/// </summary>
	public class StripSpaces : Transformation {

		public override string Process(string value) {
			
			return (value == null ? null : value.Strip(' ').ToString());
		}

		public override object Clone() {
			return new StripSpaces();
		}
	}
}
