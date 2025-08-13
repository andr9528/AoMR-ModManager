namespace ModManager.Presentation.Factory;

public static class TextBlockFactory
{
    public static TextBlock CreateLabel(string text)
    {
        TextBlock box = CreateDefaultTextBlock();

        box.Text = text;

        return box;
    }

    public static TextBlock CreateDefaultTextBlock()
    {
        return new TextBlock()
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Foreground = new SolidColorBrush(Colors.Black),
        };
    }
}
