using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace Lski.Logging {

	public class RawTextLogger : FileBasedLogger {

		public static Logger Create(string fileName) { return new RawTextLogger(fileName); }
		public static Logger Create(FileInfo fileName) { return new RawTextLogger(fileName); }

		#region "Constructors"

		public RawTextLogger(string logFile) : this(new FileInfo(logFile)) {}
		public RawTextLogger(FileInfo logFile) : base(logFile) {}

		#endregion

		public override int WriteEntry(string message, Logger.EventType eventType, int eventID, short categoryID, short maxStackTrace) {

			try {
				this.CheckFile(this.LogFile);
				File.AppendAllText(this.LogFile.FullName, message + Environment.NewLine);

			} catch {
				//Stub to suppress error
			}

			return -1;
		}

		public override int WriteEntry(System.Exception ex, Logger.EventType eventType, int eventID, short categoryID, short maxStackTrace) {

			try {

				this.CheckFile(this.LogFile);
				File.AppendAllText(this.LogFile.FullName, ex.Message + Environment.NewLine);

			} catch {
				//Stub to suppress error
			}

			return -1;
		}

	}

}
