namespace ModManager.Extensions;

public static class StringExtensions
{
    public static string ScreamingSnakeCaseToTitleCase(this string input)
    {
        return Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(string.Join(' ', input.Split('_')).ToLower());
    }
}
