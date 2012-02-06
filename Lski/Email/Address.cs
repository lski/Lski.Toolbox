using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Net.Mail;
using Lski.Txt;

namespace Lski.Email {

	/// <summary>
	/// A class for holding an email address.
	/// </summary>
	/// <remarks></remarks>
	public class Address {

		protected MailAddress _value = null;

		public Address() : base() {}
		
		public Address(string address) {
			this.Value = new MailAddress(address);
		}
		
		public Address(string address, string displayName) { 
			this.Value = new MailAddress(address, displayName); 
		}

		public MailAddress Value {
			get {return Value;}
			set {_value = value;}
		}

		/// <summary>
		/// Gets a string representation of this object
		/// </summary>
		/// <returns></returns>
		/// <remarks></remarks>
		public override string ToString() {
			return Value.ToString();
		}

		/// <summary>
		/// States whether the format of the string passed can be used as an email address.
		/// </summary>
		/// <param name="address"></param>
		/// <returns>Null on success or a basic error message on failure (error message does not include the original address)</returns>
		/// <remarks>
		/// States whether the format of the string passed can be used as an email address.
		/// 
		/// NOTE: Only does a basic check, on:
		/// <list>
		/// <item>That if angle brackets are used to hold an email address after a name, that there is an opening AND closing tag, then the
		/// string inside of those opening and closing brackets matches the format of an email address
		/// </item>
		/// <item>The @ symbol exists</item>
		/// <item>The @ symbol is not the first or last char</item>
		/// <item>That there is a dot after the @ symbol, but not as the next character @.</item>
		/// <item>That the last dot in the email address is at least 3 positions from the end of the address, to ensure the smallest TLD or
		/// end of an IP address is covered.
		/// </item>
		/// </list>
		/// </remarks>
		public static Boolean IsEmailValid(String email, Boolean throwReason = false) {

			var result = Regex.IsMatch(email, @"^[\S\s]+[@][\S\s]+[.][^\d]{2,}$");

			if (throwReason) {

				// If null or empty then return false
				if (string.IsNullOrEmpty(email))
					throw new Exception("The email address should not be empty");

				// Try to extract the email address, if theres an error catch and return the descriptive message to the calling method
				email = ExtractEmail(email);

				// Now the address has been extracted proceed to check its in the correct format
				Int32 atPos = email.IndexOf('@');
				Int32 dotPos = email.LastIndexOf('.');

				// Now simply check basic format
				if (atPos == -1) 
					throw new Exception("An email address needs to contain an '@' symbol");

				// If more than one @ symbol then there are too many
				if (email.CountChar('@') > 1)
					throw new Exception("An email address can not contain more than one '@' symbol");

				// Check the positions are correct
				if ((atPos < 1))
					throw new Exception("The first letter of an email address can not be the '@' symbol");
				else if ((atPos == email.Length - 1))
					throw new Exception("The final letter of an email address can not be the '@' symbol");
				else if ((dotPos < atPos + 1))
					throw new Exception("The must be at least one '.' after the '@' symbol");
				else if ((email[atPos + 1] == '.'))
					throw new Exception("A dot '.' can not be the next character after the '@' symbol");
				else if (dotPos > (email.Length - 3))
					throw new Exception("There must either be a valid TLD or a valid end to an IP address at the end of an email address");
			}

			return result;
		}

		/// <summary>
		/// Extracts the an email address from an email address and name combination.
		/// <example><code>"Lee Cooper" <lcooper@nationalwindscreens.co.uk'></code></example>
		/// </summary>
		/// <param name="address">The email address including name that you want to extract the email address from between 
		/// corresponding angle brackets
		/// </param>
		/// <returns>The email address extracted from between angle brackets</returns>
		/// <exception cref="Exception">If there is an error with the passed in format, and exception is thrown, including the
		/// reason why.
		/// </exception>
		/// <remarks>
		/// Extracts the an email address from an email address and name combination.
		/// 
		/// The email address can either be a basic email address, or an email address placed within angle brackets, proceeded by the
		/// descriptive name for the address. If a basic email address, with no angle brackets, the address is simply returned as-is,
		/// however if the email address contains angle brackets the method will attempt to extract the address from between these angle
		/// brackets.
		/// 
		/// This means a list of addresses of both types, can be passed in a loop.
		/// 
		/// Throws an exception on format error, with description. Rewrite using regular expressions doh lol
		/// </remarks>
		public static string ExtractEmail(string address) {

			// Look for opening angle bracket throughout the entire string
			Int32 angleOpen = address.IndexOf('<');
			// Look for closing bracket ONLY for characters after the position of an opening bracket (if there was one)
			Int32 angleClose = address.LastIndexOf('>', (address.Length - 1), (address.Length - 1 - angleOpen));

			// As a email address could possibly be located between angle brackets, check to see if there is an opening one, followed by a
			// closing one, if there is, then the string inbetween should be sent back through this method, if there is a 

			// 1. If there is both an open and close attempt to validate only the string between them (by reitrating this method)
			// 2. If there is an open tag but not a close tag then return error
			// 3. If there is a close tag but no open tag OR the close tag is before the open tag then return error

			if ((angleOpen > -1) && (angleClose > -1)) 
				return address.SubStringAdv((angleOpen + 1), (angleClose - angleOpen - 1));
			else if ((angleOpen > -1) && (angleClose == -1)) 
				throw new Exception("An email address can not contain an opening bracket '<' without a corresponding closing bracket '>'");
			else if (((angleOpen == -1) && (angleClose > -1)) || (angleOpen > angleClose))
				throw new Exception("An email address can not contain a closing bracket '>' without a corresponding opening bracket '<'"); // Note: open has to be tested against greater than

			// Simply return the basic address
			return address;
		}
	}

}


