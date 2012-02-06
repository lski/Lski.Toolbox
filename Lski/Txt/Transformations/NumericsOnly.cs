using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Lski.Txt.Transformations {

	public class NumericsOnly : Transformation {

		public override string ShortDesc { get { return "Numerics Only"; } }
		public override string FullDesc { get { return "Only returns numeric values in the string"; } }

		public override string Process(string value) { return Regex.Replace(value, "[^\\d]", ""); }
		public override object Clone() { return new NumericsOnly { Formatting = this.Formatting };}

	}
}
