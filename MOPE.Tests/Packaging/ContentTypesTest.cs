using B4.Mope.Packaging;
using System;
using System.IO;
using System.Reflection;
using Xunit;

namespace MOPE.Tests
{
	public class ContentTypesTest
	{
		[Theory]
		[InlineData("application/vnd.openxmlformats-officedocument.wordprocessingml.document.main+xml", "/word/document.xml")]
		[InlineData("application/vnd.openxmlformats-officedocument.wordprocessingml.styles+xml", "/word/styles.xml")]
		public void TestLoadOverrides(string contentType, string partUri)
		{
			ContentTypes ct;
			var assembly = Assembly.GetExecutingAssembly();
			using (var xmlFile = assembly.GetManifestResourceStream("MOPE.Tests.Collateral.[Content_Types].xml"))
			{
				ct = ContentTypes.Load(xmlFile);
			}

			Assert.Equal(contentType, ct.Overrides[partUri]);
			Assert.Equal(contentType, ct.GetContentType(partUri));
		}

		[Theory]
		[InlineData("image/jpeg", "jpeg")]
		[InlineData("application/xml", "xml")]
		[InlineData("application/vnd.openxmlformats-package.relationships+xml", "rels")]
		public void TestLoadDefaults(string contentType, string ext)
		{
			ContentTypes ct;
			var assembly = Assembly.GetExecutingAssembly();
			using (var xmlFile = assembly.GetManifestResourceStream("MOPE.Tests.Collateral.[Content_Types].xml"))
			{
				ct = ContentTypes.Load(xmlFile);
			}

			Assert.Equal(contentType, ct.Defaults[ext]);
			Assert.Equal(contentType, ct.GetContentType($"foo.{ext}"));
		}
	}
}
