using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Deployment;
using System.Xml.Serialization;
using System.Runtime.Serialization;


namespace Lski.IO.CsvDataTable {

	/// <summary>
	/// A class designed to map the the way a csv file links with items in code, either a data table or IDataReader. Needs to be overwritten in classes for either import or export.
	/// </summary>
	[DataContract()]
	public abstract class CsvDataMap : DataMap {

		#region "Properties"

		public const string DefaultDelimiter = ",";
		public const string DefaultTextDelimiter = "\"";
		public const Int32 DefaultAlertAmount = 10;

		/// <summary>
		/// The string used to separate each value in the line. Default = ","
		/// </summary>
		/// <remarks></remarks>
		[DataMember(Name = "Delimiter", Order=1)]
		public string Delimiter { get; set; }

		/// <summary>
		/// States the delimiter used to say a value is a string or not. Usually just a double quote or a single quote.
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		[DataMember(Name = "TextDelimiter", Order=2)]
		public string TextDelimiter { get; set; }

		/// <summary>
		/// States the value for the amount of records between each fire of ValueChangedEvent during import/export. If Less than 1, no event is raised. Default = 10
		/// </summary>
		/// <remarks></remarks>
		[DataMember(Name = "AlertAmount", Order=3)]
		public Int32 AlertAmount { get; set; }

		#endregion

		public CsvDataMap() : base() {
			this.AlertAmount = DefaultAlertAmount;
			this.TextDelimiter = DefaultTextDelimiter;
			this.Delimiter = DefaultDelimiter;
		}
	}

}