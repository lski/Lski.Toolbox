using Microsoft.VisualBasic;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Data.Common;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;

using Lski.Data.Extensions;
using Lski.Txt;
using Lski.IO.FileSystem;
using Lski.IO.FileSystem.Exceptions;
using Lski.Txt.Conversion;
using Lski.Txt.Transformations;

namespace Lski.IO.CsvDataTable {

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
	public class CsvImport : ICsv {

		#region "Constructors"

		protected CsvImport() {}

		public CsvImport(String filename, CsvImportMap map) {
			this.Init(filename, map);
		}

		public CsvImport(string filename, string delimiter, bool hasHeader) {
			this.Init(filename, new CsvImportMap() { Delimiter = delimiter, HasHeader = hasHeader });
		}

		public void Init(String filename, CsvImportMap map) {
			this._Filename = filename;
			this.DataMap = map;
		}

		#endregion

		#region "Properties"

		public CsvImportMap DataMap { get; set; }

		public virtual string NULLValue {
			get { return "NULL"; }
		}

		protected String _Filename;
		public String Filename { get { return _Filename; } }

		#endregion

		#region "Events"

		/// <summary>
		/// Is thrown each time an individual record is imported FROM a csv file.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		/// <remarks></remarks>
		public event ImportValueChangedEventHandler ImportValueChanged;
		public delegate void ImportValueChangedEventHandler(object sender, ValueChangedArgs args);

		/// <summary>
		/// Is thrown once an import process has completed, this is useful to users that run these methods under a separate thread
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		/// <remarks>Is thrown once an import process has completed, this is useful to users that run these methods under a separate thread
		/// 
		/// NOTE: Can be caught with the arguement super class TransferCompleteArgs instead if the handler needs to be more generic
		/// </remarks>
		public event ImportFinishedEventHandler ImportFinished;
		public delegate void ImportFinishedEventHandler(object sender, CsvTransferCompleteArgs args);

		#endregion

		#region "Private Internal Methods"

		/// <summary>
		/// Gets the csv file reader
		/// </summary>
		/// <param name="fileName"></param>
		/// <exception cref="FileNotExistsException">Fires if the file can not be found</exception>
		/// <returns></returns>
		/// <remarks></remarks>
		protected StreamReader PrvGetFileReader(string fileName) {

			StreamReader r = null;

			// If the file does not exist then exit
			if (!File.Exists(fileName))	
				throw new FileNotFoundException(fileName);

			r = new StreamReader(fileName);

			// If the file is empty, return as there is nothing to import
			if (r.EndOfStream) return null;

			return r;
		}

		protected DataTable CreateDataTable(string[] headers, String tablename) {

			DataTable tab = new DataTable(tablename);

			foreach (var field in headers) {
				tab.Columns.Add(new DataColumn(field, typeof(System.String)));
			}

			return tab;

		}

		protected DataTable CreateDataTableUsingLinks(string[] headers, List<CsvDataMapLink> csvLinks, String tablename) {

			DataTable tab = new DataTable(tablename);

			// Loop through each map item so that first the source can be checked against the headers
			foreach (var link in csvLinks) {
				if (headers.Contains(link.Target, StringComparer.OrdinalIgnoreCase))
					tab.Columns.Add(tab.UniqueColumnName(link.Target), link.DataType.Type);
			}

			return tab;
		}

		/// <summary>
		/// Takes the imported line and adds each of the values to the datarow passed, which is then altered and passed back
		/// </summary>
		/// <param name="map">The datamap to tell the computer how to link the data table and csv files</param>
		/// <param name="csvLine">The data from the csv file</param>
		/// <param name="row">The datarow to fill</param>
		/// <returns></returns>
		/// <remarks></remarks>
		private DataRow FromCsvLine(CsvImportMap map, List<CsvDataMapLink> validatedMapItems, string[] csvLine, DataRow row) {

			Int32 csvLineCount = csvLine.Count(); // Save recalling it

			// loop through each of the link objects, to get their value, in the correct format, via the type stored
			for (Int32 i = 0, n = validatedMapItems.Count; i < n; i++) {
				// Because some lines could possibly be smaller than the position of the map, dont attempt to import it!
				if (validatedMapItems[i].LinePosition < csvLineCount) 
					row[validatedMapItems[i].Target] = ProcessImportedValue(map, validatedMapItems[i], csvLine[validatedMapItems[i].LinePosition]);
			}

			return row;

		}

		/// <summary>
		/// Uses the passed information to insert the information from the
		/// CSV file to the DataTable
		/// </summary>
		/// <returns></returns>
		/// <remarks></remarks>
		private Int32 InsertRowsInDataTable(DataTable table, CsvImportMap map, List<CsvDataMapLink> validatedDataMapItems, StreamReader csvReader) {

			// The current record that the loop is importing
			Int32 currentRecord = 0;

			// If there is not a alert amount, simply dont check and raise the value

			if (map.AlertAmount < 0) {
				// Loop through each line of the csv file, until the file is empty

				while (!csvReader.EndOfStream) {
					// Get the next line for processing and increase the counter
					currentRecord += 1;

					// Add the row to the dataTable
					table.Rows.Add(FromCsvLine(map, validatedDataMapItems, SplitCsvLine(csvReader.ReadLine(), map.Delimiter), table.NewRow()));
				}


			} else {

				Int32 nextAlert = map.AlertAmount;

				// Loop through each line of the csv file, until the file is empty
				while (!csvReader.EndOfStream) {
					
					// Get the next line for processing and increase the counter
					currentRecord++;

					// Add the row to the dataTable
					table.Rows.Add(FromCsvLine(map, validatedDataMapItems, SplitCsvLine(csvReader.ReadLine(), map.Delimiter), table.NewRow()));

					// If the next alert value equals the current amount of records imported
					if (currentRecord == nextAlert) {

						// Raise an event to let listners know that another record has been imported
						if (ImportValueChanged != null) 
							ImportValueChanged(this, new ValueChangedArgs(currentRecord, ValueChangedArgs.ValueType.Imported));
						
						nextAlert += map.AlertAmount;
					}

				}

			}

			// Return the number of rows added
			return currentRecord;

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
		protected virtual object ProcessImportedValue(CsvImportMap map, CsvDataMapLink mapItem, string valueToProcess) {

			// Because the methods to insert csv values use dataTables, convert from string null to DBNull
			// Applicable to all types of values so should be done prior 
			if (string.Compare(valueToProcess, this.NULLValue, true) == 0) 
				return DBNull.Value;

			// Strip the csv rubbish from the string and format it as requested, if null returned, convert to DBNull.Value
			valueToProcess = mapItem.Translations.Process(ConvertFromCsvValue(valueToProcess, map.TextDelimiter));

			// If the map states that empty strings should be handled as null return null when its an empty string, otherwise return the empty string
			if (map.EmptyValueAsNull && valueToProcess.Length == 0)	
				return DBNull.Value;// Nothing

			// Run the datamap types own conversion method to parse the incoming value
			return mapItem.DataType.Parse(valueToProcess) ?? DBNull.Value;
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

		/// <summary>
		/// Returns the First line of the file as string array, whether thats the header line or the first record. The reader is left 
		/// </summary>
		/// <param name="reader">The file reader object</param>
		/// <remarks>
		/// Returns the First line of the file as string array, whether thats the header line or the first record. The reader is left
		/// pointing before the first record regardless of whether it extracted the first line or header. Rather than taking a full datamap
		/// takes different parts of it in case when running this method the 
		/// </remarks>  
		protected virtual string[] GetHeadersAndResetToFirstLine(ref StreamReader reader, string delimiter, string textDelimiter, bool hasHeader) {

			string[] firstLine = null;

			// Put the reader back to the beginning of the file VERY IMPORTANT clear buffer
			reader.DiscardBufferedData();
			reader.BaseStream.Seek(0, SeekOrigin.Begin);

			if (reader.EndOfStream)	
				return null;

			// Try to get the first line of the csv file

			try {
				// If the csv file should have a header, use the first line to create the headers
				// Collect the first line regardless of whether there is a header
				firstLine = SplitCsvLine(reader.ReadLine(), delimiter);

				if (hasHeader) {
					for (Int32 i = 0; i <= (firstLine.Length - 1); i++) {
						firstLine[i] = StringExt.Trim(ConvertFromCsvValue(firstLine[i], textDelimiter)); // Trim it, as it is a header item
					}

				} else {
					for (Int32 i = 0; i <= (firstLine.Length - 1); i++) {
						firstLine[i] = StringExt.ConvertToExcelColumnName(i+1);
					}
				}


			} catch (Exception ex) {
				throw new Exception("There was an error reading the first line of the file", ex);
			}

			// If this file doesnt have a header then it needs to be returned to the beginning of the file
			if (!hasHeader) {
				reader.DiscardBufferedData();
				reader.BaseStream.Seek(0, SeekOrigin.Begin);
			}

			return firstLine;
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

			if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(textDelimiter)) return str;

			// If commas surrounding the value, remove them
			if (str.StartsWith(textDelimiter) && str.EndsWith(textDelimiter)) {

				// If it has two double quote but has nothing else in it, simply return an empty string rather running substring which will error
				if ((str.Length - 2) == 0) return string.Empty;

				str = str.Substring(1, str.Length - 2);
			}

			// If it contains any other text delimiters then it should be in a pair, if so change them to single, double quotes
			str = Regex.Replace(str, textDelimiter + textDelimiter, textDelimiter);

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
		public DataTable Import() {

			Int32 insertCount = 0;
			StreamReader csvReader = null;

			try {

				// Try to open the selected file
				csvReader = PrvGetFileReader(this.Filename);

				if (csvReader == null)
					return null;

				// Get the header line to check for first line
				String[] headerLine = GetHeadersAndResetToFirstLine(ref csvReader, this.DataMap.Delimiter, this.DataMap.TextDelimiter, this.DataMap.HasHeader);

				var validatedDataMapItems = CreateValidatedDataMapItems(headerLine, this.DataMap);

				if (this.DataMap.DataMapLinks.Count == 0) {

					var qry = (from x in validatedDataMapItems select (DataMapLink)x);
					this.DataMap.DataMapLinks = qry.ToList();
				}

				var table = CreateDataTableUsingLinks(headerLine, validatedDataMapItems, Path.GetFileNameWithoutExtension(this.Filename));
				
				insertCount = InsertRowsInDataTable(table, this.DataMap, validatedDataMapItems, csvReader);

				// Raise the event to state this import has finished
				if (ImportFinished != null)
					ImportFinished(this, new CsvTransferCompleteArgs(insertCount, TransferCompleteArgs.ValueType.Imported, this.Filename, table.TableName));

				return table;

			} finally {

				if (csvReader != null) csvReader.Close();
			}

		}

		public List<CsvDataMapLink> CreateValidatedDataMapItems(string[] firstLine, CsvImportMap dataMap) {

			if (firstLine == null)
				throw new ArgumentNullException("firstLine");

			List<CsvDataMapLink> validatedMapItems = new List<CsvDataMapLink>(0);

			var firstLineLength = (firstLine.Length);

			// If the datamap has no items, then they need to be created from scratch
			if (dataMap.DataMapLinks.Any()) {

				// If the dataMap states the firstLine is a header then create the list from a combination of headerline and matching map
				if(dataMap.HasHeader) {

					// Loop through each item in the map to compare it against the header line
					foreach (var mapItem in dataMap.DataMapLinks) {

						// Get the value for the header name from the map
						var headerString = mapItem.Source;

						// Reinitialise the loop
						var i = 0;
							

						// Now loop through each header and see if there is a match between the source/target in the map item
						while (i < firstLineLength) {

							// Check to see if the header line matches the the map header (case insensitive)
							if (firstLine[i].Equals(mapItem.Source, StringComparison.OrdinalIgnoreCase)) {

								validatedMapItems.Add(new CsvDataMapLink(mapItem.Source, 
																		 mapItem.Target,
																		 i,
																		 (ConvertTo)mapItem.DataType.Clone(), 
																		 (TransformValues)mapItem.Translations.Clone()));
							}

							i++; // Increment the counter
						}
					}

				} else { // Has no header, but has mapItems, therefore should be a number

					// The parsed position holder
					Int32 tmpPosition = -1;

					// Loop through each item in the map to compare it against the header line
					foreach (var mapItem in dataMap.DataMapLinks) {

						// If the source/target from the data map is not a numeric, then it can not match a position in the list
						if (!Int32.TryParse(mapItem.Source, out tmpPosition)) 
							continue;

						// Now check to see if the position is within range
						if (tmpPosition < 0 || tmpPosition >= firstLineLength) 
							continue;

						validatedMapItems.Add(new CsvDataMapLink(mapItem.Source,
																 mapItem.Target,
																 tmpPosition,
																 (ConvertTo)mapItem.DataType.Clone(),
																 (TransformValues)mapItem.Translations.Clone()));
					}
				}

			} else { // If No map items the they have to be completely create it from scratch

				if (dataMap.HasHeader) { // Header but no mapItems

					for (var i = 0; i < firstLineLength; i++) {

						validatedMapItems.Add(new CsvDataMapLink(i.ToString(), // Add the source as it will be needed
																 firstLine[i],
																 i));
					}

				} else { //No header and no mapitems

					for (var i = 0; i < firstLineLength; i++) {

						validatedMapItems.Add(new CsvDataMapLink(i.ToString(), // Add the source as it will be needed
																 StringExt.ConvertToExcelColumnName(i+1),
																 i));
					}
				}
			}

			return validatedMapItems;
		}

		#endregion

	}

}
