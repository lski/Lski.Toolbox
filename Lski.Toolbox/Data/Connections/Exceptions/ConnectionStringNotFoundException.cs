using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lski.Toolbox.Data.Connections.Exceptions {

	public class ConnectionStringNotFoundException : Exception {

		private const string _message = "The connection string with the name '{0}' could not be found";

		public ConnectionStringNotFoundException(string connStringName) : base(string.Format(_message, connStringName)) { }
		public ConnectionStringNotFoundException(string connStringName, Exception innerException) : base(string.Format(_message, connStringName), innerException) { }
	}
}
