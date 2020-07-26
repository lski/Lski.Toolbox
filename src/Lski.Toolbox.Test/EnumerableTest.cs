using FluentAssertions;
using Lski.Toolbox.Collections;
using System.Collections.Generic;
using Xunit;

namespace Lski.Toolbox.Test
{
    public class EnumerableTest
    {
        [Fact]
        public void IsNullOrEmpty_Test()
        {
            IEnumerable<string> e = null;

            e.IsNullOrEmpty().Should().BeTrue();

            e = new List<string>();

            e.IsNullOrEmpty().Should().BeTrue();

            e = new[] { "Hello", "World" };

            e.IsNullOrEmpty().Should().BeFalse();
        }
    }
}