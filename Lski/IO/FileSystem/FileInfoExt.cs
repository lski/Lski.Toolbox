using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Lski.Txt;
using System.IO;
using System.Runtime.CompilerServices;

namespace Lski.IO.FileSystem {

	public static class FileInfoExt {

		/// <summary>
		/// Provides a MoveTo function with the added functionality of an overwrite feature, as found in FileInfo.CopyTo, and also
		/// creates the directory structure of the destination if one doesnt exist, rather than throwing an error.
		/// </summary>
		/// <param name="origFile">The full file name of the file to move in a FileInfo object</param>
		/// <param name="destinationFile">The new location, including the filename</param>
		/// <param name="overwrite">Whether or not to overwrite any file at the location stated byt the destination
		/// address. If false and the destination file exists then throws IOException, like FileInfo.MoveTo does.
		/// </param>
		/// <exception cref="FileMoveException">
		/// This exception is a sub class of the IOException and is used to simulate the CopyTo exception thrown when the file at the
		/// destination exists, but overwrite is false. Because its a sub class of IOException it can be caught in combination with CopyTo
		/// </exception>
		/// <remarks>Provides a MoveTo function with the added functionality of an overwrite feature, as found in FileInfo.CopyTo, and also
		/// creates the directory structure of the destination if one doesnt exist, rather than throwing an error.
		/// 
		/// NOTE: Only provides additional overwrite check for full listing of IOFunctions.MoveTo() see
		/// FileInfo.MoveTo which shows a complete listing of the exceptions throw
		/// 
		/// Finally unlike the MoveTo Methods it DOES NOT edit the original filename, as you have reference to the destination, from the destination passed.
		/// </remarks>
		public static void MoveToExt(this FileInfo origFile, FileInfo destinationFile, bool overwrite = true) {

			// 1. If set that can overwrite and destination exists then delete the existing destination file
			if (destinationFile.Exists && overwrite) {
				System.IO.File.Delete(destinationFile.FullName);
				origFile.MoveTo(destinationFile.FullName);
			} 
			// 2. If the file exists at the destination but not allowed to overwrite it, inform the user by throwing an exception
			else if ((destinationFile.Exists) & (!overwrite)) {
				throw new Exceptions.FileMoveException(origFile.Name);
			} 
			// 3. Otherwise try to move, creating the directory if not exists if file exists it will throw the same exceptions as FileInfo.MoveTo()
			else {
				destinationFile.Directory.Create();
				origFile.MoveTo(destinationFile.FullName);
			}

		}

		/// <summary>
		/// Provides an advanced CopyTo function, that also creates the directory structure of the destination file if one doesnt exist 
		/// rather than throwing an error.
		/// </summary>
		/// <param name="origFile">The full file name of the file to move in a FileInfo object</param>
		/// <param name="destinationFile">The new location, including the filename</param>
		/// <param name="overwrite">Whether or not to overwrite any file at the location stated byt the destination
		/// address. If false and the destination file exists then throws IOException, like FileInfo.MoveTo does.
		/// </param>
		/// <exception cref="FileCopyException">
		/// This exception is a sub class of the IOException and is used to simulate the CopyTo exception thrown when the file at the
		/// destination exists, but overwrite is false. Because its a sub class of IOException it can be caught in combination with CopyTo
		/// </exception>
		/// <remarks>Provides an advanced CopyTo function, that also creates the directory structure of the destination file if one doesnt exist 
		/// rather than throwing an error.
		/// 
		/// NOTE: For additional exceptions thrown, see FileInfo.CopyTo
		///
		/// Finally unlike the CopyTo Methods it DOES NOT edit the original filename, as you have reference to the destination, from the destination passed.
		/// </remarks>
		public static void CopyToExt(this FileInfo origFile, FileInfo destinationFile, bool overwrite) {
			// 1. If set that can overwrite and destination exists then delete the existing destination file
			// 2. If the file exists at the destination but not allowed to overwrite it, inform the user by throwing an exception
			// 3. Otherwise try to copy, creating the directory if not exists if file exists it will throw the same exceptions as 
			// FileInfo.CopyTo()
			if (destinationFile.Exists && overwrite) {
				System.IO.File.Delete(destinationFile.FullName);
				origFile.CopyTo(destinationFile.FullName);
			} else if (destinationFile.Exists & (!overwrite)) {
				throw new Exceptions.FileCopyException(origFile.Name);
			} else {
				destinationFile.Directory.Create();
				origFile.CopyTo(destinationFile.FullName);
			}

		}
	}
}
