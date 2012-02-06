using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Reflection;
using Lski.Txt;

namespace Lski.Logging {

	public class SystemLogger : Logger {

		public static Logger Create() { return new SystemLogger(); }

		public override int WriteEntry(string message, Logger.EventType eventType, int eventID, short categoryID, short maxStackTrace) {

			System.Text.StringBuilder sb = new System.Text.StringBuilder(message).AppendLine();

			StackTrace trace = new StackTrace(true);
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

				sb.Append("[").Append(_txtTitleLine + StringExt.ConvertToString(frame.GetFileLineNumber())).Append("] [").Append(CreateMethodData(m)).AppendLine("]");

				counter++; // Move the counter on to avoid infinite loop
			}

			EventLog el = new EventLog(this.LogName, Environment.MachineName, ApplicationSource);

			Int32 currentId = default(Int32);

			if (eventID < 1) currentId = NextEventID;
			else currentId = eventID;

			el.WriteEntry(message, ConvertEnumType(eventType), currentId, categoryID);

			return currentId;
		}

		public override int WriteEntry(System.Exception ex, Logger.EventType eventType, int eventID, short categoryID, short maxStackTrace) {

			EventLog el = new EventLog(this.LogName, Environment.MachineName, ApplicationSource);
			Int32 currentId = default(Int32);

			if (eventID < 1) currentId = NextEventID;
			else currentId = eventID;

			el.WriteEntry(CreateSystemExceptionMessage(ex, maxStackTrace), ConvertEnumType(eventType), currentId, categoryID);

			return currentId;
		}

		/// <summary>
		/// Creates an exception message designed for the event viewer
		/// </summary>
		/// <param name="ex"></param>
		/// <param name="maxStackTrace"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		private string CreateSystemExceptionMessage(Exception ex, Int16 maxStackTrace) {

			System.Text.StringBuilder sb = new System.Text.StringBuilder(ex.Message).AppendLine();
			sb.Append("[").Append(_txtExceptionType).AppendLine("]");

			StackTrace trace = new StackTrace(true);
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

				sb.Append("[").Append(_txtTitleLine + StringExt.ConvertToString(frame.GetFileLineNumber())).Append("] [").Append(CreateMethodData(m)).AppendLine("]");

				counter++; // Move the counter on to avoid infinite loop
			}

			if (ex.InnerException != null) 
				sb.AppendLine("Inner Exception:").AppendLine(CreateSystemExceptionMessage(ex.InnerException, maxStackTrace));

			return sb.ToString();
		}

	}

}
