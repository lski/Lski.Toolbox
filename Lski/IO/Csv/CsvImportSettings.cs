using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lski.IO.Csv {
	
	public class CsvImportSettings : CsvSettings {

		public const bool DefaultEmptyValueAsNull = true;
		public bool EmptyValueAsNull { get; set; }
		public ICollection<CsvImportLink> Links { get; set; }

		public CsvImportSettings() : base() {
			EmptyValueAsNull = DefaultEmptyValueAsNull;
		}
	}
}
