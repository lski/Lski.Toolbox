using Lski.Txt.ConvertTo;
using Lski.Txt.Transformations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lski.IO.Csv {

	public class CsvImportLink : CsvDataMapLink {

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
