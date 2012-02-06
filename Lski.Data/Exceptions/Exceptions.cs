using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

namespace Lski.Data.Exceptions {

	

	public class PrimaryKeyAlreadyExistsException : Exception {

		private const string _message = "The record in {1} could not be updated/inserted as the primary key values {0} already exist";

		public PrimaryKeyAlreadyExistsException(String tableName, String primaryKeyValues) : base(string.Format(_message, primaryKeyValues, tableName)) {}
		public PrimaryKeyAlreadyExistsException(String tableName, String primaryKeyValues, Exception innerException) : base(string.Format(_message, primaryKeyValues, tableName), innerException) {}

	}

	public class UniqueValueExistsException : Exception {

		private const string _message = "The record in {0} could not be updated/inserted as the value(s) {2} already exists in unique column(s) {1}";

		public UniqueValueExistsException(String tableName, String columns, String values) : base(string.Format(_message, tableName, columns, values)) { }
		public UniqueValueExistsException(String tableName, String columns, String values, Exception innerException) : base(string.Format(_message, tableName, columns, values), innerException) {}
	}

	public class RecordNotFoundException : Exception {

		private const string _message = "The record desired from the table {0} could not be found using value(s) {1}";

		public RecordNotFoundException(String tableName, String values) : base(string.Format(_message, tableName, values)) {}
		public RecordNotFoundException(String tableName, String values, Exception innerException) : base(string.Format(_message, tableName, values), innerException) { }
	}

	public class TableColumnNotFoundException : Exception {

		private const string _message = "Column {0} not found in the table {1}";

		public TableColumnNotFoundException(string columnName, string tableName) : base(string.Format(_message, columnName, tableName)) {}
		public TableColumnNotFoundException(string columnName, string tableName, Exception innerException) : base(string.Format(_message, columnName, tableName), innerException) {}

	}

	public class TableNotFoundException : Exception {

		private const string _message = "The desired '{0}' not found in database";

		public TableNotFoundException(string tableName) : base(string.Format(_message, tableName)) {}
		public TableNotFoundException(string tableName, Exception innerException) : base(string.Format(_message, tableName), innerException) {}
	}

	
}
