using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Lski.Toolbox.Txt {

    /// <summary>
    /// Utility class for encrypting/decrypting a string value using a password and a salt passed in. Where the algorithm used is declared calling.
    ///
    /// This is an altered version based on code found online, however I no longer can provide reference.
    /// </summary>
    public class EncryptionUtility {

        /// <summary>
        /// Encrypts a string that can also be decrypted using a password and a salt (effectively a second passord)
        /// </summary>
        /// <typeparam name="T">The SymmetricAlgorithm to use to encrypt the value</typeparam>
        public static string Encrypt<T>(string value, string password, string salt) where T : SymmetricAlgorithm, new() {

            return Encrypt<T>(value, password, Encoding.Unicode.GetBytes(salt));
        }

        /// <summary>
        /// Encrypts a string that can also be decrypted using a password and a salt (effectively a second passord)
        /// </summary>
        /// <typeparam name="T">The SymmetricAlgorithm to use to encrypt the value</typeparam>
        public static string Encrypt<T>(string value, string password, byte[] salt) where T : SymmetricAlgorithm, new() {

            DeriveBytes rgb = new Rfc2898DeriveBytes(password, salt);

            var algorithm = new T();
            var rgbKey = rgb.GetBytes(algorithm.KeySize >> 3);
            var rgbIV = rgb.GetBytes(algorithm.BlockSize >> 3);

            var transform = algorithm.CreateEncryptor(rgbKey, rgbIV);

            using (var buffer = new MemoryStream()) {

                using (var stream = new CryptoStream(buffer, transform, CryptoStreamMode.Write)) {

                    using (var writer = new StreamWriter(stream, Encoding.Unicode)) {

                        writer.Write(value);
                    }
                }

                return Convert.ToBase64String(buffer.ToArray());
            }
        }

        /// <summary>
        /// Decrypts a string that was encrypted using the same password, salt and SymmetricAlgorithm that created it
        /// </summary>
        /// <typeparam name="T">The SymmetricAlgorithm to use to decrypt the value</typeparam>
        public static string Decrypt<T>(string text, string password, string salt) where T : SymmetricAlgorithm, new() {

            return Decrypt<T>(text, password, Encoding.Unicode.GetBytes(salt));
        }

        public static string Decrypt<T>(string text, string password, byte[] salt) where T : SymmetricAlgorithm, new() {

            DeriveBytes rgb = new Rfc2898DeriveBytes(password, salt);

            var algorithm = new T();
            var rgbKey = rgb.GetBytes(algorithm.KeySize >> 3);
            var rgbIV = rgb.GetBytes(algorithm.BlockSize >> 3);

            var transform = algorithm.CreateDecryptor(rgbKey, rgbIV);

            using (var buffer = new MemoryStream(Convert.FromBase64String(text))) {

                using (var stream = new CryptoStream(buffer, transform, CryptoStreamMode.Read)) {

                    using (var reader = new StreamReader(stream, Encoding.Unicode)) {

                        return reader.ReadToEnd();
                    }
                }
            }
        }
    }
}