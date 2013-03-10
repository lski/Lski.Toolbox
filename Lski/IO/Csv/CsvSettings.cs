using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lski.IO.Csv {
	
	public class CsvSettings {

		public const string DefaultDelimiter = ",";
		public const string DefaultTextDelimiter = "\"";

		public bool HasHeader { get; set; }
		public string Delimiter { get; set; }
		public string TextDelimiter { get; set; }

		public CsvSettings() {
			HasHeader = false;
			Delimiter = DefaultDelimiter;
			TextDelimiter = DefaultTextDelimiter;
		}
	}
}
