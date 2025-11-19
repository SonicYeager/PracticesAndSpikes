using System.Diagnostics;
using PdfSharp.Fonts;

namespace PdfToolKit
{
    /// <summary>
    /// This font resolver maps each request to a valid font face of the SegoeWP fonts.
    /// </summary>
    public class FailsafeFontResolver : IFontResolver
    {
        public FontResolverInfo? ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            string typefaceName =
                $"{familyName}{(isBold ? " bold" : "")}{(isItalic ? " italic" : "")}";

            // Use either SegoeWP or SegoeWPBold.
            var result = SegoeWpFontResolver.ResolveTypeface(
                isBold
                    ? SegoeWpFontResolver.FamilyNames.SegoeWPBold
                    : SegoeWpFontResolver.FamilyNames.SegoeWP,
                false, isItalic);

            Debug.Assert(result != null);

            return result;
        }

        public byte[]? GetFont(string faceName)
        {
            return SegoeWpFontResolver.GetFont(faceName);
        }

        static readonly SegoeWpFontResolver SegoeWpFontResolver = new();
    }
}
