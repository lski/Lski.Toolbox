using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Reflection;

namespace Lski.Logging {

	/// <summary>
	/// A class designed to allow logging of messages and exceptions to a varitey of formats, using the same interface for each.
	/// </summary>
	/// <remarks>
	/// A class designed to allow logging of messages and exceptions to a varitey of formats, using the same interface for each.
	/// 
	/// The formats supported are standard text files, xml fragment files and the event viewer. The class implements the IXmlSerialiazbale Interface so can be stored
	/// as a setting.
	/// </remarks>
	public abstract class Logger {

		#region "Consts"

		/// <summary>
		/// Simply the default extension for a log file
		/// </summary>
		/// <remarks></remarks>

		public const string DefaultFileExtension = ".log";
		/// <summary>
		/// States the extention filter for openfiledialogs dealing with error log files
		/// </summary>
		/// <remarks></remarks>

		public const string ErrorLogFileFilter = "Log Files (*.log)|*.log|Xml Files (*.xml)|*.xml|All Files (*.*)|*.*";

		protected const string _dateFormat = "yyyy/MM/dd";
		protected const string _timeFormat = "HH:mm:ss";
		protected const string _errorFlag = "Error";
		protected const string _eventFlag = "Event";

		protected const string _txtTitleLine = "Line: ";
		protected const string _txtExceptionType = "Exception Type: ";

		/// <summary>
		/// States the type of the event being logged
		/// </summary>
		/// <remarks>States the type of the event being logged. 
		/// 
		/// It is needed rather than just using the System.Diagnostics.EventLogEntryType, because this causes the Xml.Serializer to break when serializing the
		/// object.
		/// </remarks>
		public enum EventType {

			Warning = System.Diagnostics.EventLogEntryType.Warning,
			Error = System.Diagnostics.EventLogEntryType.Error,
			SuccessAudit = System.Diagnostics.EventLogEntryType.SuccessAudit,
			FailureAudit = System.Diagnostics.EventLogEntryType.FailureAudit,
			Information = System.Diagnostics.EventLogEntryType.Information

		}

		/// <summary>
		/// A list of constant defaults for this class
		/// </summary>
		/// <remarks></remarks>
		public class Defaults {

			public const Int32 EventID = -1;
			public const Int16 CategoryID = 0;
			public const Int16 ErrorStackTrace = 3;
			public const Int16 EventStackTrace = 0;
			public const EventType EventType = Logger.EventType.Information;
			public const string LogName = "Application";
		}

		#endregion

		#region "Properties"

		private static Logger _default = new SystemLogger();
		public static Logger Default {
			get {
				if (_default == null) _default = new SystemLogger();
				return _default;
			}
			set {
				if (value == null)
					_default = new SystemLogger();
				else
					_default = value;
			}
		}

		private static Int32 _currentEventId = 1;
		/// <summary>
		/// Gets the current event ID
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		public static Int32 CurrentEventID {
			get { return _currentEventId; }
		}

		/// <summary>
		/// Gets the next event ID and sets the internal counter to the new value
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		public static Int32 NextEventID {
			get {
				_currentEventId = _currentEventId + 1;
				return _currentEventId;
			}
		}

		private Int16 _errorStackTrace = Defaults.ErrorStackTrace;
		public Int16 ErrorStackTrace {
			get { return _errorStackTrace; }
			set { _errorStackTrace = value; }
		}

		private Int16 _eventStackTrace = Defaults.EventStackTrace;
		public Int16 EventStackTrace {
			get { return _eventStackTrace; }
			set { _eventStackTrace = value; }
		}

		private string _logName = Defaults.LogName;
		/// <summary>
		/// Only used when using the event viewer, states the top level
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		[System.Xml.Serialization.XmlElement("logname")]
		public string LogName {
			get { return _logName; }
			set { _logName = value; }
		}

		/// <summary>
		/// Produces the source name, in the format of the Application name (Read Only)
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		public string ApplicationSource {
			get {
				string appName = "Unknown";

				try {
					appName = System.IO.Path.GetFileNameWithoutExtension(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
				} catch {
					// Stub to catch the error but dont worry about handling it
				}

				return appName;

			}
		}

		#endregion

		#region "Methods"

		/// <summary>
		/// Writes an exception as a message to the format specified in the EntryType Property.
		/// </summary>
		/// <param name="ex">The exception object to log</param>
		/// <param name="eventType">The type of the event, either information, error etc</param>
		/// <remarks>Writes an exception as a message to the format specified in the EntryType Property.</remarks>
		public Int32 WriteEntry(Exception ex, EventType eventType) {

			return WriteEntry(ex, eventType, Defaults.EventID, Defaults.CategoryID, ErrorStackTrace);

		}

		/// <summary>
		/// Writes an exception as a message to the format specified in the EntryType Property.
		/// </summary>
		/// <param name="ex">The exception object to log</param>
		/// <param name="eventType">The type of the event, either information, error etc</param>
		/// <param name="eventID">The eventID stored in the error log, this can be used to identify particular types of errors/events from a program</param>
		/// <remarks>Writes an exception as a message to the format specified in the EntryType Property.</remarks>
		public Int32 WriteEntry(Exception ex, EventType eventType, Int32 eventID) {

			return WriteEntry(ex, eventType, eventID, Defaults.CategoryID, ErrorStackTrace);

		}

		/// <summary>
		/// Writes an exception as a message to the format specified in the EntryType Property.
		/// </summary>
		/// <param name="ex">The exception object to log</param>
		/// <param name="eventType">The type of the event, either information, error etc</param>
		/// <param name="eventID">The eventID stored in the error log, this can be used to identify particular types of errors/events from a program</param>
		/// <param name="categoryID">A category</param>
		/// <remarks>Writes an exception as a message to the format specified in the EntryType Property.</remarks>
		public Int32 WriteEntry(Exception ex, EventType eventType, Int32 eventID, Int16 categoryID) {

			return WriteEntry(ex, eventType, eventID, categoryID, ErrorStackTrace);

		}

		/// <summary>
		/// Writes an exception as a message to the format specified in the EntryType Property.
		/// </summary>
		/// <param name="ex">The exception object to log</param>
		/// <param name="eventType">The type of the event, either information, error etc</param>
		/// <param name="eventID">The eventID stored in the error log, this can be used to identify particular types of errors/events from a program</param>
		/// <param name="categoryID">A category</param>
		/// <param name="maxStackTrace"></param>
		/// <remarks>Writes an exception as a message to the format specified in the EntryType Property.</remarks>
		public abstract Int32 WriteEntry(Exception ex, EventType eventType, Int32 eventID, Int16 categoryID, Int16 maxStackTrace);

		/// <summary>
		/// Writes a message to the format specified in the EntryType Property.
		/// </summary>
		/// <param name="message">The message to log</param>
		/// <param name="eventType">The type of the event, either information, error etc</param>
		/// <remarks>Writes a message to the format specified in the EntryType Property.</remarks>
		public Int32 WriteEntry(string message, EventType eventType) {

			return WriteEntry(message, eventType, Defaults.EventID, Defaults.CategoryID, EventStackTrace);

		}

		/// <summary>
		/// Writes a message to the format specified in the EntryType Property.
		/// </summary>
		/// <param name="message">The message to log</param>
		/// <param name="eventType">The type of the event, either information, error etc</param>
		/// <param name="eventID">The eventID stored in the error log, this can be used to identify particular types of errors/events from a program</param>
		/// <remarks>Writes a message to the format specified in the EntryType Property.</remarks>
		public Int32 WriteEntry(string message, EventType eventType, Int32 eventID) {

			return WriteEntry(message, eventType, eventID, Defaults.CategoryID, EventStackTrace);

		}

		/// <summary>
		/// Writes a message to the format specified in the EntryType Property.
		/// </summary>
		/// <param name="message">The message to log</param>
		/// <param name="eventType">The type of the event, either information, error etc</param>
		/// <param name="eventID">The eventID stored in the error log, this can be used to identify particular types of errors/events from a program</param>
		/// <param name="categoryID">A category</param>
		/// <remarks>Writes a message to the format specified in the EntryType Property.</remarks>
		public Int32 WriteEntry(string message, EventType eventType, Int32 eventID, Int16 categoryID) {

			return WriteEntry(message, eventType, eventID, categoryID, EventStackTrace);

		}

		/// <summary>
		/// Writes a message to the format specified in the EntryType Property.
		/// </summary>
		/// <param name="message">The message to log</param>
		/// <param name="eventType">The type of the event, either information, error etc</param>
		/// <param name="eventID">The eventID stored in the error log, this can be used to identify particular types of errors/events from a program</param>
		/// <param name="categoryID">A category</param>
		/// <param name="maxStackTrace"></param>
		/// <remarks>Writes a message to the format specified in the EntryType Property.</remarks>
		public abstract Int32 WriteEntry(string message, EventType eventType, Int32 eventID, Int16 categoryID, Int16 maxStackTrace);


		#endregion

		#region "Private Methods"

		/// <summary>
		/// Simply converts the enum from the type for this class to the type used in the Event Viewer
		/// </summary>
		/// <param name="et"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
		protected EventLogEntryType ConvertEnumType(Logger.EventType et) {

			Array list = Enum.GetValues(typeof(System.Diagnostics.EventLogEntryType));
			Int32 etAsInt = Convert.ToInt32(et);
			Boolean flugFlag = false;

			foreach (var i in list) {

				if (Convert.ToInt32(i) == (etAsInt)) {
					flugFlag = true;
					break; // TODO: might not be correct. Was : Exit For
				}

			}

			// If found then return that type other wise, return information
			return flugFlag ? (System.Diagnostics.EventLogEntryType)et : EventLogEntryType.Information;

		}

		///' <summary>
		///' Uses the passed method base object to extract a string representation of that method
		///' </summary>
		///' <returns></returns>
		///' <remarks></remarks>
		protected string CreateMethodData(MethodBase method) {

			System.Text.StringBuilder mess = new System.Text.StringBuilder();
			ParameterInfo[] paras = method.GetParameters();

			mess.Append(method.DeclaringType.Name + "." + method.Name);

			// Append the start bracket for parameters even if emtpy
			mess.Append("(");


			if ((paras != null) && paras.Length > 0) {

				foreach (var para in paras) {
					mess.Append(para.ParameterType.Name + " " + para.Name + ", ");
				}

				mess.Remove((mess.Length - 2), 2);

			}

			// Append the closing bracket for parameters even if emtpy
			mess.Append(")");

			return mess.ToString();

		}



		#endregion

	}

}
