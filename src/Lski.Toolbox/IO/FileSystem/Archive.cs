using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using Lski.Toolbox.IO.FileSystem.Exceptions;

namespace Lski.Toolbox.IO.FileSystem {

	public class Archive : FileFunctions {

		private const string DefaultArchiveDir = "archive";

		#region "Enums"

		public enum Operation {
			/// <summary>
			/// Move file
			/// </summary>
			/// <remarks></remarks>
			Move = 0,
			/// <summary>
			/// Copy file
			/// </summary>
			/// <remarks></remarks>
			Copy = 1
		}

		public enum OverwriteFlag {
			/// <summary>
			/// Attempts to over write the file in the normal manner, see instructions for Fileinfo.CopyTo and
			/// FileInfo.MoveTo for more details
			/// </summary>
			/// <remarks></remarks>
			Normal = 0,
			/// <summary>
			/// Overwrites the file completely
			/// </summary>
			/// <remarks></remarks>
			Overwrite = 1,
			/// <summary>
			/// If a file already exists, then create a new modified filename for the location of the new file
			/// </summary>
			/// <remarks></remarks>
			CreateNew = 2
		}

		#endregion

		#region "Static Methods"

		/// <summary>
		/// Looks for and archives the file passed, either by moving the file or creating a copy of it, into the directory specified.
		/// </summary>
		/// <param name="currentFile">The file to archive</param>
		/// <param name="destinationDir">
		/// The directory to archive into, if null then uses a sub directory from the file being archived called archive
		/// </param>
		/// <param name="moveOption">States whether to move or copy the file</param>
		/// <exception cref="FileNotFoundException">If the file to move is NOT found</exception>
		/// <exception cref="CreateDirectoryException">If there was an error creating the archive directory
		/// </exception>
		/// <remarks></remarks>
		public static string ArchiveFileToDirectory(FileInfo currentFile, DirectoryInfo destinationDir, Operation moveOption = Operation.Move, OverwriteFlag overwriteOption = OverwriteFlag.Overwrite) {

			FileInfo destinationFile = null;
			Char sep = Path.DirectorySeparatorChar;

			// 1. if the destination directory is nothing then use a directory below the current directory
			// 2. If destination is set, then use the currentfile name
			try {

				destinationFile = (destinationDir == null ? (new FileInfo(currentFile.DirectoryName + sep + DefaultArchiveDir + sep + currentFile.Name)) : new FileInfo(destinationDir.FullName + sep + currentFile.Name));

			} catch (Exception ex) {
				throw new CreateDirectoryException(ex.Message);
			}

			return ArchiveFile(currentFile, destinationFile, moveOption, overwriteOption);

		}

		/// <summary>
		/// Looks for the currentFile and attempts to archive the file to the location specified in destinationFile.
		/// </summary>
		/// <param name="currentFile">The file to archive</param>
		/// <param name="destinationFile">The location including the filename to archive the file to.</param>
		/// <param name="moveOption">States whether to move or copy the file</param>
		/// <param name="overwriteOption">States the default behaviour of whether the file exists</param>
		/// <exception cref="FileNotFoundException">If the file to move is NOT found</exception>
		/// <exception cref="CreateDirectoryException">If there was an error creating the archive directory</exception>
		/// <remarks></remarks>
		public static string ArchiveFile(FileInfo currentFile, FileInfo destinationFile, Operation moveOption = Operation.Move, OverwriteFlag overwriteOption = OverwriteFlag.Overwrite) {
			
			char sep = Path.DirectorySeparatorChar;

			// If the current file does not exist then throw an exception
			if (!currentFile.Exists) throw new FileNotFoundException();

			try {
				// If the user has not put anything into archiveFileName then create it as the same name as the currentFileName
				// in a sub directory called 'archive'
				if (destinationFile == null) 
					destinationFile = new FileInfo(currentFile.DirectoryName + sep + "archive" + sep + currentFile.Name);

				// 1. If the file exists and the user doesnt want to overwrite the file, then create a unique version of the file number
				// 2. Otherwise ensure the directory exists for the archive file
				if ((overwriteOption == OverwriteFlag.CreateNew) && (destinationFile.Exists)) {

					destinationFile = (FileInfo)ApplyVersionNumber(destinationFile);

				} else {

					// If the sub directory has not been pre created esnure it has been created
					if ((!destinationFile.Directory.Exists)) destinationFile.Directory.Create();
				}

			} catch (Exception ex) {
				throw new CreateDirectoryException(ex.Message);
			}

			// Try moving/copying the file
			if (moveOption == Operation.Copy) {

				currentFile.CopyTo(destinationFile.FullName, true);
			} 
			else {
				
				currentFile.MoveToExt(destinationFile, true);
			}

			return destinationFile.FullName;

		}

		#endregion

		/// <summary>
		/// Finds the first available file name, by adding a version number within brackets after the name. E.g. 'aFile.txt' already exists 
		/// so 'aFile (1).txt' is returned
		/// </summary>
		/// <param name="fsi">The FileSystemInfo obj containing the filename</param>
		/// <param name="version">The iterations the recursion is at</param>
		/// <param name="versionAtEnd">States whether the varions number should be at the end of the</param>
		/// <returns></returns>
		/// <remarks>
		/// A recursive function that finds the first available file name, by appending the suffix: (aNum) after
		/// the name. E.g. 'aFile.txt' already exists so 'aFile (1).txt' is returned
		/// </remarks>
		private static FileSystemInfo ApplyVersionNumber(FileSystemInfo fsi, Int32 version = 1) {

			string fsiSimpleName = null;
			string suffix = null;
			FileSystemInfo arcFile = null;

			// Get the first part of the file/directory path
			String prefix = Path.GetDirectoryName(fsi.FullName);

			// Depending on whether this is a directory or not collect the correct suffix (also strip the extension)
			if (fsi.IsDirectory()) {
				suffix = Path.DirectorySeparatorChar.ToString();
				fsiSimpleName = Path.GetFileNameWithoutExtension(fsi.Name);
			} else {
				suffix = fsi.Extension;
				fsiSimpleName = Path.GetFileNameWithoutExtension(fsi.Name);
			}

			arcFile = (FileSystemInfo)Activator.CreateInstance(fsi.GetType(), new object[] { prefix + fsiSimpleName + " (" + version + ")" + suffix });

			// Create a new fsi Object (same type as passed)
			return (arcFile.Exists) ? ApplyVersionNumber(fsi, version + 1) : arcFile;
		}

	}

}
