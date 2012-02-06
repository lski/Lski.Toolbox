using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lski.Txt.Transformations {

	/// <summary>
	/// A super class designed to handle all the formatting values, when the underliying format is a charlist
	/// </summary>
	/// <remarks></remarks>
	public abstract class CharBasedTransformations : Transformation {

		protected char[] _charList;
		public override string Formatting {
			get { return new string(_charList); }
			set { _charList = string.IsNullOrEmpty(value) ? null : value.ToCharArray(); }
		}

	}

}
