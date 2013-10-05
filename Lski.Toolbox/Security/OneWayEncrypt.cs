using System;
using System.Security.Cryptography;
using System.Text;

namespace Lski.Toolbox.Security {

	public class OneWayEncrypt {

		/// <summary>
		/// Coverts the passed string into a one-way encrypted string. After encrypted the value can not be decrypted.
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		/// <remarks>Coverts the passed string into a one-way encrypted string. After encrypted the value can not be decrypted.
		/// 
		/// To compare a value against a value encrypted with this method, encrypt the string to check against the already 
		/// encrypted string, then compare the result.
		/// </remarks>
		public static string Process(string input) {

			var encrypter = new SHA256CryptoServiceProvider();
			var bytes = new UTF8Encoding().GetBytes(input);

			return Convert.ToBase64String(encrypter.ComputeHash(bytes));
		}
	}
}