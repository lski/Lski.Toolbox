using Lski.Txt.Transformations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lski.IO.Csv {

	/// <summary>
	/// Processes a value that is in the style of a CSV value to a string without CSV formatting
	/// </summary>
	public class FromCsv : Transformation {

		private string _TextDelimiter;
		private string _DoubleTextDelimiter;
		/// <summary>
		/// The value delimiter
		/// </summary>
		public string TextDelimiter {
			get {
				return _TextDelimiter;
			}
			set {
				// Check for null here, rather than on processing to save the over head of checking each time
				if (String.IsNullOrEmpty(value))
					throw new ArgumentException("TextDelimiter");

				_TextDelimiter = value;
				_DoubleTextDelimiter = value + value;
			}
		}

		public FromCsv() {
			this.TextDelimiter = CsvExportSettings.DefaultTextDelimiter;
		}

		public FromCsv(string textDelimiter) {
			this.TextDelimiter = textDelimiter;
		}

		public override string Process(string value) {

			if (string.IsNullOrEmpty(value))
				return value;

			// If commas surrounding the value, remove them
			if (value.StartsWith(TextDelimiter) && value.EndsWith(TextDelimiter)) {

				// If it has two double quote but has nothing else in it, simply return an empty string rather running substring which will error
				if ((value.Length - 2) == 0)
					return "";

				value = value.Substring(1, value.Length - 2);
			}

			// If it contains any other text delimiters then it should be in a pair, if so change them to a single text delimiter from a double
			return value.Replace(_DoubleTextDelimiter, _TextDelimiter);
		}

		public override object Clone() {
			return new FromCsv(this.TextDelimiter);
		}
	}
}
