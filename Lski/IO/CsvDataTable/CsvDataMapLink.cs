using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

using Lski.Txt.Transformations;
using Lski.Txt.Conversion;


namespace Lski.IO.CsvDataTable {

	/// <summary>
	/// Holds data about each of the links between the csv file and the 
	/// </summary>
	/// <remarks></remarks>
	[DataContract(Name = "Link")]
	public class CsvDataMapLink : DataMapLink {

		#region "Properties"

		[DataMember()]
		public Int32 LinePosition { get; set; }

		#endregion

		public CsvDataMapLink(string source, string target, Int32 linePosition) : base(source, target) {
			this.LinePosition = linePosition;
		}

		public CsvDataMapLink(string source, string target, Int32 linePosition, ConvertTo dataMapType) : base(source, target, dataMapType) {
			this.LinePosition = linePosition;
		}

		public CsvDataMapLink(string source, string target, Int32 linePosition, ConvertTo dataMapType, Transformations translation) : base(source, target, dataMapType, translation) {
			this.LinePosition = linePosition;
		}

	}
}
