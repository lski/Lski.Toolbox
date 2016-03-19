using System;
using System.IO;

namespace Lski.Toolbox.IO {

	[Obsolete("Use FileSystemInfoFunctions instead")]
	public static class FileSystemInfoExtensions {

		[Obsolete("Use FileSystemInfoFunctions instead")]
		public static bool IsDirectory(this FileSystemInfo fsi) {

			return FileSystemInfoFunctions.IsDirectory(fsi);
		}

		[Obsolete("Use FileSystemInfoFunctions instead")]
		public static bool IsFile(this FileSystemInfo fsi) {

			return FileSystemInfoFunctions.IsFile(fsi);
		}
	}
}