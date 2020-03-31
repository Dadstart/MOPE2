using B4.Mope.Packaging;
using System;
using System.IO;
using System.Reflection;
using Xunit;

namespace MOPE.Tests
{
	public class RelationshipsTest
	{
		[Fact]
		public void TestLoad()
		{
			Relationships rels;
			var assembly = Assembly.GetExecutingAssembly();
			using (var xmlFile = assembly.GetManifestResourceStream("MOPE.Tests.Collateral..rels"))
			{
				rels = Relationships.Load(xmlFile);
			}

			AssertRel(rels["rId1"], "rId1", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument", "word/document.xml");
		}

		private void AssertRel(Relationship rel, string id, string type, string target)
		{
			Assert.Equal(id, rel.Id);
			Assert.Equal(type, rel.Type);
			Assert.Equal(target, rel.Target);
		}
	}
}
