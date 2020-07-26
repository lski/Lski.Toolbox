using FluentAssertions;
using Lski.Toolbox.Collections;
using System.Collections.Generic;
using Xunit;

namespace Lski.Toolbox.Test
{
	public class DictionaryExtensionTests
	{
		[Fact]
		public void ValueReturnedTest()
		{
			IDictionary<string, string> dictionary = new Dictionary<string, string> {
				{ "first", "value" }
			};

			dictionary.Get("first").Should().Be("value");
			dictionary.Get("second").Should().BeNull();
		}
	}
}