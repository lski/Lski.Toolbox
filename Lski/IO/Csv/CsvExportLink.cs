using Lski.Txt.Conversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lski.IO.Csv {
	
	/// <summary>
	/// A link for exporting a property to a position in the CSV File and he method for exporting
	/// </summary>
	public class CsvExportLink : CsvLink {

		/// <summary>
		/// An override for the conversion process (Optional)
		/// </summary>
		public ConvertTo Conversion { get; set; }
	}
}
