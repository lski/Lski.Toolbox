using Lski.Toolbox.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Lski.Toolbox.Txt
{
    /// <summary>
    /// A selection of extension methods for String objects
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Takes the passed string and either truncates the string or pads it with spaces (Default pads to the right)
        /// </summary>
        /// <param name="str">The str to turn to fixed length</param>
        /// <param name="size">The size of the fixed length string</param>
        /// <returns>A fixed length string, padded with spaces</returns>
        /// <remarks></remarks>
        [DebuggerStepThrough]
        public static string Fixed(this string str, Int32 size)
        {
            // If the passed size is larger, pad the string
            // Otherwise truncates the string down in size
            if (str == null)
            {
                return "".PadRight(size);
            }

            if (size > str.Length)
            {
                return str.PadRight(size);
            }

            if (str.Length > size)
            {
                return str.Substring(0, size);
            }

            return str;
        }

        /// <summary>
        /// Takes the passed string and either truncates the string or pads it with the specified Char. (Default pads to the right)
        /// </summary>
        /// <param name="str">The str to turn to fixed length</param>
        /// <param name="size">The size of the fixed length string</param>
        /// <param name="charToPad">The char to use to pad the string</param>
        /// <returns>A fixed length string, padded with the passed char</returns>
        [DebuggerStepThrough]
        public static string Fixed(this string str, Int32 size, char charToPad)
        {
            // If the passed size is larger, pad the string
            // Otherwise truncates the string down in size
            if (str == null)
            {
                return "".PadRight(size, charToPad);
            }

            if (size > str.Length)
            {
                return str.PadRight(size, charToPad);
            }

            if (str.Length > size)
            {
                return str.Substring(0, size);
            }

            return str;
        }

        /// <summary>
        /// Takes the passed string and either truncates the string or pads it with the specified Char, in the direction requested
        /// </summary>
        /// <param name="str">The str to turn to fixed length</param>
        /// <param name="size">The size of the fixed length string</param>
        /// <param name="charToPad">The char to use to pad the string</param>
        /// <param name="padLeft">If true states the padding should be before the string on the left</param>
        /// <returns>A fixed length string, padded with the passed char</returns>
        [DebuggerStepThrough]
        public static string Fixed(this string str, Int32 size, char charToPad, bool padLeft)
        {
            // If the passed size is larger, pad the string
            // Otherwise truncates the string down in size
            if (str == null)
            {
                if (padLeft)
                {
                    return "".PadLeft(size, charToPad);
                }

                return "".PadRight(size, charToPad);
            }

            if (size > str.Length)
            {
                if (padLeft)
                {
                    return str.PadLeft(size, charToPad);
                }

                return str.PadRight(size, charToPad);
            }

            if (str.Length > size)
            {
                return str.Substring(0, size);
            }

            return str;
        }

        /// <summary>
        /// Truncates the passed string to the maxLength or its own length whichever is smaller.
        /// </summary>
        [DebuggerStepThrough]
        public static string Truncate(this string value, int maxLength)
        {
            return string.IsNullOrEmpty(value) ? value : value.Substring(0, Math.Min(value.Length, maxLength));
        }

        /// <summary>
        /// Replaces any of the array of chars with the new char, saves recalling, faster than Regex
        /// </summary>
        [DebuggerStepThrough]
        public static String Replace(this string value, char[] toReplace, char newChar)
        {
            // If the chars to strip is null or empty, then simply return the string as is, rather than trying to look through them
            if (toReplace == null || toReplace.Length == 0)
            {
                return value;
            }

            var sb = new StringBuilder();

            foreach (char c in value)
            {
                sb.Append(toReplace.Contains(c) ? newChar : c);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Shorthand for string.IsNullOrEmpty()
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static bool IsNullOrEmpty(this string value)
        {
            return value == null || value.Length == 0;
        }

        /// <summary>
        /// Shorthand for string.Format()
        /// </summary>
        [DebuggerStepThrough]
        public static string ToFormat(this string str, params object[] args) => string.Format(str, args);

        /// <summary>
        /// Shorthand for string.Format()
        /// </summary>
        [DebuggerStepThrough]
        public static string ToFormat(this string str, IFormatProvider formatProvider, params object[] args) => string.Format(formatProvider, str, args);

        /// <summary>
        /// Removes all the characters from a string, faster than Regex
        /// </summary>
        [DebuggerStepThrough]
        public static String Strip(this string value, char[] toStrip)
        {
            // If the chars to strip is null or empty, then simply return the string as is, rather than trying to look through them
            if (value.IsNullOrEmpty() || toStrip == null || toStrip.Length == 0)
            {
                return value;
            }

            var sb = new StringBuilder();

            foreach (char c in value)
            {
                if (!toStrip.Any(c))
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Alias for string.Split(new[] { separator }, option)
        /// </summary>
        [DebuggerStepThrough]
        public static string[] Split(this string str, string separator, StringSplitOptions option = StringSplitOptions.None) => str.Split(new[] { separator }, option);

        /// <summary>
        /// Alias for string.Split(new[] { separator }, count, option)
        /// </summary>
        [DebuggerStepThrough]
        public static string[] Split(this string str, string separator, int count, StringSplitOptions option = StringSplitOptions.None) => str.Split(new string[] { separator }, count, option);

        /// <summary>
        /// Extends Substring so simply ignores start values below zero returning the string within bounds.
        /// Therefore does not throw an ArgumentOutOfBounds exception.
        /// </summary>
        [DebuggerStepThrough]
        public static string SubStringSafe(this string str, Int32 start)
        {
            // If the starting position is passed the end of the string passed,
            // or the starting position (plus the length) doesnt go past zero
            // then return an empty string, as thats what can be found passed the outside of the string
            if (start >= str.Length)
            {
                return "";
            }

            // If the starting position is before the start of the array then start is at zero
            if (start <= 0)
            {
                return str;
            }

            // If it isnt out of bounds either side, then simply run substring
            return str.Substring(start);
        }

        /// <summary>
        /// Extends Substring so simply ignores start values below zero or above string.Length returning the string within bounds.
        /// Therefore does not throw an ArgumentOutOfBounds exception.
        /// </summary>
        [DebuggerStepThrough]
        public static string SubStringSafe(this string str, Int32 start, Int32 length)
        {
            // If the starting position is passed the end of the string passed,
            // or the starting position (plus the length) doesnt go past zero
            // then return an empty string, as thats what can be found passed the outside of the string
            if (start >= str.Length || (start + length) < 0)
            {
                return "";
            }

            // If start is less than zero, reajust the start position, but also take off that amount from
            // the length var
            if (start < 0)
            {
                Int32 posValue = (start * (-1));
                start = 0;
                length -= posValue;
            }

            // If the size of the chunk requested after starting at the starting position pushes
            // the chunk past the end of the string, reduce the length so that it only gets to the end of the string

            if ((start + length) >= str.Length)
            {
                length = str.Length - start;
            }

            // If it isnt out of bounds either side, then simply run substring
            return str.Substring(start, length);
        }

        /// <summary>
        /// A naive implementation of ToTitleCase, turning the first letter of each word into an invariant captial letter.
        /// </summary>
        /// <param name="input">The string to title case </param>
        /// <param name="preserveCaps">If preserveCaps is true the rest of the word will remain in its original case, otherwise it will be lowercased.</param>
        /// <returns></returns>
        public static String ToTitleCase(this String input, Boolean preserveCaps = false)
        {
            if (input == null)
            {
                return null;
            }

            if (!preserveCaps)
            {
                input = input.ToLowerInvariant();
            }

            return Regex.Replace(input, @"\b[a-z]\w+", match => {
                var v = match.ToString();
                return char.ToUpperInvariant(v[0]) + v.Substring(1).ToLowerInvariant();
            });
        }

        /// <summary>
        /// Splits a string into equal sized blocks (E.g. If size of blocks is 2 then Helloworld = He,ll,ow,or,ld)
        /// </summary>
        /// <param name="str">String to split</param>
        /// <param name="blockSize">The size of each chunk of the original string to put into the </param>
        [DebuggerStepThrough]
        public static IEnumerable<string> ToBlocks(this string str, Int32 blockSize)
        {
            if (blockSize < 1)
            {
                throw new ArgumentException("The size of chunks to split a string into can not be less than 1");
            }

            if (blockSize == 1)
            {
                for (var i = 0; i <= str.Length - 1; i++)
                {
                    yield return str[i].ToString();
                }
            }
            else
            {
                for (var i = 0; i <= str.Length - 1; i += blockSize)
                {
                    yield return str.SubStringSafe(i, blockSize);
                }
            }
        }
    }
}