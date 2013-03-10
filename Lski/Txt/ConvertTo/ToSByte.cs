using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lski.Txt.ConvertTo {

	public class ToSByte : ConvertTo {

		public override Type Type {
			get {  return typeof(SByte); }
		}

		public override object Parse(string value) {

			if (value.Length == 0) {
				return null;
			}

			sbyte b;
			if(sbyte.TryParse(value, out b)) {
				return b;
			}

			return null;
		}

		public override object Clone() {
			return new ToSByte();
		}
	}
}
