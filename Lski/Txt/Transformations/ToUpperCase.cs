using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lski.Txt.Transformations {

	public class ToUpperCase : Transformation {

		public override string ShortDesc { get { return "Upper Cased"; } }
		public override string FullDesc { get { return "Changes the value so that it displays in upper case"; } }

		public override string Process(string value) {

			if (string.IsNullOrEmpty(value)) return value;
			return value.ToUpperInvariant();

		}

		public override object Clone() { return new ToUpperCase { Formatting = this.Formatting }; }
	}
}
