using System;
using System.IO;

namespace Lski.Toolbox.IO {

	/// <summary>
	/// Class that provides several functions used for Input/Output, mainly used for File control
	/// </summary>
	/// <remarks></remarks>
	public static class FileSystemInfoFunctions {

		/// <summary>
		/// Accepts a file system path and returns whether it is a file or a directory.
		/// If the path points to something that exists it checks its attributes, otherwise falls back to see if there is an extension.
		/// </summary>
		/// <param name="path">A file system path, which does not have to point to something that already exists</param>
		public static bool IsFile(string path) {

			return !IsDirectory(path);
		}

		/// <summary>
		/// Accepts any <see cref="FileSystemInfo"/> object and returns whether it is a file or a directory.
		/// If the <see cref="FileSystemInfo"/> points to something that exists it checks its attributes, otherwise falls back to see if there is an extension.
		/// </summary>
		/// <param name="fsi">Any <see cref="FileSystemInfo"/>, which does not have to point to something that already exists</param>
		public static bool IsFile(this FileSystemInfo fsi) {

			return !IsDirectory(fsi);
		}

		/// <summary>
		/// Accepts a file system path and returns whether it is a file or a directory.
		/// If the path points to something that exists it checks its attributes, otherwise falls back to see if there is an extension.
		/// </summary>
		/// <param name="path">A file system path, which does not have to point to something that already exists</param>
		public static bool IsDirectory(string path) {

			var fsi = CreateFsi(path);

			return IsDirectory(fsi);
		}

		/// <summary>
		/// Accepts any <see cref="FileSystemInfo"/> object and returns whether it is a file or a directory.
		/// If the <see cref="FileSystemInfo"/> points to something that exists it checks its attributes, otherwise falls back to see if there is an extension.
		/// </summary>
		/// <param name="fsi">Any <see cref="FileSystemInfo"/>, which does not have to point to something that already exists</param>
		public static bool IsDirectory(this FileSystemInfo fsi) {

			if (fsi == null) {
				throw new ArgumentNullException(nameof(fsi));
			}

			if (!fsi.Exists) {
				return fsi.Extension.Length == 0;
			}

			return (fsi.Attributes & FileAttributes.Directory) == FileAttributes.Directory;
		}

		/// <summary>
		/// Creates a <see cref="FileInfo"/> or <see cref="DirectoryInfo"/> object depending on file system path passed. Using <see cref="IsDirectory(FileSystemInfo)"/> to calculate which.
		/// The path does not have to exist
		/// </summary>
		/// <param name="filepath">A filesystem path that refers to a file or a directory</param>
		/// <returns>
		/// A <see cref="FileInfo"/> or <see cref="DirectoryInfo"/> object, if an empty or null path null is returned.
		/// </returns>
		public static FileSystemInfo CreateFsi(string filepath) {

			if (string.IsNullOrEmpty(filepath)) {
				return null;
			}

			FileSystemInfo di = new DirectoryInfo(filepath);

			return IsDirectory(di) ? di : new FileInfo(filepath);
		}
	}
}