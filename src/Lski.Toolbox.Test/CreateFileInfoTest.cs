using Lski.Toolbox.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace Lski.Toolbox.Test {

	[TestClass]
	public class CreateFileInfoTest {

		[TestMethod]
		public void CreateFileInfoObjects() {

			FileInfoExtensions.CreateSafe(@"c:\asasas").Should().NotBeNull();

			FileInfoExtensions.CreateSafe(@"asasa/asasas").Should().NotBeNull();
			
			FileInfoExtensions.CreateSafe(@"../asas.asd.sasd").Should().NotBeNull();

			FileInfoExtensions.CreateSafe(@"h:?\asdasd").Should().BeNull();

			FileInfoExtensions.CreateSafe(@"http://asas.asd.sasd").Should().BeNull();
		}
	}
}