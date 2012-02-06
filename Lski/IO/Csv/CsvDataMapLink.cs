using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

using Lski.Txt.Transformations;
using Lski.Txt.ConvertTo;


namespace Lski.IO.Csv {

	/// <summary>
	/// Holds data about each of the links between the csv file and the 
	/// </summary>
	/// <remarks></remarks>
	[DataContract(Name = "Link")]
	public class CsvDataMapLink : DataMapLink {

		#region "Properties"

		private Int32 _linePosition;
		[DataMember()]
		public Int32 LinePosition {
			get { return _linePosition; }
			set { _linePosition = value; }
		}

		#endregion

		public CsvDataMapLink(string source, string target, Int32 linePosition) : base(source, target) {
			this._linePosition = linePosition;
		}

		public CsvDataMapLink(string source, string target, Int32 linePosition, ConvertTo dataMapType) : base(source, target, dataMapType) {
			this._linePosition = linePosition;
		}

		public CsvDataMapLink(string source, string target, Int32 linePosition, ConvertTo dataMapType, TransformValues translation) : base(source, target, dataMapType, translation) {
			this._linePosition = linePosition;
		}

	}
}
