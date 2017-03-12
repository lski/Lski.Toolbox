using Lski.Toolbox.IO.Exceptions;
using System;
using System.IO;

namespace Lski.Toolbox.IO
{
    public static class FileInfoExt
    {
        /// <summary>
        /// Creates a new FileInfo object, if an invalid path it returns null.
        ///
        /// It does this by trying to check for invalid path characters prior to creating a new FileInfo, and only then capturing and suppressing any
        /// exceptions thrown by FileInfo object as that should only be a FileNotFound.
        ///
        /// This should be more efficient as creating Exceptions is a heavy process. Most situations that could cause a FileInfo to throw an exceptions
        /// would be caught before attempting to create a FileInfo object.
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static FileInfo CreateSafe(string filepath)
        {
            if (filepath == null)
            {
                throw new ArgumentNullException(nameof(filepath));
            }

            FileInfo fi;

            if (filepath.IndexOfAny(Path.GetInvalidPathChars()) > -1)
            {
                return null;
            }

            try
            {
                fi = new FileInfo(filepath);
            }
            catch (Exception)
            {
                return null;
            }

            return fi;
        }

        /// <summary>
        /// Provides a MoveTo function with the added functionality of an overwrite feature, plus creates the directory structure of the destination if one doesnt exist.
        /// </summary>
        /// <param name="original">The full file name of the file to move in a FileInfo object</param>
        /// <param name="destination">The new location, including the filename</param>
        /// <param name="overwrite">
        /// Whether or not to overwrite any file at the location stated by the destination address. If false and the destination file exists then throws FileExistsException
        /// </param>
        /// <exception cref="FileExistsException"></exception>
        public static void MoveToExtended(this FileInfo original, FileInfo destination, bool overwrite = true)
        {
            if (original == null)
            {
                throw new ArgumentNullException(nameof(original));
            }

            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            // 1. If set that can overwrite and destination exists then delete the existing destination file
            // 2. If the file exists at the destination but not allowed to overwrite it, inform the user by throwing an exception
            // 3. Otherwise try to move, creating the directory
            if (destination.Exists && overwrite)
            {
                destination.Delete();
                original.MoveTo(destination.FullName);
            }
            else if (destination.Exists && !overwrite)
            {
                throw new FileExistsException(original.Name);
            }
            else
            {
                // Check if exists first as access rights might throw exception even if it does exist
                if (!destination.Directory.Exists)
                {
                    destination.Directory.Create();
                }

                original.MoveTo(destination.FullName);
            }
        }

        /// <summary>
        /// Provides an advanced CopyTo function, that also creates the directory structure of the destination file if it doesnt exist.
        /// </summary>
        /// <param name="original">The full file name of the file to move in a FileInfo object</param>
        /// <param name="destination">The new location, including the filename</param>
        /// <param name="overwrite">
        /// Whether or not to overwrite any file at the location stated by the destination address. If false and the destination file exists then throws FileExistsException
        /// </param>
        /// <exception cref="FileExistsException"></exception>
        public static void CopyToExtended(this FileInfo original, FileInfo destination, bool overwrite)
        {
            if (original == null)
            {
                throw new ArgumentNullException("original");
            }

            if (destination == null)
            {
                throw new ArgumentNullException("destination");
            }

            // 1. If set that can overwrite and destination exists then delete the existing destination file
            // 2. If the file exists at the destination but not allowed to overwrite it, inform the user by throwing an exception
            // 3. Otherwise try to copy, creating the directory if not exists if file exists it will throw the same exceptions as FileInfo.CopyTo()
            if (destination.Exists && overwrite)
            {
                destination.Delete();
                original.CopyTo(destination.FullName);
            }
            else if (destination.Exists & (!overwrite))
            {
                throw new FileExistsException(original.Name);
            }
            else
            {
                // Check if exists first as access rights might throw exception even if it does exist
                if (!destination.Directory.Exists)
                {
                    destination.Directory.Create();
                }

                original.CopyTo(destination.FullName);
            }
        }
    }
}