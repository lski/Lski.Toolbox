using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lski.Txt;

namespace Lski.Txt.Transformations {

	/// <summary>
	/// Sets the maximum amount of characters to return starting from the left
	/// </summary>
	/// <remarks>
	/// When processing the value passed in, it takes only the amount of characters specified from the left hand side (therefore simulating left
	/// in a database and a maximum length for a field)
	/// </remarks>
	public class Left : Transformation {

		public int MaxAmount { get; set; }

		public override object Clone() {
			return new Left {
				MaxAmount = this.MaxAmount
			};
		}

		public override string Process(string value) {
			return value.SubStringAdv(0, MaxAmount);
		}
	}
}
