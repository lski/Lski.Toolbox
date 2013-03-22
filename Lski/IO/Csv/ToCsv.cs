using Lski.Txt.Transformations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Lski.IO.Csv {

	public class ToCsv : Transformation {

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
				if(String.IsNullOrEmpty(value))
					throw new ArgumentException("TextDelimiter");

				_TextDelimiter = value;
				_DoubleTextDelimiter = value + value;
			}
		}

		private string _Delimiter;
		/// <summary>
		/// The delimiter used to surround text values when needed, eg. when the value itself contains a value delimiter
		/// </summary>
		public string Delimiter { 
			get {
				return _Delimiter;
			} 
			set {
				// Check for null here, rather than on processing to save the over head of checking each time
				if (String.IsNullOrEmpty(value))
					throw new ArgumentException("Delimiter");

				_Delimiter = value;
			}
		}

		/// <summary>
		/// States that text delimiters have to be placed around values, rather than just when needed. If true it will result in a larger file, but should generate faster.
		/// </summary>
		public bool EnforceTextDelimiter { get; set; }

		public ToCsv() {
			Init(CsvSettings.DefaultDelimiter, CsvSettings.DefaultTextDelimiter, CsvExportSettings.DefaultEnforceTextDelimiter);
		}

		public ToCsv(string delimiter, string textDelimiter, bool enforceTextDelimiter) {
			Init(delimiter, textDelimiter, enforceTextDelimiter);
		}

		public void Init(string delimiter, string textDelimiter, bool enforceTextDelimiter) {

			// Do checks of the arguments now to avoi

			if (String.IsNullOrEmpty(delimiter))
				throw new ArgumentException("delimiter");

			if(String.IsNullOrEmpty(textDelimiter))
				throw new ArgumentException("textDelimiter");

			TextDelimiter = textDelimiter;
			Delimiter = delimiter;
			EnforceTextDelimiter = enforceTextDelimiter;
		}

		public override string Process(string obj) {

			var str = Convert.ToString(obj);

			// If empty string then return 'as is' to save processing
			if (string.IsNullOrEmpty(str))
				return str;

			//' NOW check to see if a need for a text delimiter (regardless of whether there is one then there might be reason to add a default one)

			// 1. If there is a NOT text delimiter then check to see if one is required, if so use a default one, only if one is needed should the default
			//    text delimiter be escaped
			// 2. If there is a text delimiter then escape any text delimiters found, regardless of whether there is NEED for a text delimiter or not, also

			if (!EnforceTextDelimiter) {

				// Create a flag to check to see to add a text delimiter needs to be placed around the beginning and end of the value
				bool needsTextDelimiter = false;

				// If the value contains a value separator delimiter, then text delimiters HAVE to be added.
				if (Regex.IsMatch(str, "(\\n|\\r|" + Delimiter + ")"))
					needsTextDelimiter = true;

				// If one of the previous criteria means that the the value will be surrounded with text delimiters then escape any text delimiters found in the value
				if (needsTextDelimiter) {

					// replace each instance of " with ""
					str = str.Replace(TextDelimiter, _DoubleTextDelimiter);
					needsTextDelimiter = true;
				}

				// If needsDoubleQuotes has been triggered
				if (needsTextDelimiter)
					str = TextDelimiter + str + TextDelimiter;

			}
			else {
				str = TextDelimiter + str.Replace(TextDelimiter, _DoubleTextDelimiter) + TextDelimiter;
			}

			return str;
		}

		public override object Clone() {
			return new ToCsv(this.Delimiter, this.TextDelimiter, this.EnforceTextDelimiter);
		}
	}
}
