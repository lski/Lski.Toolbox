using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lski.IO.CsvDataTable {

	public class CsvTransferCompleteArgs : TransferCompleteArgs {

		private string _fileName;

		private string _tableName;
		/// <summary>
		/// Creates a new event arguement object which contains the information raised by the event.
		/// </summary>
		/// <param name="quantity">The amount imported/exported during the event</param>
		/// <param name="type">States the type of value change, either imported or exported</param>
		/// <remarks></remarks>
		public CsvTransferCompleteArgs(Int32 quantity, ValueType type, string fileName, string tableName) : base(quantity, type) {
			this._fileName = fileName;
			this._tableName = tableName;
		}

		/// <summary>
		/// Gets the csv filename that is being worked with.
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		public string FileName {
			get { return _fileName; }
		}

		/// <summary>
		/// Gets the name of the database (dataTable) table that the csv file is interacting with.
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		public string TableName {
			get { return _tableName; }
		}

	}
}
