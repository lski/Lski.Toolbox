using System;
using System.Collections.Generic;
using FluentAssertions;
using Lski.Toolbox.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lski.Toolbox.Test
{
    [TestClass]
    public class DictionaryExtensionTests
    {
        [TestMethod]
        public void ValueReturnedTest() {
            IDictionary<string, string> dictionary = new Dictionary<string, string>();

            dictionary.Add("first", "value");

            dictionary.Get("first").Should().Be("value");
            dictionary.Get("second").Should().BeNull();
        }
    }
}