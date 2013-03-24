using Lski.Txt.Conversion;
using Lski.Txt.Transformations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lski.IO.Csv {

	/// <summary>
	/// A link for importing a csv value into an object property and the methods for converting the value as desired
	/// </summary>
	public class CsvImportLink : CsvExportLink {

		private Transformations _Transformations;
		/// <summary>
		/// A list of tranformations to run on the data prior to conversion (Optional)
		/// </summary>
		public Transformations Tranformations {
			get {
				return _Transformations;
			}
			set {
				_Transformations = (value ?? new Transformations());
			}
		}

		public CsvImportLink() {
			_Transformations = new Transformations();
		}
	}
}
