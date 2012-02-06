using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lski.Txt;

namespace Lski.Txt.Transformations {

	/// <summary>
	/// Alias of 'Left'
	/// </summary>
	/// <remarks></remarks>
	public class MaxLength : Left {
		public override object Clone() { return new MaxLength() { _intValue = this._intValue}; }
		public override string ShortDesc { get { return "MaxLength"; } }
	}
}