using CommunityToolkit.WinUI.UI.Controls;
using ModManager.Abstractions.Models;
using ModManager.Extensions;
using ModManager.Presentation.Converter;
using ModManager.Presentation.Factory;
using ModManager.Presentation.Logic;
using ModManager.Presentation.ViewModel;
using Uno.Toolkit.WinUI.Markup;

namespace ModManager.Presentation.UserInterface;

public class CurrentStatusDisplayerUserInterface
{
    private enum DataGridColumns
    {
        ACTIONS = 0,
        MODS = 1,
    }

    private readonly CurrentStatusDisplayerLogic logic;
    private readonly CurrentStatusDisplayerViewModel viewModel;

    public CurrentStatusDisplayerUserInterface(
        CurrentStatusDisplayerLogic logic, CurrentStatusDisplayerViewModel viewModel)
    {
        this.logic = logic;
        this.viewModel = viewModel;
    }

    public Grid CreateContentGrid()
    {
        Grid grid = GridFactory.CreateDefaultGrid();
        grid.DefineColumns(sizes: [100,]).DefineRows(sizes: [100,]);

        DataGrid dataGrid = CreateDataGrid();

        grid.Children.Add(dataGrid.SetRow(0).SetColumn(0));

        return grid;
    }

    private DataGrid CreateDataGrid()
    {
        var columns = Enum.GetValues<DataGridColumns>().Select(BuildColumn).ToList();
        DataGrid dataGrid = DataGridFactory.CreateDataGrid(viewModel,
            $"{nameof(viewModel.StateService)}.{nameof(viewModel.StateService.CurrentModStatus)}.{nameof(viewModel.StateService.CurrentModStatus.Mods)}",
            columns);

        dataGrid.SelectionMode = DataGridSelectionMode.Single;
        dataGrid.IsTabStop = false;
        dataGrid.SelectionChanged += logic.DataGridRowSelectionChanged;
        dataGrid.CellStyle = CreateCellStyle();

        dataGrid.LoadingRow += logic.DataGridLoadedRow;

        return dataGrid;
    }


    private Style CreateCellStyle()
    {
        var cellStyle = new Style(typeof(DataGridCell));

        cellStyle.Setters.Add(new Setter(FrameworkElement.BackgroundProperty, new SolidColorBrush(Colors.Transparent)));
        cellStyle.Setters.Add(new Setter(Control.BorderBrushProperty, new SolidColorBrush(Colors.Transparent)));
        cellStyle.Setters.Add(new Setter(Control.BorderThicknessProperty, new Thickness(0)));
        cellStyle.Setters.Add(new Setter(FrameworkElement.FocusVisualPrimaryBrushProperty,
            new SolidColorBrush(Colors.Transparent)));
        cellStyle.Setters.Add(new Setter(FrameworkElement.FocusVisualSecondaryBrushProperty,
            new SolidColorBrush(Colors.Transparent)));
        cellStyle.Setters.Add(new Setter(UIElement.UseSystemFocusVisualsProperty, false));
        cellStyle.Setters.Add(new Setter(UIElement.IsTabStopProperty, false));

        return cellStyle;
    }

    private DataGridColumn BuildColumn(DataGridColumns column)
    {
        return column switch
        {
            DataGridColumns.ACTIONS => BuildActionsColumn(),
            DataGridColumns.MODS => BuildModsColumn(),
            var _ => throw new ArgumentOutOfRangeException(nameof(column), column, null),
        };
    }

    private DataGridTextColumn BuildModsColumn()
    {
        return new DataGridTextColumn()
        {
            Header = DataGridColumns.MODS.ToString().ScreamingSnakeCaseToTitleCase(),
            Binding = new Binding {Path = new PropertyPath(nameof(IMod.Title)),},
            Width = new DataGridLength(70, DataGridLengthUnitType.Star),
            FontSize = 12,
            Foreground = new SolidColorBrush(Colors.Black),
        };
    }

    private DataGridTemplateColumn BuildActionsColumn()
    {
        var template = new DataTemplate(BuildActionsTemplate);

        return new DataGridTemplateColumn()
        {
            Header = DataGridColumns.ACTIONS.ToString().ScreamingSnakeCaseToTitleCase(),
            CellTemplate = template,
            Width = new DataGridLength(30, DataGridLengthUnitType.Star),
        };
    }

    private StackPanel? BuildActionsTemplate()
    {
        var panel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
        };

        Button addModButton = CreateAddModButton();

        panel.Children.Add(addModButton);

        return panel;
    }

    private Button CreateAddModButton()
    {
        Button button = ButtonFactory.CreateFontIconButton(ButtonFactory.LEFT_ARROW_SYMBOL_UNICODE)
            .ApplyFilledButtonStyle().ApplyDisabledButtonStyle();

        var enabledBinding = new Binding()
        {
            Path = nameof(IMod.IsHiddenSibling),
        };

        var tagBinding = new Binding
        {
            Path = nameof(IMod.WorkshopId),
        };

        button.SetBinding(Control.IsEnabledProperty, enabledBinding);
        button.SetBinding(FrameworkElement.TagProperty, tagBinding);

        button.Click += logic.AddModClicked;

        return button;
    }
}
