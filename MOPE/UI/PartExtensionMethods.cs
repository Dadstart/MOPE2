﻿using B4.Mope.Packaging;
using System.Web;

namespace B4.Mope.UI
{
	public static class PartExtensionMethods
	{
		public static string GetMonacoUrl(this Part part)
		{
			return $"monaco/editor.html?part={HttpUtility.UrlEncode(part.Uri)}";
		}

		public static bool CanViewInBrowser(this Part part)
		{
			var contentType = part?.ContentType;
			return ContentTypes.IsXmlType(contentType)
				|| ContentTypes.IsSupportedAudioType(contentType)
				|| ContentTypes.IsSupportedVideoType(contentType)
				|| ContentTypes.IsSupportedImageType(contentType);
		}
	}
}
