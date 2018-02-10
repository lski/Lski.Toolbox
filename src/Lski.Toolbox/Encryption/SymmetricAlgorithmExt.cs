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
        /// <param name="algorithm">The SymmetricAlgorithm to encrypt with</param>
        /// <param name="text">The text that you want to decrypt</param>
        /// <param name="encryptionKey">The password that is used with the salt to encrypt/decrypt the text</param>
        /// <param name="salt">The salt to use with the password to decrypt/encrypt the text</param>
        /// <param name="iterations">Is the number of times the encryption is performed</param>
        public static string Decrypt(this SymmetricAlgorithm algorithm, string text, string encryptionKey, string salt, int iterations = SimpleEncryption.DEFAULT_ITERATIONS)
        {
            return new SimpleEncryption(algorithm, salt).Decrypt(text, encryptionKey, iterations);
        }

        /// <summary>
        /// Decrypts a string that was encrypted using the same password, salt and <see cref="SymmetricAlgorithm"/> that created it
        /// </summary>
        /// <param name="algorithm">The SymmetricAlgorithm to encrypt with</param>
        /// <param name="text">The text that you want to decrypt</param>
        /// <param name="encryptionKey">The password that is used with the salt to encrypt/decrypt the text</param>
        /// <param name="salt">The salt to use with the password to decrypt/encrypt the text</param>
        /// <param name="iterations">Is the number of times the encryption is performed</param>
        public static string Decrypt(this SymmetricAlgorithm algorithm, string text, string encryptionKey, byte[] salt, int iterations = SimpleEncryption.DEFAULT_ITERATIONS)
        {
            return new SimpleEncryption(algorithm, salt).Decrypt(text, encryptionKey, iterations);
        }

        /// <summary>
        /// Encrypts a string that can also be decrypted using the same password and a salt and <see cref="SymmetricAlgorithm"/>
        /// </summary>
        /// <param name="algorithm">The SymmetricAlgorithm to encrypt with</param>
        /// <param name="value">The text that you want to encypt</param>
        /// <param name="encryptionKey">The password that is used with the salt to encrypt/decrypt the text</param>
        /// <param name="salt">The salt to use with the password to decrypt/encrypt the text</param>
        /// <param name="iterations">Is the number of times the encryption is performed</param>
        public static string Encrypt(this SymmetricAlgorithm algorithm, string value, string encryptionKey, string salt, int iterations = SimpleEncryption.DEFAULT_ITERATIONS)
        {
            return new SimpleEncryption(algorithm, salt).Encrypt(value, encryptionKey, iterations);
        }

        /// <summary>
        /// Encrypts a string that can also be decrypted using the same password and a salt and <see cref="SymmetricAlgorithm"/>
        /// </summary>
        /// <param name="algorithm">The SymmetricAlgorithm to encrypt with</param>
        /// <param name="value">The text that you want to encypt</param>
        /// <param name="encryptionKey">The password that is used with the salt to encrypt/decrypt the text</param>
        /// <param name="salt">The salt to use with the password to decrypt/encrypt the text</param>
        /// <param name="iterations">Is the number of times the encryption is performed</param>
        public static string Encrypt(this SymmetricAlgorithm algorithm, string value, string encryptionKey, byte[] salt, int iterations = SimpleEncryption.DEFAULT_ITERATIONS)
        {
            return new SimpleEncryption(algorithm, salt).Encrypt(value, encryptionKey, iterations);
        }
    }
}