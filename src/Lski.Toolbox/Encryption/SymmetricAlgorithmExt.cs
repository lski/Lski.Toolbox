using System.Security.Cryptography;

namespace Lski.Toolbox.Encryption
{
    /// <summary>
    /// Extension functions to SymmetricAlgorithms for encrypting/decrypting a string value using a password and a salt passed in.
    /// </summary>
    public static class SymmetricAlgorithmExt
    {
        /// <summary>
        /// Decrypts a string that was encrypted using the same password, salt and <see cref="SymmetricAlgorithm"/> that created it
        /// </summary>
        /// <param name="text">The text that you want to decrypt</param>
        /// <param name="password">The password that is used with the salt to encrypt/decrypt the text</param>
        /// <param name="salt">The salt to use with the password to decrypt/encrypt the text</param>
        public static string Decrypt(this SymmetricAlgorithm algorithm, string text, string password, string salt)
        {
            return new SimpleEncryption(algorithm, salt).Decrypt(text, password);
        }

        /// <summary>
        /// Decrypts a string that was encrypted using the same password, salt and <see cref="SymmetricAlgorithm"/> that created it
        /// </summary>
        /// <param name="text">The text that you want to decrypt</param>
        /// <param name="password">The password that is used with the salt to encrypt/decrypt the text</param>
        /// <param name="salt">The salt to use with the password to decrypt/encrypt the text</param>
        public static string Decrypt(this SymmetricAlgorithm algorithm, string text, string password, byte[] salt)
        {
            return new SimpleEncryption(algorithm, salt).Decrypt(text, password);
        }

        /// <summary>
        /// Encrypts a string that can also be decrypted using the same password and a salt and <see cref="SymmetricAlgorithm"/>
        /// </summary>
        /// <param name="value">The text that you want to encypt</param>
        /// <param name="password">The password that is used with the salt to encrypt/decrypt the text</param>
        /// <param name="salt">The salt to use with the password to decrypt/encrypt the text</param>
        public static string Encrypt(this SymmetricAlgorithm algorithm, string value, string password, string salt)
        {
            return new SimpleEncryption(algorithm, salt).Encrypt(value, password);
        }

        /// <summary>
        /// Encrypts a string that can also be decrypted using the same password and a salt and <see cref="SymmetricAlgorithm"/>
        /// </summary>
        /// <param name="value">The text that you want to encypt</param>
        /// <param name="password">The password that is used with the salt to encrypt/decrypt the text</param>
        /// <param name="salt">The salt to use with the password to decrypt/encrypt the text</param>
        public static string Encrypt(this SymmetricAlgorithm algorithm, string value, string password, byte[] salt)
        {
            return new SimpleEncryption(algorithm, salt).Encrypt(value, password);
        }
    }
}