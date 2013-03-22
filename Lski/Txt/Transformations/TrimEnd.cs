using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lski.Txt.Transformations {

	/// <summary>
	/// All the spaces to the right of the text are removed
	/// </summary>
	public class TrimEnd : Transformation {

		public char Char { get; set; }

		public override string Process(string value) {
			return value.TrimEnd(Char);
		}

		public override object Clone() {

			return new TrimEnd {
				Char = this.Char
			};
		}

	}

}
