using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Lski.Toolbox.IO.FileSystem.Exceptions {

	/// <summary>
	/// A sub class of IOException used to simiulate in Toolbox.FileSystem.MoveTo the exception thrown when a file exists when using 
	/// fileInfo.CopyTo and the overwrite flag is set to false, because a subclass of IOException it can be caught using the same catch as
	/// CopyTo.
	/// </summary>
	/// <remarks></remarks>
	public class FileMoveException : IOException {
		private const string _message = "You tried to move the file '{0}' to a destination where that file already exists, and overwrite is stated as false";

		public FileMoveException(string fileName) : base(string.Format(_message, fileName)) { }
		public FileMoveException(string fileName, Exception innerException) : base(string.Format(_message, fileName), innerException) { }
	}
}
