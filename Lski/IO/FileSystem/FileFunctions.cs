using Microsoft.VisualBasic;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Deployment.Application;

using Lski.IO.FileSystem.Exceptions;
using Lski.Txt;

namespace Lski.IO.FileSystem {

	/// <summary>
	/// Class that provides several functions used for Input/Output, mainly used for File control
	/// </summary>
	/// <remarks></remarks>
	public class FileFunctions {

		/// <summary>
		/// Provides a full list of file types .Net defines as a DataFile when deploying files, usefull when trying to locate files transferred when deployed.
		/// </summary>
		/// <remarks></remarks>
		public static string[] ProjectDataTypes = new string[] { "xml" };

		/// <summary>
		/// Returns a FileInfo object of the application file desired, regardless of whether this is network deployed Application or not. Note: File does not need to exist.
		/// </summary>
		/// <param name="relativeFileName">The filename of the file, relative to the </param>
		/// <returns></returns>
		/// <remarks></remarks>
		public static FileInfo GetApplicationFileRef(string relativeFileName) {

			// If the relativeFilename is blank or nothing return nothing
			if (String.IsNullOrEmpty(relativeFileName)) 
				return null;

			relativeFileName = relativeFileName.Replace('\\', '/');

			// Clean up the relative file name, so that it starts with a backslash
			if (relativeFileName.StartsWith("../"))
				relativeFileName = relativeFileName.SubStringAdv(3);
			
			else if (relativeFileName.StartsWith("./"))
				relativeFileName = relativeFileName.SubStringAdv(2);
			
			else if (relativeFileName.StartsWith("/"))
				relativeFileName = relativeFileName.SubStringAdv(1);


			// 1. If the application is network deployed (clickonce) then use the deployment directory or dataDirectory as the base

			if (ApplicationDeployment.IsNetworkDeployed) {

				string extension = Path.GetFileNameWithoutExtension(relativeFileName);

				// 1. If the extension matches one of those that .NET considers a dataType then return a fileInfo object based 
				if (ProjectDataTypes.Contains(extension)) {
					return new FileInfo(string.Concat(ApplicationDeployment.CurrentDeployment.DataDirectory, relativeFileName));

				}
			}

			// Only reaches here if not already returned

			// the file must be in relation to the installation directory
			return new FileInfo(string.Concat(InstallDirectory, relativeFileName));
		}

		/// <summary>
		/// Calculates the installation directory in a consistant format for the project
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		public static string InstallDirectory { 
			get { 
				return (AppDomain.CurrentDomain.BaseDirectory.Replace('\\', '/')); 
			}
		}

		/// <summary>
		/// Calculates a refrence to the deployment directory in a consistent format for the application, throws error if not deployed
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		public static string DeploymentDirectory {
			get {
				String uri = ApplicationDeployment.CurrentDeployment.UpdateLocation.LocalPath.Replace('\\', '/');
				return System.Uri.UnescapeDataString(uri.SubStringAdv(0, uri.LastIndexOf('/') + 1));
			}
		}

		/// <summary>
		/// Creates a FileInfo or DirectoryInfo object depending on the file/directory path passed. First the the path is checked to 
		/// see if it exists, where it checks the file system attributes, or if it doesnt exist returns an object depending on whether
		/// the file system path contains 
		/// </summary>
		/// <param name="fileSystemPath">A filesystem path</param>
		/// <returns>A FileInfo or DirectoryInfo object as a FileSystemInfo object, is an empty or null string passed nothing is returned
		/// </returns>
		/// <remarks>Creates a FileInfo or DirectoryInfo object depending on the file/directory path passed. 
		/// 
		/// First the the path is checked to see if it exists, where it checks the file system attributes, or if it doesnt exist returns 
		/// an object depending on whether the file system path contains an extension.
		/// 
		/// The path does not have to refer to a currently existing file system object.
		/// </remarks>
		public static FileSystemInfo CreateFSI(string fileSystemPath) {

			if (string.IsNullOrEmpty(fileSystemPath)) 
				return null;

			DirectoryInfo di = new DirectoryInfo(fileSystemPath);

			// If doesnt exist in the filesystem, then simply check the extension property

			if ((!di.Exists)) {

				if (di.Extension.Length == 0) 
					return di;
				else 
					return new FileInfo(fileSystemPath);
			}

			if ((di.Attributes & FileAttributes.Directory) == FileAttributes.Directory) 
				return di;

			return new FileInfo(fileSystemPath);
		}

		/// <summary>
		/// Resets the position of the passed stream so that it sits at the beginning of the stream.
		/// </summary>
		/// <param name="r">The stream reader to reset</param>
		/// <returns>True on success</returns>
		/// <remarks>Resets the position of the passed string so that it sits at the beginning of the stream.
		/// 
		/// Checks if the stream reader is not null and allows seeking before attempting to reset the stream, if either then
		/// it returns false, otherwise true on success
		/// 
		/// NOTE: Emptys the stream buffer to ensure the reset is read from the correct position immediatly.
		/// </remarks>
		[System.Diagnostics.DebuggerStepThrough()]
		public static bool ResetStream(ref StreamReader r) {

			// If the stream does not support seeking then return false to indicate a failure
			if (r == null || !r.BaseStream.CanSeek)
				return false;

			// Ensure the information read from the stream is from the position selected by seek, if the buffer contains
			// data the information in the buffer will be read before reading from the position set by seek
			r.DiscardBufferedData();

			r.BaseStream.Seek(0, SeekOrigin.Begin);

			return true;
		}

		/// <summary>
		/// Advances the filepointer in the passed StreamReader so that the line desired is next
		/// Line numbers for the file have a base of 1
		/// </summary>
		/// <param name="lineNumber">The line you want to be avaliable by the next call to ReadLine</param>
		/// <param name="reader">The streamreader</param>
		/// <returns></returns>
		/// <remarks>
		/// Advances the filepointer in the passed StreamReader so that the line desired is next
		/// If the streamreader reaches the end of the file before that line is found then it exits
		/// Meaning the result of readline will be nothing
		/// Line numbers for the file have a base of 1
		/// NOTE: Uses a comparison for a ControlChars.Cr or a ControlChars.Lf to determine the new line
		/// </remarks>
		[DebuggerStepThrough()]
		public static StreamReader AdvanceFilePointer(Int32 lineNumber, System.IO.StreamReader reader) {

			// Reduce the line number by one, so that it points at the correct line (zero based)
			lineNumber -= 1;

			long currentAmount = 0;
			Int32 charCode = 0;
			char currentChar = '\0';

			// Check there is still characters to be read in the reader
			if ((reader.Peek() > -1)) 
				charCode = reader.Read();


			while (charCode != -1 && currentAmount < lineNumber) {
				
				currentChar = Convert.ToChar(charCode);

				// 1. If a line feed then add to the count
				// 2. If a carriage return check next value as well to see if line feed, if so, move on again
				// 3. Simply move the reader on to the next char
				if (currentChar == '\n')
					currentAmount += 1;

				else if (currentChar == '\r') {
					currentAmount += 1;

					// because this is a carriage return check if the next value is a line feed, if it is then advance again
					charCode = reader.Read();

					// If there were no more characters then exit loop
					if (charCode == -1)
						break;

					if (Convert.ToChar(charCode) == '\n')
						charCode = reader.Read();

				}
				else {
					charCode = reader.Read();
				}
			}

			return reader;

		}

		/// <summary>
		/// Finds the number of lines in a file, using the open reader passed to it
		/// </summary>
		/// <param name="reader">The streamreader used to cycle through the file looking for newline characters
		/// </param>
		/// <returns>The number of line found</returns>
		/// <remarks>
		/// Uses the passed streamreader to count the amount of times it finds Carriage returns and line feeds.
		/// 
		/// Note: Reads from the current file pointer as its passed into the method
		/// </remarks>
		[System.Diagnostics.DebuggerStepThrough()]
		public static Int32 FileLineCount(System.IO.StreamReader reader) {

			Int32 lineCount = 0;
			Int32 counter = 0;
			char currentChar = '\0';

			// Check there is still characters to be read in the reader
			if (reader.Peek() > -1) {
				counter = reader.Read();
			}


			while (counter != -1) {

				currentChar = Convert.ToChar(counter);

				if (currentChar == '\r' || currentChar == '\n')
					lineCount += 1;

				counter = reader.Read();
			}

			return lineCount;

		}

		/// <summary>
		/// Reads from the current position in the passed stream reader and returns the the amount of new lines it finds stream, before
		/// returning the position to the position before the method call
		/// </summary>
		/// <param name="reader">The streamreader</param>
		/// <returns></returns>
		/// <remarks>
		/// Reads from the current position in the passed stream reader and returns the the amount of new lines it finds stream, before
		/// returning the position to the position before the method call.
		/// 
		/// Checks for newlines by looking for single carriage returns, single line feeds or carriage return and line feed pairs.
		/// </remarks>
		public static long CountNewlines(System.IO.StreamReader reader) {

			long startPosition = reader.BaseStream.Position;
			long currentAmount = 0;
			Int32 charCode = 0;
			char currentChar = '\0';

			// Check there is still characters to be read in the reader
			if ((reader.Peek() > -1)) charCode = reader.Read();


			while (charCode != -1) {

				currentChar = Convert.ToChar(charCode);

				// 1. If a line feed then add to the count
				// 2. If a carriage return check next value as well to see if line feed, if so, move on again
				// 3. Simply move the reader on to the next char
				if (currentChar == '\n') {
					currentAmount += 1;

				} else if (currentChar == '\r') {
					currentAmount += 1;

					// because this is a carriage return check if the next value is a line feed, if it is then advance again
					charCode = reader.Read();

					// If there were no more characters then exit loop
					if (charCode == -1)
						break;

					if (Convert.ToChar(charCode) == '\n')
						charCode = reader.Read();

				} else {
					charCode = reader.Read();
				}

			}

			// Reset the reader position in the stream
			reader.BaseStream.Position = startPosition;

			return currentAmount;

		}
	}

}
