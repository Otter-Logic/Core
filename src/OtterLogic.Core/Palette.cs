namespace OtterLogic.Core;

/// <summary>
/// The colours OtterLogic draws with, held once for every tool and both
/// front-ends, as packed ARGB.
/// <para>
/// Here rather than in an adaptor for the reason <see cref="Sections"/> is: both
/// front-ends and several toolkits say the same thing with them — a Grasshopper
/// preview, a Rhino command's layers, the layers a bake creates — and a colour
/// that means "moved" in one tool and "issue" in the next teaches a user nothing.
/// Here rather than in StructuralEngine because colour is not a reading of a
/// structure: Structural Form, the document tools and the BIM diff have no
/// structure to read and would otherwise have to take one on to get a colour.
/// </para>
/// <para>
/// Packed ARGB rather than <c>System.Drawing.Color</c>, as the document tools
/// already hand colours about, so nothing below the adaptors takes a drawing
/// dependency; an adaptor unpacks one with <c>Color.FromArgb</c>.
/// </para>
/// </summary>
public static class Palette
{
    /// <summary>
    /// Context: what a preview is not about — unchanged elements, clean ones,
    /// elements outside every group. Mid grey reads on both of Grasshopper's canvas
    /// greys and in a shaded Rhino viewport without competing with anything else.
    /// </summary>
    public const int Grey = unchecked((int)0xFF8C8C8C);

    /// <summary>The thing a tool is about, or a change of shape — a modified element.</summary>
    public const int Blue = unchecked((int)0xFF2473C0);

    /// <summary>The second thing a tool is about, or a change of place — a moved element.</summary>
    public const int Orange = unchecked((int)0xFFE8872B);

    /// <summary>Something new — an added element.</summary>
    public const int Green = unchecked((int)0xFF2E9E6B);

    /// <summary>
    /// Attention: an issue to fix, or an element that has gone. Held back from every
    /// other use, so red in a preview always means look here.
    /// </summary>
    public const int Red = unchecked((int)0xFFD63636);

    /// <summary>
    /// Turbo at eleven even stops, 0 to 1 — the rainbow Grasshopper's own Gradient
    /// draws by default, so a user sees the same ramp in a preview as on the canvas.
    /// Turbo rather than the hue wheel because its lightness rises and falls
    /// smoothly: neighbouring groups never jump from a dark hue to a light one, and
    /// no band of greens runs together the way it does round the wheel.
    /// </summary>
    private static readonly int[] Turbo =
    {
        0x30123B, 0x4454C4, 0x4490FE, 0x1FC8DE, 0x29EFA2, 0x7EFF55,
        0xC3F133, 0xF1CA3A, 0xFE922A, 0xEA4F0D, 0x7A0403,
    };

    /// <summary>
    /// How much of each end of the ramp <see cref="Spread"/> leaves unused. Turbo's
    /// ends are nearly black, and a group coloured nearly black disappears against a
    /// dark viewport and reads as unshaded against a light one.
    /// </summary>
    private const double Trim = 0.08;

    private const double GoldenAngle = 137.50776405003785;

    /// <summary>The rainbow at <paramref name="t"/>, clamped to 0 to 1: blue at 0, green at a half, red at 1.</summary>
    public static int Rainbow(double t)
    {
        if (double.IsNaN(t))
            t = 0.5;

        double at = Math.Clamp(t, 0.0, 1.0) * (Turbo.Length - 1);
        int k = Math.Min((int)at, Turbo.Length - 2);
        double f = at - k;

        int Channel(int shift)
        {
            int a = (Turbo[k] >> shift) & 0xFF, b = (Turbo[k + 1] >> shift) & 0xFF;
            return (int)Math.Round(a + (b - a) * f);
        }

        return Pack(Channel(16), Channel(8), Channel(0));
    }

    /// <summary>
    /// Colour <paramref name="index"/> of <paramref name="count"/>, spread evenly
    /// along the rainbow — first blue, last red, one alone green. For anything with
    /// an order or a known number of groups: stages, levels, types.
    /// </summary>
    public static int Spread(int index, int count)
    {
        if (count <= 1)
            return Rainbow(0.5);

        return Rainbow(Trim + (1.0 - 2.0 * Trim) * Math.Clamp(index, 0, count - 1) / (count - 1));
    }

    /// <summary>
    /// One distinct colour per index, for when the number of groups is not known as
    /// the first colour is handed out.
    /// <para>
    /// Hues stepped by the golden angle rather than evenly divided, because an even
    /// division of the wheel would have to be redone every time the count changed.
    /// The golden step keeps any run of consecutive indices spread round the wheel,
    /// so groups 0 to 4 are as far apart in colour as 0 to 12 are. Saturation and
    /// value are fixed where the colours read on both canvas greys and in a
    /// viewport. Moved here from the Document repo's <c>GroupPalette</c> on
    /// 2026-09-26, unchanged, when the other palettes joined it.
    /// </para>
    /// </summary>
    public static int Distinct(int index)
    {
        double hue = ((index * GoldenAngle) % 360.0 + 360.0) % 360.0;
        var (r, g, b) = FromHsv(hue, 0.65, 0.85);
        return Pack(r, g, b);
    }

    /// <summary>Packs opaque red, green and blue, 0 to 255 each, into ARGB.</summary>
    public static int Pack(int r, int g, int b)
        => unchecked((int)0xFF000000) | (Math.Clamp(r, 0, 255) << 16) | (Math.Clamp(g, 0, 255) << 8) | Math.Clamp(b, 0, 255);

    private static (int R, int G, int B) FromHsv(double hue, double saturation, double value)
    {
        double c = value * saturation;
        double x = c * (1.0 - Math.Abs(hue / 60.0 % 2.0 - 1.0));
        double m = value - c;

        var (r, g, b) = (int)(hue / 60.0) switch
        {
            0 => (c, x, 0.0),
            1 => (x, c, 0.0),
            2 => (0.0, c, x),
            3 => (0.0, x, c),
            4 => (x, 0.0, c),
            _ => (c, 0.0, x),
        };

        return ((int)Math.Round((r + m) * 255.0), (int)Math.Round((g + m) * 255.0), (int)Math.Round((b + m) * 255.0));
    }
}
