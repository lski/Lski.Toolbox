using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lski.IO.FileSystem.Exceptions {

	public class CreateDirectoryException : Exception {
	
		private const string _message = "The directory '{0}' could not be created";

		public CreateDirectoryException(string directoryName) : base(string.Format(_message, directoryName)) { }
		public CreateDirectoryException(string directoryName, Exception innerException) : base(string.Format(_message, directoryName), innerException) { }
	}
}
