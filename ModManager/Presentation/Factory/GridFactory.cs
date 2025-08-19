namespace ModManager.Presentation.Factory;

public static class GridFactory
{
    public static Grid CreateDefaultGrid()
    {
        return new Grid()
        {
            Margin = new Thickness(2),
            IsTabStop = false,
        };
    }

    public static Grid CreateLeftAlignedGrid()
    {
        Grid grid = CreateDefaultGrid();

        grid.HorizontalAlignment = HorizontalAlignment.Left;

        return grid;
    }
}
