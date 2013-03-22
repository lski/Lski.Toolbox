using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Lski.IO.Csv {

	/// <summary>
	/// A class designed to store the links for exporting a Csv file from code.
	/// </summary>
	public class CsvExportSettings : CsvSettings {

		#region "Properties"

		public const bool DefaultAppendToFile = false;
		public const bool DefaultNullAsEmptyValue = true;
		public const bool DefaultEnforceTextDelimiter = false;

		/// <summary>
		/// States whether the export should replace any file found or simply append to it
		/// </summary>
		public bool AppendToFile { get; set; }
		
		/// <summary>
		/// States whether the value should be converted to an empty string or the value stored for the CsvExport class should be used
		/// </summary>
		public bool NullAsEmptyValue { get; set; }

		/// <summary>
		/// When true, regardless of whether a text value being exported requires it, text delimiters will be added. If false text delimiters are only added when needed.
		/// </summary>
		public bool EnforceTextDelimiter { get; set; }

		/// <summary>
		/// The links between properties in the object and the positions in the CSV file
		/// </summary>
		public ICollection<CsvExportLink> Links { get; set; }

		#endregion

		public CsvExportSettings()
			: base() {

			this.NullAsEmptyValue = DefaultNullAsEmptyValue;
			this.AppendToFile = DefaultAppendToFile;
			this.EnforceTextDelimiter = DefaultEnforceTextDelimiter;
		}
	}
}
