using System;
using System.Diagnostics;
using System.IO;

namespace Lski.Toolbox.IO
{
    public static class StreamReaderExtensions
    {
        /// <summary>
        /// Resets the position of the passed stream so that it sits at the beginning of the stream.
        /// </summary>
        /// <remarks>
        /// Resets the position of the passed string so that it sits at the beginning of the stream.
        ///
        /// Checks if the stream reader is not null and allows seeking before attempting to reset the stream, if either then
        /// it returns false, otherwise true on success
        ///
        /// NOTE: Emptys the stream buffer to ensure the reset is read from the correct position immediately.
        /// </remarks>
        [DebuggerStepThrough]
        public static bool ResetStream(this StreamReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            // If the stream does not support seeking then return false to indicate a failure
            if (!reader.BaseStream.CanSeek)
            {
                return false;
            }

            // If the buffer contains data the information in the buffer will be read before reading from the position set by seek
            reader.DiscardBufferedData();

            reader.BaseStream.Seek(0, SeekOrigin.Begin);

            return true;
        }

        /// <summary>
        /// Advances the filepointer in the passed StreamReader so that the line desired is next position in the stream.
        /// Note: Line numbers for the file have a base of 1
        /// </summary>
        [DebuggerStepThrough]
        public static StreamReader AdvancePointer(this StreamReader reader, Int32 lineNumber)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            if (lineNumber < 1)
            {
                throw new ArgumentException(nameof(lineNumber) + " can not be less than 1");
            }

            // Reduce the line number by one, so that it points at the correct line (zero based)
            lineNumber -= 1;

            var currentAmount = 0;
            var charCode = 0;

            // Check there is still characters to be read in the reader without advancing it
            if (reader.Peek() > -1)
            {
                charCode = reader.Read();
            }

            while (charCode != -1 && currentAmount < lineNumber)
            {
                var currentChar = Convert.ToChar(charCode);

                // 1. If a line feed then add to the count
                // 2. If a carriage return check next value as well to see if line feed to clear it off and add to the count
                // 3. Simply move the reader on to the next char and loop again
                if (currentChar == '\n')
                {
                    currentAmount += 1;

                    if (currentAmount == lineNumber)
                    {
                        // we have reached enough line numbers, exit and return
                        break;
                    }
                }
                else if (currentChar == '\r')
                {
                    currentAmount += 1;

                    // because this is a carriage return check if the next value is a line feed, if it is then advance again
                    charCode = reader.Read();

                    // If there were no more characters then exit loop
                    if (charCode == -1)
                    {
                        break;
                    }

                    if (Convert.ToChar(charCode) == '\n')
                    {
                        charCode = reader.Read();
                    }

                    if (currentAmount == lineNumber)
                    {
                        // we have reached enough line numbers and cleared the line feed, exit and return
                        break;
                    }
                }
                else
                {
                    charCode = reader.Read();
                }
            }

            return reader;
        }

        /// <summary>
        /// Reads from the current position in the passed stream reader and returns the the amount of new lines it finds. Optionally returns the
        /// stream to its original position.
        /// </summary>
        public static long CountNewlines(this StreamReader reader, bool resetPosition = true)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            var startPosition = resetPosition ? reader.BaseStream.Position : 0;
            var currentAmount = 0;
            var charCode = 0;

            // Check there is still characters to be read in the reader
            if (reader.Peek() > -1)
            {
                charCode = reader.Read();
            }

            while (charCode != -1)
            {
                var currentChar = Convert.ToChar(charCode);

                // 1. If a line feed then add to the count
                // 2. If a carriage return check next value as well to see if line feed, if so, move on again
                // 3. Simply move the reader on to the next char
                if (currentChar == '\n')
                {
                    currentAmount += 1;
                }
                else if (currentChar == '\r')
                {
                    currentAmount += 1;

                    // because this is a carriage return check if the next value is a line feed, if it is then advance again
                    charCode = reader.Read();

                    // If there were no more characters then exit loop
                    if (charCode == -1)
                    {
                        break;
                    }

                    if (Convert.ToChar(charCode) == '\n')
                    {
                        charCode = reader.Read();
                    }
                }
                else
                {
                    charCode = reader.Read();
                }
            }

            if (resetPosition)
            {
                reader.BaseStream.Position = startPosition;
            }

            return currentAmount;
        }
    }
}