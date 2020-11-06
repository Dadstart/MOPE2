using B4.Mope.Packaging;
using System.Web;

namespace B4.Mope.UI
{
	public static class PartExtensionMethods
	{
		public static string GetMonacoEditorUrl(this Part part)
		{
			return $"monaco/editor.html?part={HttpUtility.UrlEncode(part.Uri)}";
		}

		public static string GetMonacoDiffUrl(this DiffPart part)
		{
			var uri = part.Left?.Uri ?? part.Right?.Uri;
			return $"monaco/diff.html?part={HttpUtility.UrlEncode(uri)}";
		}

		public static bool CanViewInBrowser(this Part part)
		{
			var contentType = part?.ContentType;
			return part.IsAnyTextType()
				|| ContentTypes.IsSupportedAudioType(contentType)
				|| ContentTypes.IsSupportedVideoType(contentType)
				|| ContentTypes.IsSupportedImageType(contentType);
		}

		public static bool IsAnyTextType(this Part part)
		{
			var contentType = part?.ContentType;
			return ContentTypes.IsXmlType(contentType)
				|| ContentTypes.IsCodeType(contentType)
				|| ContentTypes.IsTextType(contentType);
		}
	}
}
