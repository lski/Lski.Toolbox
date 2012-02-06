using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lski.Txt;

namespace Lski.Txt.Transformations {

	/// <summary>
	/// A super class designed to handle all the formatting values, when the underliying format is a charlist
	/// </summary>
	/// <remarks></remarks>
	public abstract class IntBasedTransformations : Transformation {


		protected Int32 _intValue = 0;
		/// <summary>
		/// The formatting of this AlterValue type
		/// </summary>
		/// <exception cref="InvalidCastException">If the value being set does not parse as an Int</exception>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		public override string Formatting {

			get { return StringExt.ConvertToString(_intValue); }
			set {
				Int32 tmp = 0;
				// If has problem, throw a bespoke error message
				if (!Int32.TryParse(value, out tmp))
					throw new InvalidCastException("Could not set the formatting of the " + this.GetType().Name + " object as the value needs to be an Int but the value was: [" + value + "]");

				_intValue = tmp;
			}
		}

	}
}
