using CommunityToolkit.WinUI.UI.Controls;
using ModManager.Abstractions.Models;
using ModManager.Extensions;
using ModManager.Presentation.Converter;

namespace ModManager.Presentation.Factory;

public static class ListViewFactory
{
    public static Grid CreateListView(
        IList<int> columnSizes, IList<TextBlock> columnHeaders, Func<Border> templateFactory, Binding itemSourceBinding)
    {
        Grid grid = CreateGridForListView(columnSizes);
        PositionColumnHeaders(grid, columnHeaders);

        var listView = new ListView()
        {
            ItemTemplate = new DataTemplate(templateFactory),
            Margin = new Thickness(5),
        };

        listView.ScrollViewer((builder) => builder.VerticalScrollBarVisibility(ScrollBarVisibility.Hidden));

        listView.SetBinding(ItemsControl.ItemsSourceProperty, itemSourceBinding);

        grid.Add(listView.SetRow(1).SetColumn(0, columnSizes.Count));

        return grid;
    }

    private static void PositionColumnHeaders(Grid grid, IList<TextBlock> columnHeaders)
    {
        columnHeaders.ForEach((index, header) => grid.Add(header.SetRow(0).SetColumn(index)));
    }

    private static Grid CreateGridForListView(IList<int> columnSizes)
    {
        Grid grid = GridFactory.CreateDefaultGrid();

        grid.DefineColumns(sizes: columnSizes.ToArray());

        grid.RowDefinitions.Add(new RowDefinition() {Height = new GridLength(10, GridUnitType.Auto),});
        grid.DefineRows(sizes: [90,]);

        return grid;
    }

    public static Border BuildListViewTemplate(IList<int> columnSizes, IList<Grid> columnTemplates)
    {
        var border = new Border()
        {
            BorderBrush = new SolidColorBrush(Constants.UiColors.RowBorderColor),
            BorderThickness = new Thickness(1),
        };

        var backgroundBinding = new Binding()
        {
            Path = nameof(IMod.IsEnabled),
            Converter = new BooleanToBrushConverter()
            {
                TrueBrush = new SolidColorBrush(Constants.UiColors.OnRowColor),
                FalseBrush = new SolidColorBrush(Constants.UiColors.OffRowColor),
            },
        };

        border.SetBinding(FrameworkElement.BackgroundProperty, backgroundBinding);

        Grid rowGrid = GridFactory.CreateDefaultGrid().DefineColumns(sizes: columnSizes.ToArray())
            .DefineRows(sizes: [100,]);
        rowGrid.BorderThickness = new Thickness(1);

        columnTemplates.ForEach((index, panel) =>
        {
            var cellBorder = new Border
            {
                BorderBrush = new SolidColorBrush(Constants.UiColors.RowBorderColor),
                BorderThickness = new Thickness(index == 0 ? 0 : 1, 0, 0, 0),
                Child = panel,
            };

            rowGrid.Children.Add(cellBorder.SetColumn(index));
        });

        border.Child = rowGrid;

        return border;
    }
}
