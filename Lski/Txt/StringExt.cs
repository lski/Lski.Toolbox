using Microsoft.VisualBasic;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Globalization;

namespace Lski.Txt {

	public static class StringExt {

		#region "Extension Methods"

		/// <summary>
		/// Takes the passed string and either truncates the string or pads it with spaces (Default pads to the right)
		/// </summary>
		/// <param name="str">The str to turn to fixed length</param>
		/// <param name="size">The size of the fixed length string</param>
		/// <returns>A fixed length string, padded with spaces</returns>
		/// <remarks></remarks>
		[DebuggerStepThrough()]
		public static string FixedLength(this string str, Int32 size) {

			// If the passed size is larger, pad the string
			// Otherwise truncates the string down in size
			if (str == null)
				return String.Empty.PadRight(size);
			else if (size > str.Length)		
				return str.PadRight(size);
			else if (str.Length > size)		
				return str.Substring(0, size);

			return str;
		}

		/// <summary>
		/// Takes the passed string and either truncates the string or pads it with the specified Char. (Default pads to the right)
		/// </summary>
		/// <param name="str">The str to turn to fixed length</param>
		/// <param name="size">The size of the fixed length string</param>
		/// <param name="charToPad">The char to use to pad the string</param>
		/// <returns>A fixed length string, padded with the passed char</returns>
		/// <remarks></remarks>
		[DebuggerStepThrough()]
		public static string FixedLength(this string str, Int32 size, char charToPad) {

			// If the passed size is larger, pad the string
			// Otherwise truncates the string down in size
			if (str == null)
				return String.Empty.PadRight(size, charToPad);
			else if (size > str.Length) 	
				return str.PadRight(size, charToPad);
			else if (str.Length > size)		
				return str.Substring(0, size);

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
		/// <remarks></remarks>
		[DebuggerStepThrough()]
		public static string FixedLength(this string str, Int32 size, char charToPad, bool padLeft) {

			// If the passed size is larger, pad the string
			// Otherwise truncates the string down in size
			if (str == null) {

				if (padLeft)
					return String.Empty.PadLeft(size, charToPad);
				else
					return String.Empty.PadRight(size, charToPad);

			} else if (size > str.Length) {
				
				if (padLeft)	
					return str.PadLeft(size, charToPad);
				else			
					return str.PadRight(size, charToPad);
				
			} else if (str.Length > size) 
				return str.Substring(0, size);

			return str;

		}

		/// <summary>
		/// Truncates the passed string to the maxLength or its own length whichever is smaller
		/// </summary>
		/// <param name="value">The string to truncate</param>
		/// <param name="maxLength">The maximum length of the desired string</param>
		/// <returns></returns>
		public static string Truncate(this string value, int maxLength) {
			
			if (string.IsNullOrEmpty(value))
				return value; 

			return value.Substring(0, Math.Min(value.Length, maxLength));
		}


		/// <summary>
		/// Counts the characters in the passed string, that match the char passed. Does not break if the length or start are passed the
		/// end of array. It simply truncates the searched area of the string.
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public static Int32 CountChar(this string str, char chr, Int32 start = 0, Int32? length = null) {

			// If the start is passed the last position in the passed string then theres nothing to check, so return zero
			if (string.IsNullOrEmpty(str) || (start >= str.Length))
				return 0;

			Int32 counter = 0;
			Int32 lastPosition = (start + (length ?? str.Length) - 1);

			// If the length + start is past the end of the string, cut it down to fit
			if (lastPosition > str.Length)
				lastPosition = (str.Length - 1);

			// Run through the string
			for (Int32 i = start; i <= lastPosition; i++) {
				
				if (str[i] == chr)
					counter += 1;
			}

			return counter;

		}

		/// <summary>
		/// Strips the character from the string, faster than string.Replace as its not adding empty strings and accepts a simply char to compare
		/// </summary>
		/// <param name="string">The string to strip the character from</param>
		/// <param name="chr">The character to strip from the</param>
		/// <returns></returns>
		/// <remarks></remarks>
		[DebuggerStepThrough()]
		public static StringBuilder Strip(this string str, char chr) {

			System.Text.StringBuilder sb = new System.Text.StringBuilder();

			foreach (Char c in str) {

				if (c != chr) 
					sb.Append(c);
			}

			return sb;

		}

		/// <summary>
		/// Removes the any instance from the array of characters wherever they are found within the string 
		/// </summary>
		/// <param name="value">The string to strip the character from</param>
		/// <param name="charsToStrip">The character list to strip from the current string</param>
		/// <returns></returns>
		/// <remarks></remarks>
		[DebuggerStepThrough()]
		public static StringBuilder Strip(this string value, Char[] chars) {

			// If the chars to strip is null or empty, then simply return the string as is, rather than trying to look through them
			if (chars == null || chars.Length == 0)	
				return new System.Text.StringBuilder(value);

			var sb = new System.Text.StringBuilder();

			foreach (Char c in value) {

				if (!chars.Contains(c)) 
					sb.Append(c);
			}

			return sb;
		}

		/// <summary>
		/// Replaces any of the array of chars with the new char, saves recalling, faster than Regex
		/// </summary>
		/// <param name="value"></param>
		/// <param name="oldChars"></param>
		/// <param name="newChar"></param>
		/// <returns></returns>
		[DebuggerStepThrough()]
		public static String Replace(this string value, Char[] oldChars, char newChar) {

			// If the chars to strip is null or empty, then simply return the string as is, rather than trying to look through them
			if (oldChars == null || oldChars.Length == 0)	
				return value;

			var sb = new System.Text.StringBuilder();

			foreach (Char c in value) {
				sb.Append(oldChars.Contains(c) ? newChar : c);
			}

			return sb.ToString();
		}

		public static string[] Split(this string str, string separator, StringSplitOptions option = StringSplitOptions.None) {

			return str.Split(new string[] { separator }, option);
		}

		public static string[] Split(this string str, string separator, int count, StringSplitOptions option = StringSplitOptions.None) {

			return str.Split(new string[] { separator }, count, option);
		}

		/// <summary>
		/// Splits a string into an array of string, provides additional functionality to 
		/// stringObj.Split, as it accepts a string greater than one character long
		/// </summary>
		/// <param name="str">The string to split into an array</param>
		/// <param name="separator">The string used to split the main array</param>
		/// <param name="options">States how it deals with empty sections</param>
		/// <returns>Returns a string array with the correct number of positions, if nothing found, then an empty array is returned</returns>
		/// <remarks>
		/// Splits a string into an array of string, provides additional functionality to 
		/// stringObj.Split, as it accepts a string greater than one character long
		/// 
		/// Returns a string array rather than a list(of String) because that is the default
		/// behavior of stringObj.Split and if this method is only passed a single char string
		/// then it runs that method by default to take advantage of the that methods optimization.
		/// </remarks>
		[System.Diagnostics.DebuggerStepThrough()]
		[Obsolete("No longer any quicker than the built in version", true)]
		public static string[] SplitAdv(this string str, string separator, StringSplitOptions option = StringSplitOptions.None)
		{

			// If nothing passed then return empty string array
			if (string.IsNullOrEmpty(str)) return new string[] { };

			// 1. If no separator was passed return string array, with passed string only value
			// 2. If only a single char then run the quicker stringObj.Split method
			if (string.IsNullOrEmpty(separator))
				return new string[] { str };
			else if (separator.Length == 1)
				return str.Split(separator.ToCharArray(), option);

			// Looping vars
			Int32 i = default(Int32);
			Int32 j = default(Int32);
			Int32 textStart = 0;
			bool matchFound = false;
			// Get the last position to bother checking in the char loop, if separator is longer than
			// than 1 position then why check past the last position - the length of the separator
			Int32 posOfLastCharToCheck = ((str.Length) - separator.Length);

			List<string> returnVal = new List<string>();
			char[] sepChars = separator.ToCharArray();

			// Loop through each char of the

			for (i = 0; i <= posOfLastCharToCheck; i += 1) {
				// If the current char is the same as the first char of the separator
				// then loop loop to check the next chars match too

				if (str[i] == sepChars[0]) {
					// A match has been found with the first char
					matchFound = true;

					// Loop through the rest of the chars in the separator

					for (j = 1; j <= (sepChars.Length - 1); j++) {
						// If not equal to the char in str then exit, and mark as not found

						if (sepChars[j] != str[i + j]) {
							matchFound = false;
							break;
						}

					}

					// If still a match found then a COMPLETE separator has been found, so add it to the 
					// returnVal

					if (matchFound) {
						// If the length of the string to store is zero, only store if the splitOptions
						// equal 'none' for no exclusions
						if ((i - textStart) > 0) {
							returnVal.Add(str.Substring(textStart, (i - textStart)));
						} else if (option == StringSplitOptions.None) {
							returnVal.Add("");
						}

						// Remove the one, because the loop will add it
						i = (i + j) - 1;
						textStart = i + 1;

					}

				}

			}

			// If textStart is less than the end of the string then there is a string to
			// capture
			// If equal then there was a separator at the end, so therefore a blank position
			// Else there is nothing else to capture

			if (textStart <= (str.Length)) {
				// If the length of the string to store is zero, only store if the splitOptions
				// equal 'none' for no exclusions
				if ((str.Length - textStart) > 0) {
					returnVal.Add(str.Substring(textStart, (str.Length - textStart)));
				} else if (option == StringSplitOptions.None) {
					returnVal.Add("");
				}

			}

			return returnVal.ToArray();

		}

		/// <summary>
		/// Extracts a section of string, without throwing an argumentoutofbounds exception
		/// </summary>
		/// <param name="str">The string to extract from</param>
		/// <param name="start">The starting position of the string (if below zero, simply returns the whole
		/// string
		/// </param>
		/// <returns>Type: System.String 
		/// The section of string requested</returns>
		/// <remarks>Provides Additional functionality to the String.SubString method, the only difference being is that
		/// this method will NOT throw an out-of-bounds error. It deals with it sensibly. If the start is before zero
		/// it changes it to zero, to return the whole string. 
		/// 
		/// This can be useful in automated processes, where
		/// it is impossible to guarantee that the start value coming in will be in the bounds of the original
		/// string.
		/// </remarks>
		[System.Diagnostics.DebuggerStepThrough()]
		public static string SubStringAdv(this string str, Int32 start) {

			// If the starting position is passed the end of the string passed, 
			// or the starting position (plus the length) doesnt go past zero
			// then return an empty string, as thats what can be found passed the outside of the string
			if (start >= str.Length)
				return "";

			// If the starting position is before the start of the array then start is at zero
			if (start <= 0)
				return str;

			// If it isnt out of bounds either side, then simply run substring
			return str.Substring(start);
		}

		/// <summary>
		/// Extracts a section of string, without throwing an argumentoutofbounds exception
		/// </summary>
		/// <param name="str">The string to extract from</param>
		/// <param name="start">The starting position of the string (if below zero, simply returns the whole
		/// string
		/// </param>
		/// <param name="length">The total length of the string desired (from original starting position)</param>
		/// <returns>Type: System.String 
		/// The section of string requested</returns>
		/// <remarks>Provides Additional functionality to the String.SubString method, the only difference being is that
		/// this method will NOT throw an out-of-bounds error. It deals with it sensibly. If the start is before 
		/// zero it changes it to zero, to return the length of string desired (minus what was taken away for
		/// the start position. 
		/// 
		/// E.g. SubString(str, -5, 10) will return a string 5 characters in length, where as 
		/// Substring(str, -10, 5) will return an empty string.
		/// 
		/// This can be useful in automated processes, where
		/// it is impossible to guarantee that the start value coming in will be in the bounds of the original
		/// string.
		/// </remarks>
		[System.Diagnostics.DebuggerStepThrough()]
		public static string SubStringAdv(this string str, Int32 start, Int32 length) {

			// If the starting position is passed the end of the string passed, 
			// or the starting position (plus the length) doesnt go past zero
			// then return an empty string, as thats what can be found passed the outside of the string
			if ((start >= str.Length) || (start + length) < 0) 
				return "";
			

			// If start is less than zero, reajust the start position, but also take off that amount from
			// the length var
			if (start < 0) {
				Int32 posValue = (start * (-1));
				start = 0;
				length -= posValue;
			}

			// If the size of the chunk requested after starting at the starting position pushes
			// the chunk past the end of the string, reduce the length so that it only gets to the end of the string

			if ((start + length) >= str.Length) 
				length = str.Length - start;

			// If it isnt out of bounds either side, then simply run substring
			return str.Substring(start, length);
		}

		/// <summary>
		/// Converts the passed string into a title case verson, preserveAllCaps being true means that if a word is all in capitals then it will remain all in captials. However
		/// if preserveAllCaps is false then all words are converted.
		/// </summary>
		/// <param name="input"></param>
		/// <param name="preserveAllCaps"></param>
		/// <returns></returns>
		public static String ToTitleCase(this String input, Boolean preserveAllCaps = false) {

			if (input == null)
				return null;

			try {
				return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(preserveAllCaps ? input : input.ToLower());
			} catch (Exception) {
				return null;
			}
		}

		#endregion

		#region "Non Extension Methods"

		/// <summary>
		/// Converts a numeric to an Alpha e.g. 1 => A, 27 => AA etc
		/// </summary>
		/// <param name="columnNumber"></param>
		/// <returns></returns>
		/// <remarks>
		/// As I appear to has lost my version of this method is cleaning code this comes from Graham Miller @ Stakoverflow
		/// </remarks>
		public static string ConvertToExcelColumnName(int columnNumber) {

			int dividend = columnNumber;
			string columnName = String.Empty;
			int modulo;

			while (dividend > 0) {
				modulo = (dividend - 1) % 26;
				columnName = System.Convert.ToChar(65 + modulo).ToString() + columnName;
				dividend = (int)((dividend - modulo) / 26);
			}

			return columnName;
		}

		/// <summary>
		/// A version of trim for strings, that handles null string values as well as initialised strings. Either returns null or and empty string, depending
		/// on the option passed.
		/// </summary>
		/// <param name="str">The string to trim</param>
		/// <param name="emptyStringOnNull">If true and the string is null, then an empty string is returned, otherwise null</param>
		/// <returns></returns>
		public static String Trim(string str, bool emptyStringOnNull = true) {

			if (str == null) {

				if (emptyStringOnNull)
					return String.Empty;
				else
					return null;
			}

			return str.Trim();
		}

		/// <summary>
		/// Accepts a list of strings and returns them joined together. If a string is not null and has a length of greater then zero the string followed by
		/// a string separator. An implementation of the MySQL Concat With Separator (ConcatWS)
		/// </summary>
		/// <param name="separator"></param>
		/// <param name="strings"></param>
		/// <returns></returns>
		public static String ConcatWS(String separator, params Object[] objs) {
			return StringExt.ConcatWS(objs, separator);
		}

		/// <summary>
		/// Accepts a list of strings and returns them joined together. If a string is not null and has a length of greater then zero the string followed by
		/// a string separator.
		/// </summary>
		/// <param name="separator"></param>
		/// <param name="strings"></param>
		/// <returns></returns>
		public static String ConcatWS(IEnumerable<String> objs, String separator) {

			return String.Join(separator, (from s in objs
										   where s != null && s.Length > 0
										   select s));
		}

		/// <summary>
		/// Accepts a list of strings and returns them joined together. If a string is not null and has a length of greater then zero the string followed by
		/// a string separator.
		/// </summary>
		/// <param name="separator"></param>
		/// <param name="strings"></param>
		/// <returns></returns>
		public static String ConcatWS(IEnumerable<Object> objs, String separator) {

			return String.Join(separator, (from o in objs 
										   where o != null 
										   let s = o.ToString() // Convert it once only
										   where s.Length > 0 
										   select s));
		}

		/// <summary>
		/// Simply cuts down the string and returns it in a list where each piece is the size stated (E.g. If size of chunks is 2 then Helloworld = He,ll,ow,or,ld)
		/// </summary>
		/// <param name="chunkSize">The size of each chunk of the original string to put into the </param>
		/// <returns></returns>
		/// <remarks></remarks>
		public static IEnumerable<string> ChunkString(string str, Int32 chunkSize) {

			if (chunkSize < 1)
				throw new ArgumentException("The size of chunks to split a string into can not be less than 1");

			if (chunkSize == 1) {

				for (Int32 i = 0; i <= str.Length - 1; i++) {

					yield return str[i].ToString();
				}


			} else {

				for (Int32 i = 0; i <= str.Length - 1; i += chunkSize) {

					yield return str.SubStringAdv(i, chunkSize);
				}

			}

		}

		#endregion
	}
}
