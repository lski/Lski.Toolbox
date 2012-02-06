using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Text.RegularExpressions;
using System.IO;

using Lski.Txt;
using Lski.Txt.Transformations;
using Lski.Txt.ConvertTo;

namespace Lski.IO.Csv {
	
	public class CsvExport : ICsv {

		#region

		/// <summary>
		/// Hidden blank csv export constructor, used for serialising
		/// </summary>
		protected CsvExport() { }

		public CsvExport(string filename, CsvExportMap map) {
			this.Init(filename, map);
		}

		public CsvExport(string filename, string delimiter) {
			throw new NotImplementedException();
			//this.Init(filename, new CsvExportMap() { Delimiter = delimiter });
		}

		public CsvExport(string filename, string delimiter, string textDelimiter) {
			throw new NotImplementedException();
			//this.Init(filename, new CsvExportMap() { Delimiter = delimiter, TextDelimiter = textDelimiter });
		}

		public CsvExport(string filename, string delimiter, string textDelimiter, Boolean appendHeader) {
			throw new NotImplementedException();
			//this.Init(filename, new CsvExportMap() { Delimiter = delimiter, TextDelimiter = textDelimiter, AppendHeader = appendHeader });
		}

		public void Init(string filename, CsvExportMap map) {
			this._Filename = filename;
			this.DataMap = map;
		}

		#endregion

		#region "Properties"

		public CsvExportMap DataMap { get; set; }

		public virtual string NULLValue {
			get { return "NULL"; }
		}

		protected String _Filename;
		public String Filename { get { return _Filename; } }

		#endregion

		#region "Events"

		/// <summary>
		/// Is thrown each time an individual record is exported TO a csv file.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		/// <remarks></remarks>
		public event ExportValueChangedEventHandler ExportValueChanged;
		public delegate void ExportValueChangedEventHandler(object sender, ValueChangedArgs args);

		/// <summary>
		/// Is thrown once an export process has completed, this is useful to users that run these methods under a separate thread
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		/// <remarks>Is thrown once an export process has completed, this is useful to users that run these methods under a separate thread
		/// 
		/// NOTE: Can be caught with the arguement super class TransferCompleteArgs instead if the handler needs to be more generic
		/// </remarks>
		public event ExportFinishedEventHandler ExportFinished;
		public delegate void ExportFinishedEventHandler(object sender, CsvTransferCompleteArgs args);

		#endregion

		#region  "Internal Methods"

		/// <summary>
		/// Gets a file witer for an export to csv file, appendHeader is passed ByRef because if file already exists and the user wants to
		/// append to the end of a file, then they will NOT want another header in the middle of their csv resultSet
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="appendToFile"></param>
		/// <param name="appendHeader">Adding incase it needs to be updated by the method</param>
		/// <returns></returns>
		/// <remarks></remarks>
		protected StreamWriter PrvGetFileWriter(string fileName, ref bool appendHeader, bool appendToFile) {

			FileStream csvStream = null;

			try {
				// 1. If the file does not exist then create it
				// 2. If the user wants this data appended to the end of the file then
				if (appendToFile) {

					// If this set is to be appended to a file (and the file exists) then remove the header
					if (File.Exists(fileName)) appendHeader = false;
					csvStream = new FileStream(fileName, FileMode.Append, FileAccess.Write);

				} else
					csvStream = new FileStream(fileName, FileMode.Create, FileAccess.Write);

				return new StreamWriter(csvStream);

			} catch (Exception ex) {
				throw new Exception("There was an error opening the csv file '" + fileName + "'", ex);
			}

		}

		/// <summary>
		/// Provides a simple method for creating the header line from the datatable
		/// </summary>
		/// <param name="columns">The columns collection desired, usually table.Columns</param>
		/// <returns></returns>
		/// <remarks></remarks>
		protected virtual string ToCsvHeader(CsvDataMap dataMap, List<CsvDataMapLink> dataMapItems) {
		//protected virtual string ToCsvHeader(System.Data.DataColumnCollection columns, CsvDataMap dataMap, List<CsvDataMapItem> dataMapItems) {

			StringBuilder returnStr = new StringBuilder();

			foreach (CsvDataMapLink mapItem in dataMapItems) {
				returnStr.Append(ConvertToCsvValue(mapItem.Target, dataMap.Delimiter, dataMap.TextDelimiter)).Append(dataMap.Delimiter);
			}

			return returnStr.ToString(0, returnStr.Length - dataMap.Delimiter.Length);
		}

		/// <summary>
		/// Provides a simple method for creating a line from the the column collection, row passed and delimiter
		/// </summary>
		/// <param name="row">The row containing values</param>
		/// <param name="columns">The columns collection desired, usually table.Columns</param>
		/// <returns></returns>
		/// <remarks></remarks>
		protected virtual string ToCsvLine(DataRow row, CsvDataMap dataMap, List<CsvDataMapLink> adjustedMapItems) {

			StringBuilder returnStr = new StringBuilder();

			foreach (var mapItem in adjustedMapItems) {

				Object value = row[mapItem.Target];

				if (value.Equals(DBNull.Value))
					returnStr.Append(this.NULLValue).Append(dataMap.Delimiter);
				else
					returnStr.Append(ConvertToCsvValue(value, dataMap.Delimiter, dataMap.TextDelimiter)).Append(dataMap.Delimiter);
			}

			return returnStr.ToString(0, returnStr.Length - dataMap.Delimiter.Length);
		}

		/// <summary>
		/// Takes the data map passed to this import and validates the mappings inside so that they dont break when running
		/// </summary>
		/// <param name="map"></param>
		/// <param name="fields">A list of the fields as the are stored in either the </param>
		/// <returns></returns>
		/// <remarks></remarks>
		protected virtual List<CsvDataMapLink> ValidateMapForExport(CsvExportMap map, DataColumnCollection fields) {

			var validatedLinks = new List<CsvDataMapLink>();

			Int32 i = 0, fieldsLoopEnd = (fields.Count), tmpPosition = -1;

			// Loop through each item in the map to compare it against the header line
			foreach (var mapItem in map.DataMapLinks) {

				// If the source from the data map is not a numeric, then it, the position needs to be gained from matching the target with the datatable field
				Boolean isSourceNumber = Int32.TryParse(mapItem.Source, out tmpPosition);

				// Now check to see if the source was a position and it is outside the range of the fields it can pick from, ignore and go to next mapItem
				if (isSourceNumber && (tmpPosition < 0 || tmpPosition >= fields.Count)) 
					continue;

				// Now check to see if the field is valid in the fields list
				i = 0;

				// See if there field marked in the map matches a field within the table thats being exported
				while (i++ < fieldsLoopEnd) {

					if (fields[i].ColumnName.Equals(mapItem.Target, StringComparison.OrdinalIgnoreCase)) {

						var pos = (isSourceNumber ? tmpPosition : i);
						validatedLinks.Add(new CsvDataMapLink(fields[i].ColumnName, // Use the column name from table is its the correct case for the dataTable
															  (map.AppendHeader ? mapItem.Target : pos.ToString()), // If no header is appended to be appended to the file then mark with position 
															  pos, // The line position to write to
															  (ConvertTo.GetDataMapType(fields[i].DataType)), // Calculated from the data type of the datatable, if issues arise maybe switch to original map type
															  (TransformValues)mapItem.Translations.Clone())); //Any conversions, although on export unlikely to include ANY
					}
				}
			}

			i = 0; // Reinitialise the loop

			// Sort the map into the correct order now to save doing it for every line
			validatedLinks.Sort(delegate(CsvDataMapLink csvDataMapItemOne, CsvDataMapLink csvDataMapItemTwo) { return csvDataMapItemOne.LinePosition.CompareTo(csvDataMapItemTwo); });

			// Now the internal data map has been created, reorder and reassign positions (as they might various positions missing within the set)
			foreach (var mapItem in validatedLinks) {
				mapItem.LinePosition = i++; //Increment after the assignment
			}

			return validatedLinks;
		}

		#endregion

		#region "Public Conversion Methods"

		/// <summary>
		/// Converts the value passed into a csv style value, if the value passed is not a string, then simply return the passed value
		/// </summary>
		/// <param name="value">The value to turn into a csv style value</param>
		/// <param name="delimiter">The delimiter between each value in the line</param>
		/// <returns></returns>
		/// <remarks></remarks>
		public static string ConvertToCsvValue(object value, string delimiter, string textDelimiter) {

			// 1. If the type of the value is boolean, dont convert it to its string version (as it wont be importable back into the db)
			// 2. If not a string type, simply convert it to string and return
			// 3. Finally check string if it requires formatting and return

			if (value is bool)
				return ((bool)value) ? "1" : "0";
			else if (!(value is string))
				return StringExt.ConvertToString(value);

			// Convert and get a handle on the value thats come in
			String str = StringExt.ConvertToString(value);

			// If empty string then return 'as is' to save processing
			if (string.IsNullOrEmpty(str)) return str;

			//' NOW check to see if a need for a text delimiter (regardless of whether there is one then there might be reason to add a default one)

			// 1. If there is a NOT text delimiter then check to see if one is required, if so use a default one, only if one is needed should the default
			//    text delimiter be escaped
			// 2. If there is a text delimiter then escape any text delimiters found, regardless of whether there is NEED for a text delimiter or not, also

			if (string.IsNullOrEmpty(textDelimiter)) {
				// Create a flag to check to see to add a text delimiter needs to be placed around the beginning and end of the value
				bool needsTextDelimiter = false;

				// Actually give a default text delimiter for the absolute musts
				textDelimiter = CsvDataMap.DefaultTextDelimiter;

				// If the value contains a value separator delimiter, then text delimiters HAVE to be added.
				if (Regex.IsMatch(str, "(\\n|\\r|" + delimiter + ")")) needsTextDelimiter = true;

				// Only if a value delimiter is already found within the value and therefore a text delimiter required, the default text delimiter NEEDS to be escaped
				if (needsTextDelimiter && Regex.IsMatch(str, textDelimiter)) {

					// replace each instance of " with ""
					str = Regex.Replace(str, textDelimiter, textDelimiter + textDelimiter);
					needsTextDelimiter = true;
				}

				// Used to check if the last character is whitespace, if it was it would then add text delimiters to the value
				//If (Not needsTextDelimiter) AndAlso Regex.IsMatch(str, "(^\s{1}|\s{1}$)") Then needsTextDelimiter = True

				// If needsDoubleQuotes has been triggered
				if (needsTextDelimiter) str = textDelimiter + str + textDelimiter;


			} else
				str = textDelimiter + Regex.Replace(str, textDelimiter, textDelimiter + textDelimiter) + textDelimiter;

			return str;
		}

		#endregion

		#region "Export Methods"

		/// <summary>
		/// Creates/Appends To a CSV file using the data in the dataset, from the selected table
		/// </summary>
		/// <param name="fileName">The csv file to input the data into</param>
		/// <remarks></remarks>
		public void Export(DataTable table) {

			StreamWriter csvWriter = null;
			Int32 currentRecord = default(Int32);

			if (table == null) 
				throw new ArgumentNullException("To export to CSV the table holding the information to export can not be null");

			Boolean appendHeader = DataMap.AppendHeader;

			// Try to write the csv text to the file
			try {

				// Collect whether the header needs to be appended, which is dependant on whether a file already exists
				csvWriter = PrvGetFileWriter(Filename, ref appendHeader, DataMap.AppendToFile);

				List<CsvDataMapLink> adjustedDataMapItems = ValidateMapForExport(DataMap, table.Columns);

				if (appendHeader)
					csvWriter.WriteLine(ToCsvHeader(DataMap, adjustedDataMapItems)); //csvWriter.WriteLine(ToCsvHeader(table.Columns, map, adjustedDataMapItems));

				// If there is no alert amount, simply go through the increments, to save comparisons
				if (DataMap.AlertAmount < 1) {

					foreach (DataRow row in table.Rows) {
						currentRecord += 1;
						csvWriter.WriteLine(ToCsvLine(row, DataMap, adjustedDataMapItems));
					}

				} else {

					Int64 nextAlert = DataMap.AlertAmount;

					foreach (DataRow row in table.Rows) {

						currentRecord++;
						csvWriter.WriteLine(ToCsvLine(row, DataMap, adjustedDataMapItems));

						// Let the user know each record

						// 1. If the value changed step is 1 then dont do modulus check
						// 2. If not 1 then do a modulus check so that it only fires when the user wants it
						if (nextAlert == currentRecord) {
							// Raise an event to let listners know that another record has been imported
							if (ExportValueChanged != null) 
								ExportValueChanged(this, new ValueChangedArgs(currentRecord, ValueChangedArgs.ValueType.Exported));

							// Now set the next alert amount to check against (saves using Modulus)
							nextAlert = nextAlert + DataMap.AlertAmount;
						}
					}
				}

			} catch (Exception ex) {

				throw new Exception("There was an error writing the CSV text to the file", ex);

			} finally {
				csvWriter.Close();
			}

			// Raise the event to state this export has finished
			if (ExportFinished != null) 
				ExportFinished(this, new CsvTransferCompleteArgs(currentRecord, TransferCompleteArgs.ValueType.Exported, Filename, table.TableName));
		}

		#endregion
	}
}
