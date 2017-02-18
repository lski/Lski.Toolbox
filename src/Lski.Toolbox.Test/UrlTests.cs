using System;
using FluentAssertions;
using Lski.Toolbox.Txt;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lski.Toolbox.Test
{
    [TestClass]
    public class UrlTests
    {
        [TestMethod]
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

        [TestMethod]
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
    }
}