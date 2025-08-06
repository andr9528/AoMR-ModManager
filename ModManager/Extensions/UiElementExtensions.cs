namespace ModManager.Extensions;

public static class UiElementExtensions
{
    public static T SetRow<T>(this T element, int row, int span = 1) where T : UIElement
    {
        Grid.SetRow(element, row);
        Grid.SetRowSpan(element, span);
        return element;
    }

    public static T SetColumn<T>(this T element, int column, int span = 1) where T : UIElement
    {
        Grid.SetColumn(element, column);
        Grid.SetColumnSpan(element, span);
        return element;
    }

    public static Grid DefineRows(this Grid grid, GridUnitType unitType = GridUnitType.Star, params int[] sizes)
    {
        if (!sizes.Any())
        {
            return grid;
        }

        foreach (int size in sizes)
        {
            grid.RowDefinitions.Add(new RowDefinition {Height = new GridLength(size, unitType),});
        }

        return grid;
    }

    public static Grid DefineColumns(this Grid grid, GridUnitType unitType = GridUnitType.Star, params int[] sizes)
    {
        if (!sizes.Any())
        {
            return grid;
        }

        foreach (int size in sizes)
        {
            grid.ColumnDefinitions.Add(new ColumnDefinition {Width = new GridLength(size, unitType),});
        }

        return grid;
    }
}
