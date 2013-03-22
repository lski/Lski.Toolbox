using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lski.Txt;

namespace Lski.Txt.Transformations {

	/// <summary>
	/// Removes all characters stated in formatting from the selected value
	/// </summary>
	public class StripChars : Transformation {

		public char[] Chars { get; set; }

		public override string Process(string value) {

			return (Chars == null || value == null ? value : value.Strip(Chars).ToString());
		}

		public override object Clone() {

			return new StripChars {
				Chars = this.Chars
			};
		}
	}
}


