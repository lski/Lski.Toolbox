using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Lski.Misc {

	/// <summary>
	/// Simple functions for working with processes
	/// </summary>
	public static class Process {

		/// <summary>
		/// Runs an external program as a system process, using the command line text passed and waits until the external program
		/// has finished before continuing. By Default it runs the process as a hidden program.
		/// </summary>
		/// <param name="processCommand"></param>
		/// <param name="windowBehaviour"></param>
		/// <remarks></remarks>
		[System.Diagnostics.DebuggerStepperBoundary()]
		public static void RunToComplete(string processCommand, ProcessWindowStyle windowBehaviour = ProcessWindowStyle.Hidden) {
			System.Diagnostics.Process process = null;

			try {
				process = new System.Diagnostics.Process();
			} catch (Exception ex) {
				throw new Exception("There was an error creating a new process object", ex);
			}

			try {
				process.StartInfo.FileName = processCommand;
				process.StartInfo.WindowStyle = windowBehaviour;
				process.Start();

				// Wait until the process passes back an exit code 
				process.WaitForExit();
			} catch (Exception ex) {
				throw new Exception("There was an error when running the process " + processCommand, ex);
			} finally {
				// Free resources associated with this process
				process.Close();
			}

		}
	}
}
