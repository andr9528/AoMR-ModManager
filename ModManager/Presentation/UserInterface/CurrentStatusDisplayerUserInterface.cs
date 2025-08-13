using CommunityToolkit.WinUI.UI.Controls;
using ModManager.Abstractions.Models;
using ModManager.Extensions;
using ModManager.Presentation.Converter;
using ModManager.Presentation.Core;
using ModManager.Presentation.Factory;
using ModManager.Presentation.Logic;
using ModManager.Presentation.ViewModel;
using Uno.Toolkit.WinUI.Markup;

namespace ModManager.Presentation.UserInterface;

public class
    CurrentStatusDisplayerUserInterface : BaseDisplayerUserInterface<CurrentStatusDisplayerLogic,
    CurrentStatusDisplayerViewModel>
{
    private enum DataGridColumns
    {
        ACTIONS = 0,
        MODS = 1,
        INDICATORS = 2,
    }

    public CurrentStatusDisplayerUserInterface(
        CurrentStatusDisplayerLogic logic, CurrentStatusDisplayerViewModel viewModel) : base(logic, viewModel)
    {
    }

    /// <inheritdoc />
    protected override void ConfigureContentGrid(Grid grid)
    {
        grid.DefineColumns(sizes: [100,]).DefineRows(sizes: [100,]);
    }

    /// <inheritdoc />
    protected override void AddChildrenToGrid(Grid grid)
    {
        DataGrid dataGrid = CreateDataGrid();

        grid.Children.Add(dataGrid.SetRow(0).SetColumn(0));
    }

    private DataGrid CreateDataGrid()
    {
        var columns = Enum.GetValues<DataGridColumns>().Select(BuildColumn).ToList();
        DataGrid dataGrid = DataGridFactory.CreateDataGrid(ViewModel,
            $"{nameof(ViewModel.StateService)}.{nameof(ViewModel.StateService.CurrentModStatus)}.{nameof(ViewModel.StateService.CurrentModStatus.Mods)}",
            columns);

        dataGrid.SelectionMode = DataGridSelectionMode.Single;
        dataGrid.IsTabStop = false;
        dataGrid.SelectionChanged += Logic.DataGridRowSelectionChanged;
        dataGrid.CellStyle = CreateCellStyle();

        dataGrid.LoadingRow += Logic.DataGridLoadedRow;

        return dataGrid;
    }


    private Style CreateCellStyle()
    {
        var cellStyle = new Style(typeof(DataGridCell));

        cellStyle.Setters.Add(new Setter(FrameworkElement.BackgroundProperty, new SolidColorBrush(Colors.Transparent)));
        cellStyle.Setters.Add(new Setter(Control.BorderBrushProperty,
            new SolidColorBrush(Constants.UiColors.RowBorderColor)));
        cellStyle.Setters.Add(new Setter(Control.BorderThicknessProperty, new Thickness(1)));
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
            DataGridColumns.ACTIONS => BuildActionsColumn(DataGridColumns.ACTIONS.ToString()),
            DataGridColumns.MODS => BuildModsColumn(DataGridColumns.MODS.ToString()),
            DataGridColumns.INDICATORS => BuildIndicatorsColumn(DataGridColumns.INDICATORS.ToString()),
            var _ => throw new ArgumentOutOfRangeException(nameof(column), column, null),
        };
    }

    /// <inheritdoc />
    protected override StackPanel BuildActionsTemplate()
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
        Button button = ButtonFactory.CreateFontIconButton(Constants.Glyphs.LEFT_ARROW_SYMBOL_UNICODE);

        var hitTestBinding = new Binding()
        {
            Path = nameof(IMod.IsHiddenSibling),
        };

        var backgroundBinding = new Binding()
        {
            Path = nameof(IMod.IsHiddenSibling),
            Converter = new BooleanToBrushConverter()
            {
                TrueBrush = new SolidColorBrush(Constants.UiColors.InteractableButtonColor),
                FalseBrush = new SolidColorBrush(Constants.UiColors.DisabledButtonColor),
            },
        };

        var tagBinding = new Binding
        {
            Path = nameof(IMod.WorkshopId),
        };

        button.SetBinding(UIElement.IsHitTestVisibleProperty, hitTestBinding);
        button.SetBinding(FrameworkElement.BackgroundProperty, backgroundBinding);
        button.SetBinding(FrameworkElement.TagProperty, tagBinding);

        button.Click += Logic.AddModClicked;

        return button;
    }
}
