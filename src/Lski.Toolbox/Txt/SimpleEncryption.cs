using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Lski.Toolbox.Txt {

	/// <summary>
	/// Used for encrypting/decrypting a string value using a password and a salt passed in. Where the algorithm used is declared as a generic type.
	/// </summary>
	/// <remarks>
	/// NB: This is an altered version based on code found online, however I no longer can provide reference.
	/// </remarks>
	public class SimpleEncryption<T> : ISimpleEncryption<T> where T : SymmetricAlgorithm, new()
    {
		readonly byte[] _salt;

		/// <summary>
		/// Used for encrypting/decrypting a string value using a password and a salt passed in. Where the algorithm used is declared as a generic type.
		/// </summary>
		/// <param name="salt">The salt to be used with the password to encrypt/decrypt text</param>
		public SimpleEncryption(string salt) {

			_salt = Encoding.Unicode.GetBytes(salt);
		}

		/// <summary>
		/// Used for encrypting/decrypting a string value using a password and a salt passed in. Where the algorithm used is declared as a generic type.
		/// </summary>
		/// <param name="salt">The salt to be used with the password to encrypt/decrypt text</param>
		public SimpleEncryption(byte[] salt) {

			_salt = salt;
		}

        /// <summary>
        /// Decrypts a string that was encrypted using the same password, salt and <see cref="SymmetricAlgorithm"/> that created it
        /// </summary>
        /// <param name="text">The text that you want to decrypt</param>
        /// <param name="password">The password that is used with the salt to encrypt/decrypt the text</param>
	    public string Decrypt(string text, string password) {

	        return DecryptAsync(text, password).GetAwaiter().GetResult();
	    }

        /// <summary>
		/// Encrypts a string that can also be decrypted using the same password and a salt and <see cref="SymmetricAlgorithm"/>
		/// </summary>
		/// <param name="value">The text that you want to encypt</param>
		/// <param name="password">The password that is used with the salt to encrypt/decrypt the text</param>
	    public string Encrypt(string value, string password) {

	        return EncryptAsync(value, password).GetAwaiter().GetResult();
	    }

	    /// <summary>
        /// Decrypts a string that was encrypted using the same password, salt and <see cref="SymmetricAlgorithm"/> that created it
        /// </summary>
        /// <param name="text">The text that you want to decrypt</param>
        /// <param name="password">The password that is used with the salt to encrypt/decrypt the text</param>
        public async Task<string> DecryptAsync(string text, string password) {

			using (DeriveBytes rgb = new Rfc2898DeriveBytes(password, _salt)) {

				var algorithm = new T();
				var rgbKey = rgb.GetBytes(algorithm.KeySize >> 3);
				var rgbIV = rgb.GetBytes(algorithm.BlockSize >> 3);

				var transform = algorithm.CreateDecryptor(rgbKey, rgbIV);

				using (var buffer = new MemoryStream(Convert.FromBase64String(text))) {

					using (var stream = new CryptoStream(buffer, transform, CryptoStreamMode.Read)) {

						using (var reader = new StreamReader(stream, Encoding.Unicode)) {

							return await reader.ReadToEndAsync();
						}
					}
				}
			}
		}

		/// <summary>
		/// Encrypts a string that can also be decrypted using the same password and a salt and <see cref="SymmetricAlgorithm"/>
		/// </summary>
		/// <param name="value">The text that you want to encypt</param>
		/// <param name="password">The password that is used with the salt to encrypt/decrypt the text</param>
		public async Task<string> EncryptAsync(string value, string password) {

			using (DeriveBytes rgb = new Rfc2898DeriveBytes(password, _salt)) {

				var algorithm = new T();
				var rgbKey = rgb.GetBytes(algorithm.KeySize >> 3);
				var rgbIV = rgb.GetBytes(algorithm.BlockSize >> 3);

				var transform = algorithm.CreateEncryptor(rgbKey, rgbIV);

				using (var buffer = new MemoryStream()) {

					using (var stream = new CryptoStream(buffer, transform, CryptoStreamMode.Write)) {

						using (var writer = new StreamWriter(stream, Encoding.Unicode)) {

							await writer.WriteAsync(value);
						}
					}

					return Convert.ToBase64String(buffer.ToArray());
				}
			}
		}
	}
}