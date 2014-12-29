using System;
using System.IO;

namespace Lski.Toolbox.IO {

    /// <summary>
    /// A set of extension methods for the FileSystemInfo class
    /// </summary>
    /// <remarks></remarks>
    public static class FileSystemInfoExtensions {

        /// <summary>
        /// Accepts any FileSystemInfo object and returns whether it is a directory or not. If its exists it checks the attributes, otherwise
        /// fallsback to see if there is an extension for this fsi or not.
        /// </summary>
        public static bool IsDirectory(this FileSystemInfo fsi) {

            if (fsi == null) {
                throw new ArgumentNullException("fsi");
            }

            // If doesnt exist in the filesystem, then simply check the extension property

            if (!fsi.Exists) {
                return fsi.Extension.Length == 0;
            }

            return (fsi.Attributes & FileAttributes.Directory) == FileAttributes.Directory;
        }

        /// <summary>
        /// Accepts any FileSystemInfo object and returns whether it is a directory or not. If its exists it checks the attributes, otherwise
        /// fallsback to see if there is an extension for this fsi or not.
        /// </summary>
        public static bool IsFile(this FileSystemInfo fsi) {

            return !IsDirectory(fsi);
        }

    }

}