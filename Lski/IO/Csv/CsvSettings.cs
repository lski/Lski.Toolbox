using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lski.IO.Csv {
	
	public class CsvSettings {

		public const string DefaultDelimiter = ",";
		public const string DefaultTextDelimiter = "\"";

		/// <summary>
		/// States whether the CSV file has a header
		/// </summary>
		public virtual bool Header { get; set; }
		/// <summary>
		/// The value delimiter, to mark to split between different values, usually a single comma ','
		/// </summary>
		public virtual string Delimiter { get; set; }
		/// <summary>
		/// The text delimiter, to be placed around a text value when a value delimiter is placed inside the text and needs to avoid being confused as a value delimiter, normal double quote '"'
		/// </summary>
		public virtual string TextDelimiter { get; set; }

		private string _NULL;
		/// <summary>
		/// The string equiv of null, can not be null as it represents its value as a string the CSV File, e.g. "" or "NULL"
		/// </summary>
		public string NULL {
			get { return _NULL; }
			set { _NULL = (value ?? ""); }
		}

		public CsvSettings() {
			NULL = "";
			Header = false;
			Delimiter = DefaultDelimiter;
			TextDelimiter = DefaultTextDelimiter;
		}
	}
}
