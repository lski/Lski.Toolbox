using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.IO;

namespace Lski.IO.FileSystem {

	/// <summary>
	/// A set of extension methods for the FileSystemInfo class
	/// </summary>
	/// <remarks></remarks>
	public static class FileSystemInfoExt {

		/// <summary>
		/// Accepts any FileSystemInfo object and returns whether it is a directory or not. First checks if exists, if not, 
		/// then checks if there is an extension.
		/// </summary>
		/// <param name="fsi"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public static bool IsDirectory(this FileSystemInfo fsi) {

			if (fsi == null) 
				throw new ArgumentNullException("fsi");

			// If doesnt exist in the filesystem, then simply check the extension property
			if (!fsi.Exists) 
				return ((fsi.Extension.Length == 0) ? true : false);

			return ((fsi.Attributes & FileAttributes.Directory) == FileAttributes.Directory) ? true : false;
		}

		/// <summary>
		/// Accepts any FileSystemInfo object and returns whether it is a object or not. First checks if exists, if not, 
		/// then checks if there is an extension.
		/// </summary>
		/// <param name="fsi"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public static bool IsFile(this FileSystemInfo fsi) {

			if (fsi == null) 
				throw new ArgumentNullException("fsi");

			// If doesnt exist in the filesystem, then simply check the extension property

			if (!fsi.Exists) 
				return ((fsi.Extension.Length == 0) ? false : true);

			return ((fsi.Attributes & FileAttributes.Directory) == FileAttributes.Directory) ? false : true;
		}

	}

}
