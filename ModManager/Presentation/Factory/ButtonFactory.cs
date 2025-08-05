namespace ModManager.Presentation.Factory;

public static class ButtonFactory
{
    public static Button CreateDefaultButton()
    {
        return new Button
        {
            Margin = new Thickness(10),
            Padding = new Thickness(10, 5, 10, 5),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
        };
    }
}
