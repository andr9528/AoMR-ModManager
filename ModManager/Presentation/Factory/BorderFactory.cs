namespace ModManager.Presentation.Factory;

public static class BorderFactory
{
    public static Border ConfigureDefaultBorder(this Border border)
    {
        border.HorizontalAlignment = HorizontalAlignment.Stretch;
        border.VerticalAlignment = VerticalAlignment.Stretch;
        border.Margin = new Thickness(5);
        border.BorderThickness = new Thickness(1);
        border.BorderBrush = new SolidColorBrush(Colors.Black);

        return border;
    }
}
