namespace ModManager.Extensions;

public static class ColorExtensions
{
    public static Color WithAlpha(this Color color, byte alpha)
    {
        return Color.FromArgb(alpha, color.R, color.G, color.B);
    }

    public static Color WithAlpha(this Color color, double alpha)
    {
        if (alpha is < 0 or > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(alpha), "'Alpha' must be between 0 and 1.");
        }

        return Color.FromArgb((byte) (alpha * 255), color.R, color.G, color.B);
    }

    /// <summary>
    /// Lerp -> linear color interpolation
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="leanTo"></param>
    /// <returns></returns>
    public static Color LerpColors(this Color from, Color to, double leanTo)
    {
        if (leanTo is < 0 or > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(leanTo), "'LeanTo' must be between 0 and 1.");
        }

        var alpha = (byte) (from.A + (to.A - from.A) * leanTo);
        var red = (byte) (from.R + (to.R - from.R) * leanTo);
        var green = (byte) (from.G + (to.G - from.G) * leanTo);
        var blue = (byte) (from.B + (to.B - from.B) * leanTo);

        return Color.FromArgb(alpha, red, green, blue);
    }
}
