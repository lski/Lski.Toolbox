using FluentAssertions;
using Lski.Toolbox.IO;
using Xunit;

namespace Lski.Toolbox.Tests
{
    public class CreateFileInfoTest
    {
        [Fact]
        public void CreateFileInfoObjects()
        {
            FileInfoExt.CreateSafe(@"c:\asasas").Should().NotBeNull();

            FileInfoExt.CreateSafe(@"asasa/asasas").Should().NotBeNull();

            FileInfoExt.CreateSafe(@"../asas.asd.sasd").Should().NotBeNull();

            FileInfoExt.CreateSafe(@"h:?\asdasd").Should().BeNull();

            FileInfoExt.CreateSafe(@"http://asas.asd.sasd").Should().BeNull();
        }
    }
}