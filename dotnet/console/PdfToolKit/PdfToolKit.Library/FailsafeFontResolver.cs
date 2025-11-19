using System.Diagnostics;
using PdfSharp.Fonts;

namespace PdfToolKit.Library;

/// <summary>
/// This font resolver maps each request to a valid font face of the SegoeWP fonts.
/// </summary>
public sealed class FailsafeFontResolver : IFontResolver
{
    public FontResolverInfo? ResolveTypeface(string familyName, bool isBold, bool isItalic)
    {
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