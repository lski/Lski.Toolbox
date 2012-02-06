using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lski.Txt;

namespace Lski.Txt.Transformations {

	/// <summary>
	/// Changes the original passed string by replacing the text using the format provided E.g.
	/// string = "hello" -> format = "0^^1^^H" -> result = "Hello"
	/// </summary>
	/// <remarks>
	/// Changes the original passed string by replacing the text using the format provided
	/// 
	/// Accepts the format using a double semicolon separtor e.g.
	/// string = "hello" -> format = "0^^1^^H" -> result = "Hello"
	/// 
	/// 1st parameter: indicated is the first position in the string to replace, the 
	/// 2nd parameter: indicates the length to cut out, 
	/// 3rd parameter: is the string to replace it with
	/// 
	/// It is possible to provide more than one replace, by adding a pair of colons and the other replace
	/// IMPORTANT: Each replace is done sequentially, therefore the second replace will be working on
	/// the string as it is manipulated by the first string and NOT the original string e.g.
	/// string = "Hello World" -> format = "3^^2^^ABC::4^^2^^XYZ" -> result = "HelAXYZ World"
	/// 
	/// Note: if the start position is outside of the string passed to it, it is ignored, however if the
	/// length pushes past the end of the string, the string changed, will include upto the end of the string
	/// e.g. "hello" -> "3^^6^^World" -> "helWorld"
	/// </remarks>
	public class InsertText : Transformation {

		public override string ShortDesc {
			get { return "Insert Text"; }
		}

		public override string FullDesc {
			get { return "New text is added to the existing text, as specified by the user"; }
		}

		public override string Process(string value) {

			Int32 start = default(Int32);
			Int32 length = default(Int32);
			string newText = null;
			string[] @params = null;
			String[] exchanges = this.Formatting.SplitAdv("::", StringSplitOptions.RemoveEmptyEntries);

			// Loop through each of the text exchanges

			foreach (var e in exchanges) {
				
				@params = e.SplitAdv("^^", StringSplitOptions.None);

				// If there are not the correct amount of parts return Nothing because there was an error
				if (@params.Length != 3)
					throw new Exception("There was not enough information provided to insert the selected text " + this.Formatting);

				// Fill the data holder
				Int32.TryParse(@params[0], out start);
				Int32.TryParse(@params[1], out length);
				newText = @params[2];

				// If the start position is greater (or equal) to the length of the string
				// then return the original string as there is nothing to change DO NOT THROW Error
				if ((start >= value.Length)) continue;

				value = value.SubStringAdv(0, start) + newText + value.SubStringAdv(start + newText.Length);
			}

			return value;
		}

		public override object Clone() {
			return new InsertText() { Formatting = this.Formatting };
		}

	}
}
