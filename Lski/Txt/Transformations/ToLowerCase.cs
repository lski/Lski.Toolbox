using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lski.Txt.Transformations {

	public class ToLowerCase : Transformation {

		public override string ShortDesc { get { return "Lower Cased"; }}
		public override string FullDesc { get { return "Changes the value so that it displays in lower case"; } }

		public override string Process(string value) {

			if (string.IsNullOrEmpty(value)) return value;

			return value.ToLowerInvariant();
		}

		public override object Clone() { return new ToLowerCase(); }
	}
}
