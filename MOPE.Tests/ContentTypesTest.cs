using B4.Mope.Packaging;
using System;
using System.IO;
using System.Reflection;
using Xunit;

namespace MOPE.Tests
{
	public class ContentTypesTest
	{
		[Fact]
		public void TestLoad()
		{
			ContentTypes ct;
			var assembly = Assembly.GetExecutingAssembly();
			using (var xmlFile = assembly.GetManifestResourceStream("MOPE.Tests.Collateral.[Content_Types].xml"))
			{
				ct = ContentTypes.Load(xmlFile);
			}

			Assert.Equal("image/jpeg", ct.Defaults["jpeg"]);
			Assert.Equal("application/vnd.openxmlformats-package.relationships+xml", ct.Defaults["rels"]);
			Assert.Equal("application/xml", ct.Defaults["xml"]);

			Assert.Equal("application/vnd.openxmlformats-officedocument.wordprocessingml.document.main+xml", ct.Overrides["/word/document.xml"]);
			Assert.Equal("application/vnd.openxmlformats-officedocument.wordprocessingml.styles+xml", ct.Overrides["/word/styles.xml"]);
		}
	}
}
