using FluentAssertions;
using Lski.Toolbox.Txt;
using System;
using System.Text;
using Xunit;

namespace Lski.Toolbox.Test
{
    public class UrlTests
    {
        [Fact]
        public void AddParameterTest()
        {
            var uri = new Uri("http://www.google.com/");

            uri.AddParameter("q", "avalue");

            uri.ToString().Should().Be("http://www.google.com/", "Because its non-destructive");

            uri = uri.AddParameter("q", null);

            uri.ToString().Should().Be("http://www.google.com/", "Not add null values");

            uri = uri.AddParameter("q", "avalue");

            uri.ToString().Should().Be("http://www.google.com/?q=avalue");

            uri = uri.AddParameter("x", 1);

            uri.ToString().Should().Be("http://www.google.com/?q=avalue&x=1");
        }

        [Fact]
        public void ChangeSchemeTest()
        {
            var uri = new Uri("http://www.google.com/");

            uri.ChangeScheme();

            uri.ToString().Should().Be("http://www.google.com/", "Because its non-destructive");

            uri = uri.ChangeScheme();

            uri.ToString().Should().Be("https://www.google.com/");

            uri = uri.ChangeScheme("ftp");

            uri.ToString().Should().Be("ftp://www.google.com/");
        }

        [Fact]
        public void AddParameterStringTest()
        {
            var uri = "http://www.google.com/";

            uri.AddQueryParameter("q", "avalue");

            uri.Should().Be("http://www.google.com/", "Because its non-destructive");

            uri = uri.AddQueryParameter("q", null);

            uri.Should().Be("http://www.google.com/", "Not add null values");

            uri = uri.AddQueryParameter("q", "avalue");

            uri.Should().Be("http://www.google.com/?q=avalue");

            uri = uri.AddQueryParameter("x", 1);

            uri.Should().Be("http://www.google.com/?q=avalue&x=1");
        }

        [Fact]
        public void AddParameterStringBuilderTest()
        {
            var uri = new StringBuilder("http://www.google.com/");

            uri = uri.AddQueryParameter("q", null);

            uri.ToString().Should().Be("http://www.google.com/", "Not add null values");

            uri = uri.AddQueryParameter("q", "avalue");

            uri.ToString().Should().Be("http://www.google.com/?q=avalue");

            uri = uri.AddQueryParameter("x", 1);

            uri.ToString().Should().Be("http://www.google.com/?q=avalue&x=1");
        }
    }
}