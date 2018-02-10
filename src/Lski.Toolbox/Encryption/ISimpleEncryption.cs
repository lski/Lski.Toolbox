using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Lski.Toolbox.Encryption
{
    /// <summary>
    /// Utility class for encrypting/decrypting a string value using a password and a salt passed in. Where the algorithm used is declared as a generic type.
    /// </summary>
    /// <typeparam name="T">The <see cref="SymmetricAlgorithm"/> to use to encrypt the value</typeparam>
    public interface ISimpleEncryption
    {
        /// <summary>
        /// Decrypts a string that was encrypted using the same password, salt and <see cref="SymmetricAlgorithm"/> that created it
        /// </summary>
        /// <param name="text">The text that you want to decrypt</param>
        /// <param name="encryptionKey">The password that is used with the salt to encrypt/decrypt the text</param>
        /// <param name="iterations">Is the number of times the encryption is performed</param>
        string Decrypt(string text, string encryptionKey, int iterations = 1000);

        /// <summary>
        /// Encrypts a string that can also be decrypted using the same password and a salt and <see cref="SymmetricAlgorithm"/>
        /// </summary>
        /// <param name="value">The text that you want to encypt</param>
        /// <param name="encryptionKey">The password that is used with the salt to encrypt/decrypt the text</param>
        /// <param name="iterations">Is the number of times the encryption is performed</param>
        string Encrypt(string value, string encryptionKey, int iterations = 1000);

        /// <summary>
        /// Decrypts a string that was encrypted using the same password, salt and <see cref="SymmetricAlgorithm"/> that created it
        /// </summary>
        /// <param name="text">The text that you want to decrypt</param>
        /// <param name="encryptionKey">The password that is used with the salt to encrypt/decrypt the text</param>
        /// <param name="iterations">Is the number of times the encryption is performed</param>
        Task<string> DecryptAsync(string text, string encryptionKey, int iterations = 1000);

        /// <summary>
        /// Encrypts a string that can also be decrypted using the same password and a salt and <see cref="SymmetricAlgorithm"/>
        /// </summary>
        /// <param name="value">The text that you want to encypt</param>
        /// <param name="encryptionKey">The password that is used with the salt to encrypt/decrypt the text</param>
        /// <param name="iterations">Is the number of times the encryption is performed</param>
        Task<string> EncryptAsync(string value, string encryptionKey, int iterations = 1000);
    }
}