using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lski.Data.Connections.Exceptions {

	public class CantConnectException : Exception {

		private const string _message = "Could not connect to the selected data source";

		public CantConnectException(Exception innerException) : base(_message, innerException) { }
		public CantConnectException(string connectionString, Exception innerException) : base(string.Concat(_message, " '", connectionString, "'"), innerException) { }
	}
}
