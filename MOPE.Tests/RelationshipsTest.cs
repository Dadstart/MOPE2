using B4.Mope.Packaging;
using Moq;
using System;
using System.IO;
using System.Reflection;
using Xunit;

namespace MOPE.Tests
{
	public class RelationshipsTest
	{
		[Theory]
		[InlineData("rId1", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument", "word/document.xml", false)]
		[InlineData("rId2", "http://schemas.openxmlformats.org/package/2006/relationships/metadata/core-properties", "docProps/core.xml", false)]
		[InlineData("rId3", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/extended-properties", "docProps/app.xml", false)]
		[InlineData("rId4", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/custom-properties", "docProps/custom.xml", false)]
		[InlineData("rId5", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/hyperlink", "https://www.bing.com", true)]
		public void TestLoad(string id, string type, string target, bool external)
		{
			Relationships rels;
			var assembly = Assembly.GetExecutingAssembly();
			using (var xmlFile = assembly.GetManifestResourceStream("MOPE.Tests.Collateral..rels"))
			{
				rels = Relationships.Load(Mock.Of<Package>(), null, xmlFile);
			}

			AssertRel(rels[id], id, type, target, external);
		}

		private void AssertRel(Relationship rel, string id, string type, string target, bool external)
		{
			Assert.Equal(id, rel.Id);
			Assert.Equal(type, rel.Type);
			Assert.Equal(target, rel.Target);
			Assert.Equal(external, rel.External);
		}

		[Theory]
		[InlineData("word/document.xml", "word/_rels/document.xml.rels")]
		[InlineData(null, "_rels/.rels")]
		public void TestGetRelationshipPartOwner(string source, string relsPart)
		{
			Assert.Equal(source, Relationships.GetRelationshipPartOwner(relsPart));
		}
	}
}
