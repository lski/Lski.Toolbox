using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Lski.IO.CsvDataTable {

	/// <summary>
	/// A class designed to store the links for exporting a Csv file from code. Designed mostly for storage in XML in a file.
	/// </summary>
	[DataContract()]
	public class CsvExportMap : CsvDataMap {

		#region "Properties"

		public const bool DefaultAppendToFile = false;
		public const bool DefaultAppendHeader = true;
		public const bool DefaultNullAsEmptyValue = true;

		private bool _appendHeader;
		/// <summary>
		/// States whether to attach a header if possible to the csv file. Default = "True"
		/// </summary>
		/// <remarks></remarks>
		[DataMember(Name = "AppendHeader")]
		public bool AppendHeader {
			get { return _appendHeader; }
			set { _appendHeader = value; }
		}

		private bool _appendToFile;
		/// <summary>
		/// States whether when exporting to the csv file the values are added to the end of the file, rather than using an empty
		/// file. Overrides the setting 'AppendHeader' if the file exists and set to true. (Default = "False")
		/// </summary>
		/// <remarks></remarks>
		[DataMember(Name = "AppendToFile")]
		public bool AppendToFile {
			get { return _appendToFile; }
			set { _appendToFile = value; }
		}

		private bool _NullAsEmptyValue;
		/// <summary>
		/// Gets/Sets whether when an exporting a null value an empty string is exported or 'NULL' Default = True
		/// </summary>
		/// <remarks></remarks>
		[DataMember(Name = "NullAsEmptyValue")]
		public bool NullAsEmptyValue {
			get { return _NullAsEmptyValue; }
			set { _NullAsEmptyValue = value; }
		}

		/// <summary>
		/// The list of links between the source (csv file) and the target
		/// </summary>
		/// <remarks>This list is in fact just an alias for DataMapLinks, it is used so that the ordering can be set</remarks>
		[DataMember(Name = "Links", Order = 100)]
		protected List<DataMapLink> Links {
			get { return this.DataMapLinks; }
			set { this.DataMapLinks = value; }
		}


		#endregion

		public CsvExportMap() : base() {
			this._NullAsEmptyValue = DefaultNullAsEmptyValue;
			this._appendToFile = DefaultAppendToFile;
			this._appendHeader = DefaultAppendHeader;
		}

		/// <summary>
		/// Clones this object
		/// </summary>
		/// <param name="emptySet">If true only clones the base settings, not the full list</param>
		/// <returns></returns>
		/// <remarks></remarks>
		public virtual object Clone() {

			var newObj = new CsvExportMap();

			newObj.Delimiter = this.Delimiter;
			newObj.TextDelimiter = this.TextDelimiter;
			newObj.AlertAmount = this.AlertAmount;
			newObj.AppendHeader = this._appendHeader;
			newObj.AppendToFile = this._appendToFile;
			newObj.NullAsEmptyValue = this.NullAsEmptyValue;

			return newObj;
		}
	}
}
