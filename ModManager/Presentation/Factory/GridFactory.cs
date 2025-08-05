namespace ModManager.Presentation.Factory;

public static class GridFactory
{
    public static Grid CreateDefaultGrid()
    {
        return new Grid()
        {
            Margin = new Thickness(5),
        };
    }
}
