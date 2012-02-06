using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Data.Common;
using System.Text;
using Lski;
using Lski.Data.Common;
using System.Runtime.Serialization;

namespace Lski.Data.ORM.DataObjects {

	/// <summary>
	/// Interface for holding together each of the business objects
	/// </summary>
	/// <remarks></remarks>
	[DataContract()]
	public abstract class DataObjectBasic {

		protected DataObjectState _recordState = DataObjectState.Added;
		/// <summary>
		/// Returns the current state of the record
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		public DataObjectState RecordState { get { return _recordState; } }

		/// <summary>
		/// Accepts any changes since the last time the record had acceptchanges called
		/// </summary>
		/// <remarks></remarks>
		public void AcceptChanges() { 
			this._recordState = DataObjectState.Unchanged; 
		}
	}
}
