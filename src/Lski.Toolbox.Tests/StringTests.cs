using FluentAssertions;
using Lski.Toolbox.Txt;
using Xunit;

namespace Lski.Toolbox.Tests
{
    public class StringTests
    {
        [Fact]
        public void Format_IsCorrectTest()
        {
            var str = "hello {0}".ToFormat("world");

            str.Should().Be("hello world");

            str = "{0} {1}".ToFormat("hello", "world");

            str.Should().Be("hello world");
        }
    }
}