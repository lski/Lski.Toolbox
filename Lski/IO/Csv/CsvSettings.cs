using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lski.IO.Csv {
	
	public class CsvSettings {

		public const string DefaultDelimiter = ",";
		public const string DefaultTextDelimiter = "\"";

		public virtual bool Header { get; set; }
		public virtual string Delimiter { get; set; }
		public virtual string TextDelimiter { get; set; }

		public CsvSettings() {
			Header = false;
			Delimiter = DefaultDelimiter;
			TextDelimiter = DefaultTextDelimiter;
		}
	}
}
