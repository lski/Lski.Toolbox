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
using Lski.Txt.ConvertTo;
using Lski.Txt.Transformations;

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
		/// Takes the value passed that is in the format of a value from a csv file, and converts it into a 'normal'
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

///// <summary>
///// Creates a data map using the header line from the csv file and the fields from a datatable or resultset
///// </summary>
///// <param name="header">The first line of the csv line</param>
///// <param name="tableFieldNames">The list of field names from the data table or resultset with their data types</param>
///// <param name="delimiter">The delimiter string to split the csv header line up with</param>
///// <param name="isImport">States whether this is an import from or export to a csv file. Depending on which sets the appropriate field in the datamapitem.</param>
///// <returns></returns>
///// <remarks>
///// Creates a data map using the header line from the csv file and the fields from a datatable or resultset
///// </remarks>
//protected List<CsvDataMapItem> CreateCsvDataMapItemList(string[] header, string delimiter, bool hasHeader) {

//    // Counters
//    Int32 i = 0, j = 0, n = (header.Length - 1);

//    CsvDataMapItem newMapItem = null;
//    var newMap = new List<CsvDataMapItem>();

//    try {

//        if(!hasHeader)
//            throw new ArgumentException("A Csv Data Map List can not be created for use with importing a new Data Table");

//        // If the csvLine is Null then the no rows exception, whether has header or not
//        if (header == null)
//            throw new ArgumentNullException("The csv line passed to the CreateDataMap method can not be null");

//        for (i = 0; i <= n; i++) {

//            newMapItem = new CsvDataMapItem(header[i], j, new DataTypeText());
//            newMap.Add(newMapItem);

//        }

//    } catch (Exception ex) {
//        throw new Exception("There was an error getting linking the columns from the CSV to the DataTable", ex);
//    }

//    return newMap;
//}

///// <summary>
///// Creates a data map using the header line from the csv file and the fields from a datatable or resultset
///// </summary>
///// <param name="header">The first line of the csv line</param>
///// <param name="tableFieldNames">The list of field names from the data table or resultset with their data types</param>
///// <param name="delimiter">The delimiter string to split the csv header line up with</param>
///// <param name="isImport">States whether this is an import from or export to a csv file. Depending on which sets the appropriate field in the datamapitem.</param>
///// <returns></returns>
///// <remarks>
///// Creates a data map using the header line from the csv file and the fields from a datatable or resultset
///// </remarks>
//protected List<CsvDataMapItem> CreateCsvDataMapItemList(string[] header, DataColumnCollection tableFieldNames, string delimiter, bool hasHeader) {

//    // Counters
//    Int32 i = default(Int32);
//    Int32 j = default(Int32);

//    // A flag for checking if there was a match in the csv header line with the database table fields
//    bool hasMatch = false;

//    CsvDataMapItem newMapItem = null;
//    var newMap = new List<CsvDataMapItem>();

//    try {
//        // If the csvLine is Null then the no rows exception, whether has header or not
//        if (header == null)	
//            throw new ArgumentNullException("The csv line passed to the CreateDataMap method can not be null");

//        // 1. If the file has a header line then match against the headers in that line tableFieldNames
//        // 2. Otherwise the positions should match the table positions, anything out of bounds either way should be ignored
//        if (hasHeader) {
//            // Loop through each of the database table column names to see if there is a matching column in the csv file. It checks 
//            // this by doing a case-insensitive comparison between fieldName and csvHeader name

//            for (i = 0; i <= (tableFieldNames.Count - 1); i++) {
//                // reset the has a match flag
//                hasMatch = false;

//                // Loop through each part of the header line

//                for (j = 0; j <= (header.Length - 1); j++) {

//                    // If the current database field name is equal to the current csv header value signify a match has been found, and prevent addtional looping
//                    if (string.Compare(tableFieldNames[i].ColumnName, header[j], true) == 0) {
//                        hasMatch = true;
//                        break;
//                    }
//                }

//                // If a match was found then add it to the new CsvColumnInfo list
//                // DO NOT remove from current list as it will screw up this list, and might cause silent errors on save
//                if (hasMatch) {
//                    newMapItem = new CsvDataMapItem(tableFieldNames[i].ColumnName, j, DataType.GetDataMapType(tableFieldNames[i].DataType));
//                    newMap.Add(newMapItem);
//                }

//            }

//        // Doesnt have header
//        } else {

//            // Get the loop end, which is the shorter out of the table names 
//            Int32 loopEnd = (tableFieldNames.Count > header.Length ? (header.Length - 1) : (tableFieldNames.Count - 1));

//            for (i = 0; i <= loopEnd; i++) {
//                newMapItem = new CsvDataMapItem(tableFieldNames[i].ColumnName, i, DataType.GetDataMapType(tableFieldNames[i].DataType));
//                newMap.Add(newMapItem);
//            }

//        }

//    } catch (Exception ex) {
//        throw new Exception("There was an error getting linking the columns from the CSV to the DataTable", ex);
//    }

//    return newMap;

//}

/// <summary>
/// Takes the data map passed to this import and validates the mappings inside so that they dont break when running
/// </summary>
/// <param name="map"></param>
/// <param name="fields">A list of the fields as the are stored in either the </param>
/// <param name="headerline">The first line of the csv file, whether containing headers or not</param>
/// <returns></returns>
/// <remarks></remarks>
//protected virtual List<CsvDataMapItem> ValidateMapForImport(CsvImportMap map, string[] fields, string[] headerLine) {

//    var newMap = new List<CsvDataMapItem>();

//    Int32 i = 0, j = 0, headerLoopEnd = (headerLine.Length), fieldsLoopEnd = (fields.Length);

//    // 1. If the data map states there is a header then the header line needs to be used to compare the header names with the the map being sent in
//    // 2. If there isnt a header line, then it should be presumed the map includes positions as strings rather than header names

//    if (map.HasHeader || headerLine == null) {

//        // Tmp holders to avoid constantly checking the isImport
//        string headerString = null, fieldString = null;

//        // Loop through each item in the map to compare it against the header line
//        foreach (var mapItem in map.DataMapItems) {

//            // Get the value for the header name from the map
//            headerString = mapItem.Source;

//            // Reinitialise the loop
//            i = 0;

//            // Now loop through each header and see if there is a match between the source/target in the map item
//            while (i < headerLoopEnd) {

//                // Check to see if the header line matches the the map header (case insensitive)
//                if (string.Compare(headerLine[i], headerString, true) == 0) {
//                    // Get the value for the field name from the map
//                    fieldString = mapItem.Target;

//                    // reinitialise the second loop variable
//                    j = 0;

//                    // If there is a match above then search to see if there is a valid field for the other source/target to match
//                    while (j < fieldsLoopEnd) {

//                        if (string.Compare(fields[j], fieldString, true) == 0) {
//                            newMap.Add(new CsvDataMapItem(fields[j], i, (DataType)mapItem.DataType.Clone(), (TransformValueCollection)mapItem.Translation.Clone()));
//                            //continue;
//                        }

//                        j++;
//                    }
//                }

//                // Increment the counter
//                i++;
//            }
//        }

//    } else {

//        // The parsed position holder
//        Int32 tmpPosition = -1;

//        // Loop through each item in the map to compare it against the header line
//        foreach (var mapItem in map.DataMapItems) {

//            // If the source/target from the data map is not a numeric, then it can not match a position in the list
//            if (!Int32.TryParse(mapItem.Source, out tmpPosition)) continue;

//            // Now check to see if the position is within range
//            if (tmpPosition < 0 || tmpPosition >= headerLine.Length) continue;

//            // Now check to see if the field is valid in the fields list

//            for (j = 0; j < fieldsLoopEnd; j++) {
//                if (string.Compare(fields[j], mapItem.Target, true) == 0) {
//                    newMap.Add(new CsvDataMapItem(fields[j], tmpPosition, (DataType)mapItem.DataType.Clone(), (TransformValueCollection)mapItem.Translation.Clone()));
//                }
//            }
//        }
//    }

//    return newMap;
//}

///// <summary>
///// Inserts the values from the csv file into the passed dataTable. This process does not effect values already
///// held in the dataTable. 
///// </summary>
///// <param name="fileName">The full filename of the csv to import from.</param>
///// <returns>The amount of records imported</returns>
///// <exception cref="IndexOutOfRangeException">Thrown if a column links refers to a position not the csv line</exception>
///// <remarks>Inserts the values from the csv file into the held TableControls dataTable. This process does not effect values already
///// held in the dataTable. 
///// 
///// The passed list of CsvColumnData object relate to each being a suggested link between a value held in a 
///// certain position on a line in the csv file and a column in the dataTable this value will be inserted into. Note:
///// If a link does not match a position in the 
///// 
///// </remarks>
//public Int32 FromCSV(string fileName, DataTable table, CsvImportMap map) {

//    string[] firstLine = null;
//    List<CsvDataMapItem> validatedDataItems = null;

//    // Used for looping throughout the method
//    Int32 insertCount = 0, i = 0, j = 0;

//    if (table == null) throw new ArgumentNullException("The datatable to fill with data from the CSV file can not be null");

//    StreamReader csvReader = null;

//    try {

//        csvReader = PrvGetFileReader(fileName);

//        if (csvReader == null) return 0;

//        firstLine = this.GetHeadersAndResetToFirstLine(ref csvReader, map.Delimiter, map.TextDelimiter, map.HasHeader);

//        // Validate the incoming column links against the fields in the dataTable
//        validatedDataItems = ValidateMapForImport(ref map, table.Columns, firstLine);

//        insertCount = InsertRowsInDataTable(table, map, validatedDataItems, csvReader);

//        // Raise the event to state this import has finished
//        if (ImportFinished != null)
//            ImportFinished(this, new CsvTransferCompleteArgs(insertCount, TransferCompleteArgs.ValueType.Imported, fileName, table.TableName));

//    } finally {

//        if(csvReader != null) csvReader.Close();
//    }

//    return insertCount;
//}

///// <summary>
///// Inserts the values from the csv file into the held TableControls dataTable. This process does not effect values already
///// held in the dataTable. 
///// </summary>
///// <param name="fileName">The full file name of the csv to import from.</param>
///// <param name="delimiter">The string used to separate each value on each line of the csv file, if not supplied the delimiter 
///// in the settings property is used instead.</param>
///// <returns>The amount of records imported</returns>
///// <remarks>Inserts the values from the csv file into the held TableControls dataTable. This process does not effect values already
///// held in the dataTable. 
///// </remarks>
//public Int32 FromCSV(string fileName, DataTable table, string delimiter) {

//    if (table == null) 
//        throw new ArgumentNullException("When importing from a comma separated value file the table must be set in advance");

//    StreamReader csvReader = null;

//    // Try to open the selected file
//    try {
//        csvReader = PrvGetFileReader(fileName);

//        if (csvReader == null) return 0;



//        List<CsvDataMapItem> validatedDataMapItems = CreateInternalDataMap(GetHeadersAndResetToFirstLine(ref csvReader, delimiter, CsvDataMap.DefaultTextDelimiter, true), table.Columns, delimiter, true, true);

//        var map = new CsvImportMap() { Delimiter = delimiter };

//        Int32 insertCount = InsertRowsInDataTable(table, map, validatedDataMapItems, csvReader);

//        // Raise the event to state this import has finished
//        if (ImportFinished != null)
//            ImportFinished(this, new CsvTransferCompleteArgs(insertCount, TransferCompleteArgs.ValueType.Imported, fileName, table.TableName));

//        return insertCount;

//    } finally {

//        if (csvReader != null) 
//            csvReader.Close();
//    }

//}

///// <summary>
///// Inserts the values from the csv file into the held TableControls dataTable. This process creates a new data table, with each of the columns
///// set to string data type.
///// </summary>
///// <param name="fileName">The full file name of the csv to import from.</param>
///// <param name="delimiter">The string used to separate each value on each line of the csv file, if not supplied the delimiter 
///// in the settings property is used instead.</param>
///// <returns>The amount of records imported</returns>
///// <remarks>Inserts the values from the csv file into the held TableControls dataTable. This process does not effect values already
///// held in the dataTable. 
///// </remarks>
//public DataTable FromCSV() {

//    StreamReader csvReader = null;

//    // Try to open the selected file
//    try {
//        csvReader = PrvGetFileReader(fileName);

//        if (csvReader == null) return null;

//        String[] headerLine = GetHeadersAndResetToFirstLine(ref csvReader, delimiter, CsvDataMap.DefaultTextDelimiter, hasHeader);

//        DataTable table = CreateDataTable(headerLine);
//        table.TableName = new FileInfo(fileName).NameWithoutExt();

//        List<CsvDataMapItem> validatedDataMapItems = CreateInternalDataMap(headerLine, table.Columns, delimiter, true, true);

//        var map = new CsvImportMap() { Delimiter = delimiter };

//        Int32 insertCount = 0;
//        insertCount = InsertRowsInDataTable(table, map, validatedDataMapItems, csvReader);

//        // Raise the event to state this import has finished
//        if (ImportFinished != null)
//            ImportFinished(this, new CsvTransferCompleteArgs(insertCount, TransferCompleteArgs.ValueType.Imported, fileName, table.TableName));

//        return table;

//    } finally {
//        if (csvReader != null) csvReader.Close();
//    }

//}

//#region "Inner Classes"

///// <summary>
///// Works as an internal csv data map after the positions within the file have been worked out. And removes the requirement to cast every data map item for each line coming in.
///// </summary>
///// <remarks></remarks>
//protected class InternalCsvDataMap : ListS<CsvDataMapItem> {

//    private string _delimiter;
//    /// <summary>
//    /// The string used to separate each value in the line. Default = ","
//    /// </summary>
//    /// <remarks></remarks>
//    public string Delimiter {
//        get { return _delimiter; }
//        set { _delimiter = value; }
//    }

//    private string _textDelimiter;
//    /// <summary>
//    /// States the delimiter used to say a value is a string or not. Usually just a double quote or a single quote.
//    /// </summary>
//    /// <value></value>
//    /// <returns></returns>
//    /// <remarks></remarks>
//    public string TextDelimiter {
//        get { return _textDelimiter; }
//        set { _textDelimiter = value; }
//    }

//    private bool _appendHeader;
//    /// <summary>
//    /// States whether to attach a header if possible to the csv file. Default = "True"
//    /// </summary>
//    /// <remarks></remarks>
//    public bool AppendHeader {
//        get { return _appendHeader; }
//        set { _appendHeader = value; }
//    }

//    private bool _hasHeader;
//    /// <summary>
//    /// States whether the csv file has a header or not, if stated as not but it in fact does, the header row will be imported
//    /// in the same way as the other rows. However it does not take out into datatypes and might in fact break on import.
//    /// Default = "True"
//    /// </summary>
//    /// <remarks></remarks>
//    public bool HasHeader {
//        get { return _hasHeader; }
//        set { _hasHeader = value; }
//    }

//    private bool _appendToFile;
//    /// <summary>
//    /// States whether when exporting to the csv file the values are added to the end of the file, rather than using an empty
//    /// file. Overrides the setting 'AppendHeader' if the file exists and set to true. (Default = "False")
//    /// </summary>
//    /// <remarks></remarks>
//    public bool AppendToFile {
//        get { return _appendToFile; }
//        set { _appendToFile = value; }
//    }

//    private Int32 _alertAmount;
//    /// <summary>
//    /// States the value for the amount of records between each fire of ValueChanged during import/export
//    /// </summary>
//    /// <remarks></remarks>
//    public Int32 AlertAmount {
//        get { return _alertAmount; }
//        set { _alertAmount = value; }
//    }

//    private Boolean _emptyValueAsNull;
//    /// <summary>
//    /// States whether empty values should be counted as null
//    /// </summary>
//    /// <remarks></remarks>
//    public bool EmptyValueAsNull {
//        get { return _emptyValueAsNull; }
//        set { _emptyValueAsNull = value; }
//    }


//    public InternalCsvDataMap() {
//        _delimiter = CsvDataMap.DefaultDelimiter;
//        _textDelimiter = CsvDataMap.DefaultTextDelimiter;
//        _appendHeader = CsvDataMap.DefaultAppendHeader;
//        _hasHeader = CsvDataMap.DefaultHasHeader;
//        _appendToFile = CsvDataMap.DefaultAppendToFile;
//        _alertAmount = CsvDataMap.DefaultAlertAmount;
//        _emptyValueAsNull = CsvDataMap.DefaultEmptyValueAsNull;

//    }


//    public InternalCsvDataMap(CsvDataMap existingDataMap) {
//        _delimiter = existingDataMap.Delimiter;
//        _textDelimiter = existingDataMap.TextDelimiter;
//        _appendHeader = existingDataMap.AppendHeader;
//        _hasHeader = existingDataMap.HasHeader;
//        _appendToFile = existingDataMap.AppendToFile;
//        _alertAmount = existingDataMap.AlertAmount;
//        _emptyValueAsNull = existingDataMap.EmptyValueAsNull;

//    }

//}

//#endregion