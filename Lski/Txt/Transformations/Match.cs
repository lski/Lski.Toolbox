using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Lski.Txt.Transformations {
	
	/// <summary>
	/// Simply extracts the match made with the regular expression pattern
	/// </summary>
	public class Match : Transformation {

		public Match() {
			Pattern = "";
		}

		public Match(string pattern) {
			Pattern = pattern;
		}

		public string Pattern { get; set; }

		public override string Process(string value) {

			var match = Regex.Match(value, Pattern);

			if (!match.Success)
				return "";

			return match.Groups[0].Value;
		}

		public override object Clone() {
			return new Match() {
				Pattern = this.Pattern
			};
		}
	}
}
