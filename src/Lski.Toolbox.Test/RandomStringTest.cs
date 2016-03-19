using System;
using Lski.Toolbox.Txt;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace Lski.Toolbox.Test {

	[TestClass]
	public class RandomStringTest {

		[TestMethod]
		public void RandomStringsAreDifferent() {

			var r = new RandomString(CharacterListPredefined.All);

			var s1 = r.Generate();
			var s2 = r.Generate();

			Console.WriteLine(s1);
			Console.WriteLine(s2);

			s1.Should().NotBe(s2);
		}

		[TestMethod]
		public void StringLengthCorrect() {

			var r = new RandomString(CharacterListPredefined.All);

			var s1 = r.Generate();

			Console.WriteLine(s1);

			s1.Should().HaveLength(10);

			s1 = r.Generate(12);

			Console.WriteLine(s1);

			s1.Should().HaveLength(12);
		}

		[TestMethod]
		public void CharacterListTest() {

			var s = CharacterList.Generate(CharacterListOptions.All);

			Console.WriteLine(s);

			s.Should().Equal(@"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_{}[]()/\'""`~,;:.<>");
			s.Should().HaveCount(82);
		}

		[TestMethod]
		public void CharacterListExclusionTest() {

			var s = CharacterList.Generate(CharacterListOptions.All, CharacterListPredefined.SimilarCharacters);

			Console.WriteLine(s);

			s.Should().Equal(@"ABCDEFGHJKMNPQRSTUVWXYZabcdefghjkmnpqrstuvwxyz23456789-_{}[]()/\'""`~,;:.<>");
			s.Should().HaveCount(74);
		}
	}
}