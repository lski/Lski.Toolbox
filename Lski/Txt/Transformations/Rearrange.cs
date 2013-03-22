using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lski.Txt;

namespace Lski.Txt.Transformations {

	/// <summary>
	/// The text is rearranged in an order specified by the user
	/// </summary>
	/// <remarks>
	/// Rearranges the passed string into the format passed, in the order desired:
	/// e.g str = "Hello" -> format = "2,5,3,4,1" -> result = "eollH"
	/// 
	/// The format string is not restricted to returning the same size string: 
	/// e.g str = "Helo" -> format = "1,2,3,3,4" -> result = "Hello"
	/// 
	/// Note: if a position is passed that is not in the string passed it is ignored
	/// e.g str = "Hello" -> format = "1,2,28,3,4,5" -> result = "Hello"
	/// </remarks>
	public class Rearrange : Transformation {

		public IEnumerable<int> Positions { get; set; }

		public override string Process(string value) {

			if (Positions == null || !Positions.Any()) {
				return value;
			}

			// Create a stringbuilder obj to help develop the new string
			var sb = new StringBuilder();

			foreach (var pos in Positions) {
				
				if(value.Length > pos)
					sb.Append(value[pos]);
			}

			return sb.ToString();
		}

		public override object Clone() { 
			return new Rearrange() { 
				Positions = this.Positions 
			}; 
		}
	}
}
