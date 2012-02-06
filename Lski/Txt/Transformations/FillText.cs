using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lski.Txt.Transformations {

	public class FillText : Transformation {

		public override string ShortDesc {
			get { return "Fill With Text"; }
		}

		public override string FullDesc {
			get { return "Fills the selected target with the text supplied"; }
		}

		public override string Process(string value) {
			return this.Formatting;
		}

		public override object Clone() {
			return new FillText() { Formatting = this.Formatting };
		}

	}
}
