using Lski.Txt.Conversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lski.IO.Csv {
	
	public class CsvExportLink : CsvLink {

		/// <summary>
		/// An override for the conversion process, default used if null
		/// </summary>
		public ConvertTo Conversion { get; set; }
	}
}
