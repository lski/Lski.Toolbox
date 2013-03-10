using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Lski.Txt;

namespace Lski.IO.Csv {

	/// <summary>
	/// A class that allows importing and exporting from csv files
	/// </summary>
	/// <remarks>A class that allows importing and exporting from csv files
	/// 
	/// Uses the passed TableControl object to use the appropriate objects for the database being used.
	/// 
	/// Internally holds an object which includes the default settings for this object. These defaults can be changed, or
	/// each method can be passed its own individual settings.
	/// 
	/// During the import or export processes the methods fire events to inform the user of the amount of values imported/exported.
	/// </remarks>
	public class CsvImport : ICsv  {
		
		public CsvImportSettings Settings { get; private set; }

		public virtual string NULLValue {
			get { return "NULL"; }
		}


		protected CsvImport() { 
			// Empty for serializers only
		}

		public CsvImport(CsvSettings settings) {
			
			this.Settings = new CsvImportSettings() {
				Delimiter = settings.Delimiter,
				TextDelimiter = settings.TextDelimiter,
				HasHeader = settings.HasHeader
			};
		}

		public CsvImport(CsvImportSettings settings) {
			this.Settings = settings;
		}

		#region "Private Internal Methods"

		/// <summary>
		/// Gets the csv file reader
		/// </summary>
		/// <param name="fileName"></param>
		/// <exception cref="FileNotExistsException">Fires if the file can not be found</exception>
		/// <returns></returns>
		/// <remarks></remarks>
		protected StreamReader CreateFileReader(string fileName) {

			StreamReader r = null;

			// If the file does not exist then exit
			if (!File.Exists(fileName)) {
				throw new FileNotFoundException(fileName);
			}

			r = new StreamReader(fileName);

			// If the file is empty, return as there is nothing to import
			if (r.EndOfStream) {
				return null;
			}

			return r;
		}

		/// <summary>
		/// Takes the imported line and adds each of the values to the datarow passed, which is then altered and passed back
		/// </summary>
		/// <param name="map">The datamap to tell the computer how to link the data table and csv files</param>
		/// <param name="csvLine">The data from the csv file</param>
		/// <param name="row">The datarow to fill</param>
		/// <returns></returns>
		/// <remarks></remarks>
		private T FromCsvLine<T>(CsvImportSettings map, ICollection<InternalCsvImportMapLink> mapLinks, string[] csvLine) {

			// Save recalling it
			var csvLineCount = csvLine.Count(); 
			var result = Activator.CreateInstance<T>();

			// loop through each of the link objects, to get their value, in the correct format, via the type stored
			foreach (var mapLink in mapLinks) {
			
				// Because some lines could possibly be smaller than the position of the map, dont attempt to import it!
				if (mapLink.Position < csvLineCount) {

					ProcessImportedValue(map, mapLink, result, csvLine[mapLink.Position]);
				}
			}

			return result;
		}

		/// <summary>
		/// Works with the meta information about the current column in the csv file stored in the csvCol object and uses it
		/// to manipulate the csvValue so that its in an appropriate format for inserting into the dataTable.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Works with the meta information about the current column in the csv file stored in the csvCol object and uses it
		/// to manipulate the csvValue so that its in an appropriate format for inserting into the dataTable.
		/// 
		/// NOTE: Needs to be overridden in a database specific subclass
		/// </remarks>
		internal void ProcessImportedValue<T>(CsvImportSettings map, InternalCsvImportMapLink link, T obj, string valueToProcess) {

			// Because the methods to insert csv values use dataTables, convert from string null to DBNull
			// Applicable to all types of values so should be done prior 
			if (this.NULLValue.Equals(valueToProcess, StringComparison.OrdinalIgnoreCase)) {

				link.Property.SetValue(obj, null, null);
				return;
			}

			if (link.Tranformations != null) {
				// Strip the csv rubbish from the string and format it as requested, if null returned, convert to DBNull.Value
				valueToProcess = link.Tranformations.Process(ConvertFromCsvValue(valueToProcess, map.TextDelimiter));
			}

			// If the map states that empty strings should be handled as null return null when its an empty string, otherwise return the empty string
			if (map.EmptyValueAsNull && valueToProcess.Length == 0) {

				link.Property.SetValue(obj, null, null);
				return;
			}

			// Run the datamap types own conversion method to parse the incoming value
			link.Property.SetValue(obj, link.Conversion.Parse(valueToProcess), null);
		}

		/// <summary>
		/// Simply takes a line from the csv file and splits it according to the map. It returns the values as is, in same positions as in the csv file
		/// </summary>
		/// <param name="csvLine"></param>
		/// <param name="delimiter">The delimiter to split the csv line with</param>
		/// <returns></returns>
		/// <remarks></remarks>
		protected virtual string[] SplitCsvLine(string csvLine, string delimiter) {
			return csvLine.SplitAdv(delimiter);
		}

		#endregion

		#region "Public Conversion Methods"

		/// <summary>
		/// Takes the value passed that is in the format of a value from a csv file, and converts it into a 'normal' string therefore no escaped sequences
		/// </summary>
		/// <param name="str">The csv style value to convert</param>
		/// <returns></returns>
		/// <remarks></remarks>
		public static string ConvertFromCsvValue(string str, string textDelimiter) {

			if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(textDelimiter)) {
				return str;
			}

			// If commas surrounding the value, remove them
			if (str.StartsWith(textDelimiter) && str.EndsWith(textDelimiter)) {

				// If it has two double quote but has nothing else in it, simply return an empty string rather running substring which will error
				if ((str.Length - 2) == 0) {
					return string.Empty;
				}

				str = str.Substring(1, str.Length - 2);
			}

			// If it contains any other text delimiters then it should be in a pair, if so change them to a single 'double' quote
			str = str.Replace(textDelimiter + textDelimiter, textDelimiter);

			return str;
		}



		#endregion

		#region "Import Methods"

		/// <summary>
		/// Inserts the values from the csv file into the passed dataTable. This process does not effect values already
		/// held in the dataTable. 
		/// </summary>
		/// <param name="fileName">The full file name of the csv to import from.</param>
		/// <returns>The amount of records imported</returns>
		/// <exception cref="IndexOutOfRangeException">Thrown if a column links refers to a position not the csv line</exception>
		/// <remarks>Inserts the values from the csv file into the held TableControls dataTable. This process does not effect values already
		/// held in the dataTable. 
		/// 
		/// The passed list of CsvColumnData object relate to each being a suggested link between a value held in a 
		/// certain position on a line in the csv file and a column in the dataTable this value will be inserted into. Note:
		/// If a link does not match a position in the 
		/// 
		/// </remarks>
		public IEnumerable<T> Import<T>(string filename) where T : new() {

			StreamReader csvReader = null;

			try {

				// Try to open the selected file
				csvReader = CreateFileReader(filename);

				foreach (var item in InternalImport<T>(this.Settings, csvReader)) {
					yield return item;
				}
			}
			finally {

				if (csvReader != null) {
					csvReader.Close();
				}
			}

		}

		public IEnumerable<T> Import<T>(Stream fs) where T : new() {

			foreach (var item in InternalImport<T>(this.Settings, new StreamReader(fs))) {
				yield return item;
			}

		}

		/// <summary>
		/// Inserts the values from the csv file into the passed dataTable. This process does not effect values already
		/// held in the dataTable. 
		/// </summary>
		/// <param name="fileName">The full file name of the csv to import from.</param>
		/// <returns>The amount of records imported</returns>
		/// <exception cref="IndexOutOfRangeException">Thrown if a column links refers to a position not the csv line</exception>
		/// <remarks>Inserts the values from the csv file into the held TableControls dataTable. This process does not effect values already
		/// held in the dataTable. 
		/// 
		/// The passed list of CsvColumnData object relate to each being a suggested link between a value held in a 
		/// certain position on a line in the csv file and a column in the dataTable this value will be inserted into. Note:
		/// If a link does not match a position in the 
		/// 
		/// </remarks>
		public IEnumerable<T> InternalImport<T>(CsvImportSettings map, StreamReader rdr) where T : new() {

			if (rdr == null)
				yield break;

			if (map.Links == null && !map.HasHeader) {
				throw new ArgumentException("If not manually providing links to CsvImport there must be a header to match against property names");
			}
			// If there are no links then get names for the links from the first line of 
			else if(map.Links == null) {
				map.Links = CreateLinksFromHeader(map.Delimiter, rdr);
			}
			// If there are links and the file has an header move the pointer in the stream on one line
			else if (map.Links != null && map.HasHeader) {
				rdr.ReadLine();
			}

			var internalMap = InternalCsvImportMapLink.CreateInternalLinks<T>(map.Links);

			while (!rdr.EndOfStream) {

				// Add the row to the dataTable
				yield return FromCsvLine<T>(map, internalMap, SplitCsvLine(rdr.ReadLine(), map.Delimiter));
			}
		}

		/// <summary>
		/// Uses the CSV files first livne to create a set of auto generated links to store in map.Links
		/// </summary>
		/// <param name="rdr"></param>
		/// <returns></returns>
		private ICollection<CsvImportLink> CreateLinksFromHeader(String delimiter, StreamReader rdr) {

			var line = rdr.ReadLine();
			var lst = new List<CsvImportLink>();

			if (String.IsNullOrEmpty(line)) {
				return lst;
			}

			var i = 0;
			foreach (var item in SplitCsvLine(line, delimiter)){
				lst.Add(new CsvImportLink() {
					 Position = i++,
					 Property = item
				});
			}

			return lst;
		}

		#endregion

	}
}
