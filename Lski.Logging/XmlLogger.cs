using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Lski.Txt;
using Lski.IO.FileSystem.Exceptions;
using System.IO;
using System.Xml;
using System.Reflection;

namespace Lski.Logging {

	public class XmlLogger : FileBasedLogger {

		protected const string _nodeNameLog = "Log";
		protected const string _nodeNameMethod = "Method";
		protected const string _nodeNameExceptionName = "ExceptionType";
		protected const string _nodeNameDesc = "Desc";
		protected const string _nodeAttrLine = "Line";
		protected const string _nodeAttrLogType = "LogType";
		protected const string _objTypeAttr = "TypeName";
		protected const string _nodeAttrDate = "Date";
		protected const string _nodeAttrTime = "Time";
		protected const string _nodeAttrEventID = "EventID";

		protected const string _nodeAttrCategoryID = "CategoryID";

		public static Logger Create(string fileName) {return new XmlLogger(fileName);}
		public static Logger Create(FileInfo fileName) {return new XmlLogger(fileName);}

		#region "Constructors"

		public XmlLogger(string logFile)	: this(new FileInfo(logFile)) {}
		public XmlLogger(FileInfo logFile)	: base(logFile) {}

		#endregion

		/// <summary>
		/// Writes an exception as a message to the format specified in the EntryType Property.
		/// </summary>
		/// <param name="ex">The exception object to log</param>
		/// <param name="eventType">The type of the event, either information, error etc</param>
		/// <param name="eventID">The eventID stored in the error log, this can be used to identify particular types of errors/events from a program</param>
		/// <param name="categoryID">A category</param>
		/// <param name="maxStackTrace"></param>
		/// <remarks>Writes an exception as a message to the format specified in the EntryType Property.</remarks>
		public override Int32 WriteEntry(Exception ex, EventType eventType, Int32 eventID, Int16 categoryID, Int16 maxStackTrace) {

			XmlWriter writer = null;

			try {
				// If the xml writer causes an error we want it to bubble up
				writer = GetXmlWriter();

				return ExportXmlException(writer, ex, eventType, eventID, categoryID, maxStackTrace);

			} finally {

				if (writer != null) writer.Close();
			}

		}

		public override int WriteEntry(string message, Logger.EventType eventType, int eventID, short categoryID, short maxStackTrace) {

			XmlWriter writer = null;
			Int32 currentId = default(Int32);

			if (eventID < 1) 
                currentId = NextEventID;
			else			 
                currentId = eventID;

			try {
				writer = GetXmlWriter();

				// A a writer could not be created then write the event to the event viewer instead
				if (writer == null) 
					SystemLogger.Create().WriteEntry(message, eventType, eventID, categoryID, maxStackTrace);
				
				// Open log element
				writer.WriteStartElement(_nodeNameLog);

				writer.WriteAttributeString(_nodeAttrLogType, _eventFlag);
				writer.WriteAttributeString(_nodeAttrDate, System.DateTime.Now.ToString(_dateFormat));
				writer.WriteAttributeString(_nodeAttrTime, System.DateTime.Now.ToString(_timeFormat));
				writer.WriteAttributeString(_nodeAttrEventID, StringExt.ConvertToString(currentId));
				writer.WriteAttributeString(_nodeAttrCategoryID, StringExt.ConvertToString(categoryID));

				writer.WriteElementString(_nodeNameDesc, message);

				// Print method trace
				StackTrace trace = new StackTrace(true);
				StackFrame[] frames = trace.GetFrames() ?? new StackFrame[0];
				Int32 counter = 0;

				foreach (var frame in frames) {
					
					// If reached the max amount of methods then exit
					if(counter == maxStackTrace)
						break;

					MethodBase m = frame.GetMethod();

					// If this is in fact simply a method inside the logger, then move to the next method
					if (typeof(Logger).IsAssignableFrom(m.ReflectedType))
						continue;

					writer.WriteStartElement(_nodeNameMethod);
					writer.WriteAttributeString(_nodeAttrLine, StringExt.ConvertToString(frame.GetFileLineNumber()));
					writer.WriteString(CreateMethodData(m));
					writer.WriteEndElement();
					
					counter++; // Move the counter on to avoid infinite loop
				}

				// Close the log 
				writer.WriteEndElement();
				writer.WriteWhitespace(Environment.NewLine);

				writer.Flush();

			} finally {
				if (writer != null) writer.Close();
			}

			return currentId;

		}

		/// <summary>
		/// Returns an XmlWriter that can be used to write to the log files.
		/// </summary>
		/// <remarks>Returns an XmlWriter that can be used to write to the log files.
		/// 
		/// The writer is created with the ability to close the underlying filestream
		/// but also without producing the doctype tag.
		/// </remarks>
		private XmlWriter GetXmlWriter() {

			this.CheckFile(this.LogFile);

			FileStream fs = null;
			XmlWriter xmlWriter = null;
			XmlWriterSettings settings = new XmlWriterSettings();

			settings.OmitXmlDeclaration = true;
			settings.Indent = true;
			settings.CloseOutput = true;
			settings.ConformanceLevel = ConformanceLevel.Fragment;

			try {
				fs = new FileStream(this.LogFile.FullName, FileMode.Append, FileAccess.Write);

				if (!fs.CanWrite) {
					SystemLogger.Create().WriteEntry(new Exception("The file '" + this.LogFile.FullName + "' is not writable"), EventType.Warning, 1, Convert.ToInt16(1), 0);
					return null;
				}

				xmlWriter = XmlTextWriter.Create(fs, settings);
				return xmlWriter;

			} catch {
				if (xmlWriter != null) 
					xmlWriter.Close();
			}

			return null;

		}

		/// <summary>
		/// Used to state when a file is not writable, where the only option left is to throw an exception.
		/// </summary>
		/// <remarks></remarks>
		public class FileNotWriteableException : Exception {
			private const string _message = "The file '{0}' is not writable";

			public FileNotWriteableException(string fileName) : base(string.Format(_message, fileName)) { }
			public FileNotWriteableException(string fileName, Exception innerException) : base(string.Format(_message, fileName), innerException) { }
		}

		/// <summary>
		/// Creates an exception message for writing to the desired device
		/// </summary>
		/// <param name="ex"></param>
		/// <remarks></remarks>
		protected Int32 ExportXmlException(XmlWriter writer, Exception ex, EventType type, Int32 eventID, Int16 categoryID, Int16 maxStackTrace) {

			try {

				// If xmlwriter is set then use that one, otherwise create a new one
				if (writer == null) return -1;

				Int32 currentId = default(Int32);

				if (eventID < 1) currentId = NextEventID;
				else currentId = eventID;

				// Open log element
				writer.WriteStartElement(_nodeNameLog);

				writer.WriteAttributeString(_nodeAttrLogType, _errorFlag);
				writer.WriteAttributeString(_nodeAttrDate, System.DateTime.Now.ToString(_dateFormat));
				writer.WriteAttributeString(_nodeAttrTime, System.DateTime.Now.ToString(_timeFormat));
				writer.WriteAttributeString(_nodeAttrEventID, StringExt.ConvertToString(currentId));
				writer.WriteAttributeString(_nodeAttrCategoryID, StringExt.ConvertToString(categoryID));

				writer.WriteElementString(_nodeNameExceptionName, ex.GetType().Name);
				writer.WriteElementString(_nodeNameDesc, ex.Message);

				// Print method trace
				StackTrace trace = new StackTrace(ex, true);
				StackFrame[] frames = trace.GetFrames() ?? new StackFrame[0];
				Int32 counter = 0;

				foreach (var frame in frames) {

					// If reached the max amount of methods then exit
					if (counter == maxStackTrace)
						break;

					MethodBase m = frame.GetMethod();

					// If this is in fact simply a method inside the logger, then move to the next method
					if (typeof(Logger).IsAssignableFrom(m.ReflectedType))
						continue;

					writer.WriteStartElement(_nodeNameMethod);
					writer.WriteAttributeString(_nodeAttrLine, StringExt.ConvertToString(frame.GetFileLineNumber()));
					writer.WriteString(CreateMethodData(m));
					writer.WriteEndElement();

					counter++; // Move the counter on to avoid infinite loop
				}

				// If there is an inner exception set for this exception, add it, with an additional offset
				if (ex.InnerException != null) 
					ExportXmlException(writer, ex.InnerException, type, eventID, categoryID, maxStackTrace);

				// Close the log 
				writer.WriteEndElement();
				writer.WriteWhitespace(Environment.NewLine);

				writer.Flush();

				return currentId;

			} catch (Exception ex2) {
				SystemLogger.Create().WriteEntry(ex2, EventType.Warning);
			}

			return 0;

		}

	}

}
