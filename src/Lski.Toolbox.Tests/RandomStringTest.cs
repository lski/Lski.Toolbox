using FluentAssertions;
using Lski.Toolbox.Txt;
using System;
using Xunit;

namespace Lski.Toolbox.Test
{
    public class RandomStringTest
    {
        [Fact]
        public void RandomStringsAreDifferent()
        {
            var r = new RandomString(CharacterList.All);

            var s1 = r.Generate();
            var s2 = r.Generate();

            Console.WriteLine(s1);
            Console.WriteLine(s2);

            s1.Should().NotBe(s2);
        }

        [Fact]
        public void StringLengthCorrect()
        {
            var r = new RandomString(CharacterList.All);

            var s1 = r.Generate();

            Console.WriteLine(s1);

            s1.Should().HaveLength(10);

            s1 = r.Generate(12);

            Console.WriteLine(s1);

            s1.Should().HaveLength(12);
        }
    }
}