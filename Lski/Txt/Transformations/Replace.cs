using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Lski.Txt;

namespace Lski.Txt.Transformations {

	/// <summary>
	/// Replaces a regular expression match with a replacement string
	/// </summary>
	/// <remarks>
	/// Uses regular expressions to 
	/// </remarks>
	public class Replace : Transformation {

		public string Pattern { get; set; }
		public string Replacement { get; set; }

		public override string Process(string value) {
			return Regex.Replace(value, Pattern, Replacement);
		}

		public override object Clone() {	
			return new Replace() {
				Pattern = this.Pattern 
			}; 
		}
	}
}
