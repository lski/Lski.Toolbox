using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lski.Txt.Transformations {

	/// <summary>
	/// All the spaces to the left of the text are removed
	/// </summary>
	public class TrimStart : Transformation {

		public char Char { get; set; }

		public override string Process(string value) {
			return value.TrimStart(Char); 
		}

		public override object Clone() {
			
			return new TrimStart() {
				Char = this.Char 
			};
		}
	}
}
