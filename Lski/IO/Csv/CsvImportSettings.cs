using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lski.IO.Csv {
	
	/// <summary>
	/// Used to tell how a CsvImport object should handle importing of records
	/// </summary>
	/// <remarks>
	/// Used to tell how a CsvImport object should handle importing of records
	/// 
	/// If the settings do not include links, they are automatically generated to match the properties of the object being filled. If links are included then the Conversion and Transformation methods are
	/// optional and are auto generated, unless stated.
	/// </remarks>
	public class CsvImportSettings : CsvSettings {

		public const bool DefaultEmptyValueAsNull = true;

		/// <summary>
		/// States if the value coming in is empty (rather than what is set for NullValue) it should be set to null
		/// </summary>
		public bool EmptyValueAsNull { get; set; }

		/// <summary>
		/// The (optional) links to tell the CsvImport class how to handle the import
		/// </summary>
		public ICollection<CsvImportLink> Links { get; set; }

		public CsvImportSettings() : base() {
			EmptyValueAsNull = DefaultEmptyValueAsNull;
		}
	}
}
