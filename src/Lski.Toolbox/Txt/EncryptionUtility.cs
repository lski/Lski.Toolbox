using System;
using System.Security.Cryptography;

namespace Lski.Toolbox.Txt {

	/// <summary>
	/// A static Utility class for encrypting/decrypting a string value using a password and a salt passed in. Where the algorithm used is declared as a generic type.
	/// </summary>
	/// <remarks>
	/// NB: This is simply a static wrapper for the SimpleEncryption class
	/// </remarks>
	public static class EncryptionUtility {

		/// <summary>
		/// Decrypts a string that was encrypted using the same password, salt and <see cref="SymmetricAlgorithm"/> that created it
		/// </summary>
		/// <param name="text">The text that you want to decrypt</param>
		/// <param name="password">The password that is used with the salt to encrypt/decrypt the text</param>
		/// <param name="salt">The salt to use with the password to decrypt/encrypt the text</param>
		/// <typeparam name="T">The <see cref="SymmetricAlgorithm"/> to use to decrypt the value</typeparam>
		public static string Decrypt<T>(string text, string password, string salt) where T : SymmetricAlgorithm, new() {

			return new SimpleEncryption(salt).Decrypt<T>(text, password);
		}

		/// <summary>
		/// Decrypts a string that was encrypted using the same password, salt and <see cref="SymmetricAlgorithm"/> that created it
		/// </summary>
		/// <param name="text">The text that you want to decrypt</param>
		/// <param name="password">The password that is used with the salt to encrypt/decrypt the text</param>
		/// <param name="salt">The salt to use with the password to decrypt/encrypt the text</param>
		/// <typeparam name="T">The <see cref="SymmetricAlgorithm"/> to use to decrypt the value</typeparam>
		public static string Decrypt<T>(string text, string password, byte[] salt) where T : SymmetricAlgorithm, new() {

			return new SimpleEncryption(salt).Decrypt<T>(text, password);
		}

		/// <summary>
		/// Encrypts a string that can also be decrypted using the same password and a salt and <see cref="SymmetricAlgorithm"/>
		/// </summary>
		/// <param name="value">The text that you want to encypt</param>
		/// <param name="password">The password that is used with the salt to encrypt/decrypt the text</param>
		/// <param name="salt">The salt to use with the password to decrypt/encrypt the text</param>
		/// <typeparam name="T">The <see cref="SymmetricAlgorithm"/> to use to encrypt the value</typeparam>
		public static string Encrypt<T>(string value, string password, string salt) where T : SymmetricAlgorithm, new() {

			return new SimpleEncryption(salt).Encrypt<T>(value, password);
		}

		/// <summary>
		/// Encrypts a string that can also be decrypted using the same password and a salt and <see cref="SymmetricAlgorithm"/>
		/// </summary>
		/// <param name="value">The text that you want to encypt</param>
		/// <param name="password">The password that is used with the salt to encrypt/decrypt the text</param>
		/// <param name="salt">The salt to use with the password to decrypt/encrypt the text</param>
		/// <typeparam name="T">The <see cref="SymmetricAlgorithm"/> to use to encrypt the value</typeparam>
		public static string Encrypt<T>(string value, string password, byte[] salt) where T : SymmetricAlgorithm, new() {

			return new SimpleEncryption(salt).Encrypt<T>(value, password);
		}
	}
}