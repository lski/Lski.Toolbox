using Lski.Txt.Conversion;
using Lski.Txt.Transformations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Lski.IO.Csv {
	
	public class InternalCsvLink {

		/// <summary>
		/// Line position within the Csv line
		/// </summary>
		public int Position { get; set; }

		/// <summary>
		/// The property to map the value too.
		/// </summary>
		public PropertyInfo Property { get; set; }

		/// <summary>
		/// An override for the conversion process, default used if null
		/// </summary>
		public ConvertTo Conversion { get; set; }

		/// <summary>
		/// A list of tranformations to run on the data prior to conversion
		/// </summary>
		public Transformations Tranformations { get; set; }

		
	}
}
