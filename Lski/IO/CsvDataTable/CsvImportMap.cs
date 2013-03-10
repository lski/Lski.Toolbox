using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Lski.IO.CsvDataTable {

	/// <summary>
	/// A class designed to store the links for importing a Csv file into code. Designed mostly for storage in XML in a file.
	/// </summary>
	[DataContract()]
	public class CsvImportMap<T> : CsvDataMap {

		#region "Properties"

		public const bool DefaultHasHeader = true;
		public const bool DefaultEmptyValueAsNull = true;

		/// <summary>
		/// States whether the csv file has a header or not, if stated as not but it in fact does, the header row will be imported
		/// in the same way as the other rows. However it does not take out into datatypes and might in fact break on import.
		/// Default = "True"
		/// </summary>
		/// <remarks></remarks>
		[DataMember(Name = "HasHeader")]
		public bool HasHeader { get; set; }

		/// <summary>
		/// Gets/Sets whether when an empty string is found, if an empty string is returned, or null. Default = True
		/// </summary>
		/// <remarks></remarks>
		[DataMember(Name = "EmptyValueAsNull")]
		public bool EmptyValueAsNull { get; set; }

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

		public CsvImportMap()
			: base() {
			this.HasHeader = DefaultHasHeader;
			this.EmptyValueAsNull = DefaultEmptyValueAsNull;
		}

		/// <summary>
		/// Clones this object
		/// </summary>
		/// <param name="emptySet">If true only clones the base settings, not the full list</param>
		/// <returns></returns>
		/// <remarks></remarks>
		public virtual object Clone() {

			var newObj = new CsvImportMap();

			newObj.Delimiter = this.Delimiter;
			newObj.TextDelimiter = this.TextDelimiter;
			newObj.HasHeader = this.HasHeader;
			newObj.AlertAmount = this.AlertAmount;
			newObj.EmptyValueAsNull = this.EmptyValueAsNull;

			return newObj;
		}


	}

	/// <summary>
	/// A class designed to store the links for importing a Csv file into code. Designed mostly for storage in XML in a file.
	/// </summary>
	[DataContract()]
	public class CsvImportMap : CsvDataMap {

		#region "Properties"

		public const bool DefaultHasHeader = true;
		public const bool DefaultEmptyValueAsNull = true;

		/// <summary>
		/// States whether the csv file has a header or not, if stated as not but it in fact does, the header row will be imported
		/// in the same way as the other rows. However it does not take out into datatypes and might in fact break on import.
		/// Default = "True"
		/// </summary>
		/// <remarks></remarks>
		[DataMember(Name = "HasHeader")]
		public bool HasHeader { get; set; }

		/// <summary>
		/// Gets/Sets whether when an empty string is found, if an empty string is returned, or null. Default = True
		/// </summary>
		/// <remarks></remarks>
		[DataMember(Name = "EmptyValueAsNull")]
		public bool EmptyValueAsNull { get; set; }

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

		public CsvImportMap() : base() {
			this.HasHeader = DefaultHasHeader;
			this.EmptyValueAsNull = DefaultEmptyValueAsNull;
		}

		/// <summary>
		/// Clones this object
		/// </summary>
		/// <param name="emptySet">If true only clones the base settings, not the full list</param>
		/// <returns></returns>
		/// <remarks></remarks>
		public virtual object Clone() {
	
			var newObj = new CsvImportMap();

			newObj.Delimiter = this.Delimiter;
			newObj.TextDelimiter = this.TextDelimiter;
			newObj.HasHeader = this.HasHeader;
			newObj.AlertAmount = this.AlertAmount;
			newObj.EmptyValueAsNull = this.EmptyValueAsNull;

			return newObj;
		}

		
	}
}
