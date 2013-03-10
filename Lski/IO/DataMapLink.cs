using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Lski.Txt.ConvertTo;
using Lski.Txt.Transformations;
using System.Runtime.Serialization;

namespace Lski.IO {

	[DataContract(Name = "Link")]
	[KnownType("GetKnownTypes")]
	public class DataMapLink {

		#region "Properties"

		public static ConvertTo DefaultDataType { 
			get { return new ToText(); } 
		}

		protected ConvertTo _dataType;
		/// <summary>
		/// States the DataType of the link, from text to numeric to dates
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		[DataMember(Name = "DataType")]
		public ConvertTo DataType {
			get { return _dataType; }
			set { _dataType = value ?? DefaultDataType; }
		}

		protected string _source;
		[DataMember(Name = "Source")]
		public string Source {
			get { return _source; }
			set { _source = value ?? String.Empty; }
		}

		protected string _target;
		[DataMember(Name = "Target")]
		public string Target {
			get { return _target; }
			set { _target = value ?? String.Empty; }
		}

		protected TransformValues _translation;
		[DataMember(Name = "Translations")]
		public TransformValues Translations {
			get { return _translation; }
			set { _translation = value ?? new TransformValues(); }
		}

		#endregion

		// Stub for serialization
		protected DataMapLink() {}

		public DataMapLink(String source, String target) {
			this._source = source;
			this._target = target;
			this._dataType = DefaultDataType;
			this._translation = new TransformValues();
		}

		public DataMapLink(String source, String target, ConvertTo dataType) {
			this._source = source;
			this._target = target;
			this._dataType = dataType;
			this._translation = new TransformValues();
		}

		public DataMapLink(String source, String target, ConvertTo dataType, TransformValues translation) {
			this._source = source;
			this._target = target;
			this._dataType = dataType;
			this._translation = translation;
		}

		public static IEnumerable<Type> GetKnownTypes() {
			return new Type[] { typeof(CsvDataTable.CsvDataMapLink) };
		}
	}

}
