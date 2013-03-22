using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lski.Txt.Transformations {

	/// <summary>
	/// Fills the selected target with the text supplied
	/// </summary>
	public class DefaultValue : Transformation {

		public string Value { get; set; }

		public override string Process(string value) {
			return this.Value;
		}

		public override object Clone() {
			
			return new DefaultValue() {
				Value = this.Value 
			};
		}

	}
}
