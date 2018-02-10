using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Lski.Toolbox.Encryption
{
	/// <summary>
	/// Used for encrypting/decrypting a string value using a password and a salt passed in. Where the algorithm used is declared as a generic type.
	/// </summary>
	public class SimpleEncryption : ISimpleEncryption
	{
		/// <summary>
		/// The default number of iterations used for encrypting and decrypting
		/// </summary>
		public const int DEFAULT_ITERATIONS = 1000;

		private SymmetricAlgorithm _algorithm;
		private readonly byte[] _salt;

		/// <inheritdoc />
		public SimpleEncryption(SymmetricAlgorithm algorithm, string salt)
			: this(algorithm, Encoding.Unicode.GetBytes(salt)) { }

		/// <inheritdoc />
		public SimpleEncryption(SymmetricAlgorithm algorithm, byte[] salt)
		{
			_algorithm = algorithm;
			_salt = salt;
		}

		/// <inheritdoc />
		public string Decrypt(string text, string encryptionKey, int iterations = DEFAULT_ITERATIONS)
		{
			return DecryptAsync(text, encryptionKey, iterations).GetAwaiter().GetResult();
		}

		/// <inheritdoc />
		public string Encrypt(string value, string encryptionKey, int iterations = DEFAULT_ITERATIONS)
		{
			return EncryptAsync(value, encryptionKey, iterations).GetAwaiter().GetResult();
		}

		/// <inheritdoc />
		public async Task<string> DecryptAsync(string text, string encryptionKey, int iterations = DEFAULT_ITERATIONS)
		{
			using (var rgb = new Rfc2898DeriveBytes(encryptionKey, _salt, iterations))
			{
				var rgbKey = rgb.GetBytes(_algorithm.KeySize >> 3);
				var rgbIV = rgb.GetBytes(_algorithm.BlockSize >> 3);

				var transform = _algorithm.CreateDecryptor(rgbKey, rgbIV);

				using (var buffer = new MemoryStream(Convert.FromBase64String(text)))
				{
					using (var stream = new CryptoStream(buffer, transform, CryptoStreamMode.Read))
					{
						using (var reader = new StreamReader(stream, Encoding.Unicode))
						{
							return await reader.ReadToEndAsync();
						}
					}
				}
			}
		}

		/// <inheritdoc />
		public async Task<string> EncryptAsync(string value, string encryptionKey, int iterations = DEFAULT_ITERATIONS)
		{
			using (DeriveBytes rgb = new Rfc2898DeriveBytes(encryptionKey, _salt, iterations))
			{
				var rgbKey = rgb.GetBytes(_algorithm.KeySize >> 3);
				var rgbIV = rgb.GetBytes(_algorithm.BlockSize >> 3);

				var transform = _algorithm.CreateEncryptor(rgbKey, rgbIV);

				using (var buffer = new MemoryStream())
				{
					using (var stream = new CryptoStream(buffer, transform, CryptoStreamMode.Write))
					{
						using (var writer = new StreamWriter(stream, Encoding.Unicode))
						{
							await writer.WriteAsync(value);
						}
					}

					return Convert.ToBase64String(buffer.ToArray());
				}
			}
		}
	}
}