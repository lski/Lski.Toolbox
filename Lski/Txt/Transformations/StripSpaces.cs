using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lski.Txt;

namespace Lski.Txt.Transformations {

	public class StripSpaces : Transformation {

		public override string ShortDesc { get { return "Remove Spaces"; } }
		public override string FullDesc { get { return "Removes all spaces from the selected value"; } }

		public override string Process(string value) { return value.Strip(' ').ToString(); }
		public override object Clone() { return new StripSpaces(); }
	}
}
