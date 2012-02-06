using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace Lski.Logging {

	public abstract class FileBasedLogger : Logger {

		#region "Constructors"

		public FileBasedLogger(string logFile) {
			Init(new FileInfo(logFile));
		}

		public FileBasedLogger(FileInfo logFile) {
			Init(logFile);
		}

		protected void Init(FileInfo logFile) {
			_logFile = logFile;
		}

		#endregion

		#region "Properties"

		private FileInfo _logFile;
		public FileInfo LogFile {
			get { return _logFile; }
			set { _logFile = value; }
		}


		#endregion

		/// <summary>
		/// Checks the file passed is valid, throws exceptions if null or the sub directory could not be created
		/// </summary>
		/// <remarks></remarks>

		protected void CheckFile(FileInfo file) {
			// If the logfile property is not set then return null so that the calling method can then write to the event log instead opf the xml file
			if (file == null) {
				throw new ArgumentNullException("To write the log file a filename used for the destination of the log must be made");
			}

			try {
				// Ensure the log file directory exists
				file.Directory.Create();

			} catch (Exception ex) {
				// If an error is created in ensuring the directory is present then write a warning to the event viewer and return Nothing so the calling method can write its message to
				throw new ArgumentException(string.Format("There was an error creating the directory that the destination log file should be located in. '{0}'", file.FullName), ex);

			}

		}

	}

}
