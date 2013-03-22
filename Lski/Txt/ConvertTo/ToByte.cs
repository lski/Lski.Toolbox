using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lski.Txt.Conversion {

	public class ToByte : ConvertTo {

		public override object Parse(string value) {

			if (value.Length == 0) {
				return null;
			}

			byte b;
			if(byte.TryParse(value, out b)) {
				return b;
			}

			return null;
		}

		public override object Clone() {
			return new ToByte();
		}
	}
}
