using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Lski.Txt.Transformations {

	/// <summary>
	/// Changes the value so that it displays in title case
	/// </summary>
	public class ToTitleCase : Transformation {

		public override string Process(string value) {

			if (string.IsNullOrEmpty(value)) 
				return value;
			
			// Have to convert it to lower case first in case in Upper case then it wont change it
			return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(value.ToLowerInvariant());
		}

		public override object Clone() { 
			return new ToTitleCase(); 
		}
	}
}
