using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lski.Txt;

namespace Lski.Txt.Transformations {

	/// <summary>
	/// Replaces any matching sections of strings, with a replacement string
	/// </summary>
	/// <remarks>Replaces any matching sections of strings, with a replacement string
	/// 
	/// Works like the stringObj.replace function, but accepts several values to replace as a separated 
	/// string. with a :: separating each replacement and the ^^ separating the old text from the new 
	/// text e.g.
	/// str = "**Hello**" -> format = "*^^+::l^^L" -> result = "++HeLLo++"
	/// </remarks>
	[Obsolete("Needs changing to a regular expression verison")]
	public class ReplaceText : Transformation {

		public override string ShortDesc { get { return "Replace Text"; } }
		public override string FullDesc { get { return "Certain sections of text are replaced by those specified by the user"; } }

		public override string Process(string value) {

			string[] @params = null;
			string[] exchanges = this.Formatting.SplitAdv("::", StringSplitOptions.RemoveEmptyEntries);

			// Loop through each of the text exchanges

			foreach (var e in exchanges) {

				@params = e.SplitAdv("^^", StringSplitOptions.None);

				// If there are not the correct amount of parts return Nothing because there was an error
				if (@params.Length != 2) 
					throw new Exception("There was not enough information provided to replace the selected characters " + this.Formatting);
				else if (@params[0].Length == 0) 
					throw new Exception("You can not replace an empty line of text within a string " + this.Formatting);
				
				value = value.Replace(@params[0], @params[1]);
			}

			return value;
		}

		public override object Clone() { return new ReplaceText() { Formatting = this.Formatting }; }
	}
}
