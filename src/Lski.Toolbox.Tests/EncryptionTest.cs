using FluentAssertions;
using Lski.Toolbox.Encryption;
using System.Security.Cryptography;
using Xunit;
using Xunit.Abstractions;

namespace Lski.Toolbox.Tests
{
    public class EncryptionTest
    {
        private readonly ITestOutputHelper _output;

        public EncryptionTest(ITestOutputHelper output)
        {
            this._output = output;
        }

        [Fact]
        public void EncryptOnlyTest()
        {
            var encryptor = new SimpleEncryption(Aes.Create(), new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 });

            var encrypted = encryptor.Encrypt("Hello", "World");

            _output.WriteLine($"Encrypted {encrypted}");

            encrypted.Should().NotBeNullOrWhiteSpace();
            encrypted.Should().Be("ihA41lsg5UkZq6Md1g4YsA==");
        }

        [Fact]
        public void EncryptAndDecryptTest()
        {
            var encryptor = new SimpleEncryption(Aes.Create(), new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 });

            var encrypted = encryptor.Encrypt("Hello", "World");

            _output.WriteLine($"Encrypted {encrypted}");

            var decrypted = encryptor.Decrypt(encrypted, "World");

            _output.WriteLine($"Decrypted {decrypted}");

            encrypted.Should().NotBeNullOrWhiteSpace();
            encrypted.Should().Be("ihA41lsg5UkZq6Md1g4YsA==");
            decrypted.Should().NotBeNullOrWhiteSpace();
            decrypted.Should().Be("Hello");
        }

        [Fact]
        public void EncryptOnlyStaticTest()
        {
            var encrypted = Aes.Create().Encrypt("Hello", "World", new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 });

            _output.WriteLine($"Encrypted {encrypted}");

            encrypted.Should().NotBeNullOrWhiteSpace();
            encrypted.Should().Be("ihA41lsg5UkZq6Md1g4YsA==");
        }
    }
}