using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lski.Txt.ConvertTo;
using Lski.Txt.Transformations;

namespace Lski.IO.Csv {
	
	public abstract class CsvDataMap {

		/// <summary>
		/// Line position within the Csv line
		/// </summary>
		public int Position { get; set; }

		/// <summary>
		/// The property to map the value too.
		/// </summary>
		public string Property { get; set; }

	}
}
