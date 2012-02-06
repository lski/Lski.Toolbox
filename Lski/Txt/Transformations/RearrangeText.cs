using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lski.Txt;

namespace Lski.Txt.Transformations {

	/// <summary>
	/// Rearranges the passed string into the format passed
	/// </summary>
	/// <remarks>
	/// Rearranges the passed string into the format passed.
	/// 
	/// Format is in the style of a comma delimited string, containing position numbers:
	/// e.g str = "Hello" -> format = "2,5,3,4,1" -> result = "eollH"
	/// 
	/// The format string is not restricted to returning the same size string: 
	/// e.g str = "Helo" -> format = "1,2,3,3,4" -> result = "Hello"
	/// 
	/// Note: if a position is passed that is not in the string passed it is ignored
	/// e.g str = "Hello" -> format = "1,2,28,3,4,5" -> result = "Hello"
	/// </remarks>
	public class RearrangeText : Transformation {

		public override string ShortDesc { get { return "Rearrange Text"; } }

		public override string FullDesc { get { return "The text is rearranged in an order specified by the user"; } }

		public override string Process(string value) {

			// Create a stringbuilder obj to help develop the new string
			StringBuilder sb = null;

			// Create the int Array to hold the formatting passed
			List<int> positions = new List<int>();

			// A static char array holding each so called 'integer' in the format display, used so not recalled
			// Throughout the loop
			String[] formatArray = this.Formatting.SplitAdv(",", StringSplitOptions.RemoveEmptyEntries);

			// Loop through and ensure the 
			foreach (var c in formatArray) {
				
				Int32 intResult = default(Int32);
				
				// If it is an integer and its small enough that it points to a position inside the
				// The string passed, then store it
				if (Int32.TryParse(c, out intResult) && (intResult < value.Length) && (intResult > -1)) positions.Add(intResult);
			}

			sb = new StringBuilder(positions.Count - 1);

			// Now a workable format list has been created use it to rearrange a string from the passed string
			// Add the chars in the same order as originally held in format, as thats the way the user wants it
			foreach (var pos in positions) {	
				sb.Append(value[pos]);
			}

			return sb.ToString();
		}

		public override object Clone() { return new RearrangeText() { Formatting = this.Formatting }; }
	}
}
