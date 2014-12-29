using System.IO;

namespace Lski.Toolbox.IO {

    /// <summary>
    /// Class that provides several functions used for Input/Output, mainly used for File control
    /// </summary>
    /// <remarks></remarks>
    public class FileSystemInfoFunctions {

        /// <summary>
        /// Creates a FileInfo or DirectoryInfo object depending on the file/directory path passed.
        /// </summary>
        /// <param name="filepath">A filesystem path</param>
        /// <returns>
        /// A FileInfo or DirectoryInfo object as a FileSystemInfo object, is an empty or null string passed nothing is returned
        /// </returns>
        /// <remarks>
        /// Creates a FileInfo or DirectoryInfo object depending on the file/directory path passed.
        ///
        /// First the the path is checked to see if it exists, where it checks the file system attributes, or if it doesnt exist returns
        /// an object depending on whether the file system path contains an extension.
        ///
        /// The path does not have to refer to a currently existing file system object.
        /// </remarks>
        public static FileSystemInfo CreateFsi(string filepath) {

            if (string.IsNullOrEmpty(filepath)) {
                return null;
            }

            FileSystemInfo di = new DirectoryInfo(filepath);

            return di.IsDirectory() ? di : new FileInfo(filepath);
        }

    }

}