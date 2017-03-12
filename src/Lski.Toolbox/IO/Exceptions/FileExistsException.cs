using System;
using System.IO;

namespace Lski.Toolbox.IO.Exceptions {

    /// <summary>
    /// Thrown if a file exists an prevents an action.
    /// </summary>
    public class FileExistsException : IOException
    {
        private const string _message = "The file {0} already exists";

        public FileExistsException(string fileName)
            : base(string.Format(_message, fileName)) {}

        public FileExistsException(string fileName, Exception innerException)
            : base(string.Format(_message, fileName), innerException) {}
    }
}