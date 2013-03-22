using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lski.Txt.Transformations {

	/// <summary>
	/// All the spaces to the right and left of the text are removed
	/// </summary>
	public class Trim : Transformation {

		public char Char { get; set; }

		public override string Process(string value) {
			return value.Trim(Char);
		}

		public override object Clone() {

			return new Trim {
				Char = this.Char
			};
		}
	}

}
