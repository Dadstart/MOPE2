using B4.Mope.Packaging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace B4.Mope.UI
{
	public static class PartExtensionMethods
	{
		public static string GetMonacoUrl(this Part part)
		{
			return $"monaco/editor.html?part={HttpUtility.UrlEncode(part.Uri)}";
		}
	}
}
