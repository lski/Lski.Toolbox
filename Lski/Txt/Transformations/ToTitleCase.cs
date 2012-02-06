using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Lski.Txt.Transformations {

	public class ToTitleCase : Transformation {

		public override string ShortDesc { get { return "Title Cased"; } }
		public override string FullDesc { get { return "Changes the value so that it displays in title case"; } }

		public override string Process(string value) {

			if (string.IsNullOrEmpty(value)) return value;
			return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(value.ToLowerInvariant());

		}

		public override object Clone() { return new ToTitleCase { Formatting = this.Formatting }; }
	}
}
