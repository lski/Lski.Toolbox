using FluentAssertions;
using Lski.Toolbox.Txt;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Security.Cryptography;

namespace Lski.Toolbox.Test
{
    [TestClass]
    public class EncryptionTest
    {
        [TestMethod]
        public void EncryptOnlyTest()
        {
            var encryptor = new SimpleEncryption<AesManaged>(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 });

            var encrypted = encryptor.Encrypt("Hello", "World");

            Console.WriteLine($"Encrypted {encrypted}");

            encrypted.Should().NotBeNullOrWhiteSpace();
            encrypted.Should().Be("ihA41lsg5UkZq6Md1g4YsA==");
        }

        [TestMethod]
        public void EncryptAndDecryptTest()
        {
            var encryptor = new SimpleEncryption<AesManaged>(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 });

            var encrypted = encryptor.Encrypt("Hello", "World");

            Console.WriteLine($"Encrypted {encrypted}");

            var decrypted = encryptor.Decrypt(encrypted, "World");

            Console.WriteLine($"Decrypted {decrypted}");

            encrypted.Should().NotBeNullOrWhiteSpace();
            encrypted.Should().Be("ihA41lsg5UkZq6Md1g4YsA==");
            decrypted.Should().NotBeNullOrWhiteSpace();
            decrypted.Should().Be("Hello");
        }
    }
}