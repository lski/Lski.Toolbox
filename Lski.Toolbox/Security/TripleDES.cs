using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace Lski.Toolbox.Security {

	/// <summary>
	/// Class that provides two static methods for encrypting and decrypting a string using TripleDES encryption. Not used atm as the key and iv are hardcoded.
	/// </summary>
	/// <remarks></remarks>
	public partial class TripleDES {

		/// <summary>
		/// Creates a new TripleDES with the defined salt, so Encrypt and Decrypt dont require the salt passed each time
		/// </summary>
		/// <param name="salt"></param>
		public TripleDES(byte[] salt) {
			Salt = salt;
		}

		private byte[] Salt { get; set; }

		/// <summary>
		/// Encrypt a string using a string password
		/// </summary>
		/// <param name="plainText"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public string Encrypt(string plainText, string password) {

			var pdb = new Rfc2898DeriveBytes(password, Salt);
			return Convert.ToBase64String(Encrypt(plainText, pdb.GetBytes(24), pdb.GetBytes(8)));

		}

		/// <summary>
		/// Decrypt a string using a string password
		/// </summary>
		/// <param name="cipherText"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public string Decrypt(string cipherText, string password) {

			var pdb = new Rfc2898DeriveBytes(password, Salt);
			return Decrypt(Convert.FromBase64String(cipherText), pdb.GetBytes(24), pdb.GetBytes(8));

		}

		#region Static Members

		/// <summary>
		/// Holds the application instance, needs to be set prior to using
		/// </summary>
		public static TripleDES Instance { get; set; }

		public static string Encrypt(string plainText, string password, byte[] salt) {

			var pdb = new Rfc2898DeriveBytes(password, salt);
			return Convert.ToBase64String(Encrypt(plainText, pdb.GetBytes(24), pdb.GetBytes(8)));

		}

		/// <summary>
		/// Encrypts
		/// </summary>
		/// <param name="plainText"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public static byte[] Encrypt(string plainText, byte[] key, byte[] iv) {
			
			// Declare a UTF8Encoding object so we may use the GetByte method to transform the plainText into a Byte array.
			UTF8Encoding utf8encoder = new UTF8Encoding();
			byte[] inputInBytes = utf8encoder.GetBytes(plainText);

			// Create a new TripleDES service provider
			TripleDESCryptoServiceProvider tdesProvider = new TripleDESCryptoServiceProvider();

			// The ICryptTransform interface uses the TripleDES crypt provider along with encryption key and init vector information
			ICryptoTransform cryptoTransform = tdesProvider.CreateEncryptor(key, iv);

			// All cryptographic functions need a stream to output the  encrypted information. Here we declare a memory stream for this purpose.
			MemoryStream encryptedStream = new MemoryStream();
			CryptoStream cryptStream = new CryptoStream(encryptedStream, cryptoTransform, CryptoStreamMode.Write);

			// Write the encrypted information to the stream. Flush the information  when done to ensure everything is out of the buffer.
			cryptStream.Write(inputInBytes, 0, inputInBytes.Length);
			cryptStream.FlushFinalBlock();
			encryptedStream.Position = 0;

			// Read the stream back into a Byte array and return it to the calling method.
			byte[] result = new byte[Convert.ToInt32(encryptedStream.Length - 1) + 1];
			encryptedStream.Read(result, 0, Convert.ToInt32(encryptedStream.Length));
			cryptStream.Close();
			return result;

		}

		public static string Decrypt(string cipherText, string password, byte[] salt) {

			var pdb = new Rfc2898DeriveBytes(password, salt);
			return Decrypt(Convert.FromBase64String(cipherText), pdb.GetBytes(24), pdb.GetBytes(8));

		}

		public static string Decrypt(byte[] inputInBytes, byte[] key, byte[] iv) {

			// If an empty encrypted string return an empty unencrypted string, to avoid it breaking
			if (inputInBytes.Length == 0)
				return string.Empty;

			// UTFEncoding is used to transform the decrypted Byte Array
			// information back into a string.
			UTF8Encoding utf8encoder = new UTF8Encoding();
			TripleDESCryptoServiceProvider tdesProvider = new TripleDESCryptoServiceProvider();

			// As before we must provide the encryption/decryption key along with the init vector.
			ICryptoTransform cryptoTransform = tdesProvider.CreateDecryptor(key, iv);

			// Provide a memory stream to decrypt information into
			MemoryStream decryptedStream = new MemoryStream();
			CryptoStream cryptStream = new CryptoStream(decryptedStream, cryptoTransform, CryptoStreamMode.Write);
			cryptStream.Write(inputInBytes, 0, inputInBytes.Length);
			cryptStream.FlushFinalBlock();
			decryptedStream.Position = 0;

			// Read the memory stream and convert it back into a string
			byte[] result = new byte[Convert.ToInt32(decryptedStream.Length)];
			decryptedStream.Read(result, 0, Convert.ToInt32(decryptedStream.Length));
			cryptStream.Close();
			UTF8Encoding myutf = new UTF8Encoding();
			return myutf.GetString(result);

		}

		#endregion

	}

}
