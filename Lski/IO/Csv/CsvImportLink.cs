using Lski.Txt.Conversion;
using Lski.Txt.Transformations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lski.IO.Csv {

	public class CsvImportLink : CsvExportLink {

		private Transformations _Transformations;
		/// <summary>
		/// A list of tranformations to run on the data prior to conversion
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
