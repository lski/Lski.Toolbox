using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Lski.IO.Csv {
	
	internal class InternalCsvImportMap<T> {

		public const string DefaultDelimiter = ",";
		public const string DefaultTextDelimiter = "\"";
		public const Int32 DefaultAlertAmount = 10;
		public const bool DefaultEmptyValueAsNull = true;

		public bool HasHeader { get; set; }
		public string Delimiter { get; set; }
		public string TextDelimiter { get; set; }
		public int AlertAmount { get; set; }
		public bool EmptyValueAsNull { get; set; }
		public ICollection<CsvDataMapLink> Links { get; set; }

		public InternalCsvImportMap() {
			HasHeader = false;
			Delimiter = DefaultDelimiter;
			TextDelimiter = DefaultTextDelimiter;
			AlertAmount = DefaultAlertAmount;
			EmptyValueAsNull = DefaultEmptyValueAsNull;
		}

		public InternalCsvImportMap(CsvImportMap map) {
			this.HasHeader = map.HasHeader;
			this.AlertAmount = map.AlertAmount;
			this.Delimiter = map.Delimiter;
			this.TextDelimiter = map.TextDelimiter;
			this.EmptyValueAsNull = map.EmptyValueAsNull;
		}

		
	}
}
