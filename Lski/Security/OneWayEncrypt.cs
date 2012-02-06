using System.Security.Cryptography;
using System.Text;
using System.Security;

namespace Lski.Security {

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

			MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
			System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();

			return System.Convert.ToBase64String(md5Hasher.ComputeHash(encoder.GetBytes(input)));

		}
	}
}