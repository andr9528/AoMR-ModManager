using System.Reflection.Metadata;
using CommunityToolkit.WinUI.UI.Controls;
using ModManager.Abstractions.Models;
using ModManager.Extensions;
using ModManager.Presentation.Converter;
using ModManager.Presentation.Core;
using ModManager.Presentation.Factory;
using ModManager.Presentation.Logic;

namespace ModManager.Presentation.UserInterface;

public class PlaysetSelectorUserInterface : BaseUserInterface
{
    private enum DataGridColumns
    {
        PLAYSETS = 0,
        ACTIONS = 1,
    }

    private readonly PlaysetSelectorLogic logic;
    private readonly ViewModel.PlaysetSelectorViewModel viewModel;

    public PlaysetSelectorUserInterface(PlaysetSelectorLogic logic, ViewModel.PlaysetSelectorViewModel viewModel)
    {
        this.logic = logic;
        this.viewModel = viewModel;
    }

    public StackPanel CreateNavigationPanel()
    {
        var panel = new StackPanel
        {
            Orientation = Orientation.Vertical,
            Margin = new Thickness(10),
        };

        StackPanel togglePanel = CreateTogglePanel();
        DataGrid dataGrid = CreateDataGrid();

        panel.Children.Add(togglePanel);
        panel.Children.Add(dataGrid);

        return panel;
    }

    private StackPanel CreateTogglePanel()
    {
        var panel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
        };

        Button panelToggleButton = CreatePanelToggleButton();
        Button newPlaysetButton = CreateNewPlaysetButton();

        panel.Children.Add(panelToggleButton);
        panel.Children.Add(newPlaysetButton);

        return panel;
    }

    private Button CreateNewPlaysetButton()
    {
        var icon = new FontIcon()
        {
            FontSize = 12,
            Glyph = Constants.Glyphs.ADD_SYMBOL_UNICODE,
            Foreground = new SolidColorBrush(Constants.UiColors.AddFontColor),
        };

        Button button = ButtonFactory.CreateFontIconButton(icon);
        button.HorizontalAlignment = HorizontalAlignment.Left;
        button.Margin = new Thickness(2, 0, 0, 0);

        button.Click += logic.NewPlaysetButtonClicked;

        return button;
    }

    private Button CreatePanelToggleButton()
    {
        Button button = ButtonFactory.CreateDefaultButton();
        button.HorizontalAlignment = HorizontalAlignment.Left;

        var binding = new Binding()
        {
            Path = nameof(viewModel.IsMenuOpen),
            Converter = new BooleanToSymbolIconConverter()
            {
                TrueSymbol = Symbol.OpenPane,
                FalseSymbol = Symbol.ClosePane,
            },
        };

        button.SetBinding(ContentControl.ContentProperty, binding);

        button.Click += (sender, args) => viewModel.IsMenuOpen = !viewModel.IsMenuOpen;
        return button;
    }

    private DataGrid CreateDataGrid()
    {
        var columns = Enum.GetValues<DataGridColumns>().Select(BuildColumn).ToList();
        DataGrid dataGrid = DataGridFactory.CreateDataGrid(viewModel,
            $"{nameof(viewModel.StateService)}.{nameof(viewModel.StateService.Playsets)}", columns);

        dataGrid.SelectionMode = DataGridSelectionMode.Single;
        dataGrid.AddHandler(UIElement.PointerPressedEvent, new PointerEventHandler(logic.DataGridPreviewPointerPressed),
            true);

        viewModel.IsMenuOpenChanged += (sender, e) => logic.UpdateDataGridColumnVisibility(e, dataGrid);
        logic.UpdateDataGridColumnVisibility(viewModel.IsMenuOpen, dataGrid);

        dataGrid.SelectionChanged += logic.DataGridRowSelectionChanged;

        return dataGrid;
    }

    private DataGridColumn BuildColumn(DataGridColumns column)
    {
        return column switch
        {
            DataGridColumns.PLAYSETS => BuildTitleColumn(),
            DataGridColumns.ACTIONS => BuildActionsColumn(),
            var _ => throw new ArgumentOutOfRangeException(nameof(column), column, null),
        };
    }

    private DataGridTemplateColumn BuildActionsColumn()
    {
        var template = new DataTemplate(BuildActionsTemplate);

        return new DataGridTemplateColumn()
        {
            Header = DataGridColumns.ACTIONS.ToString().ScreamingSnakeCaseToTitleCase(),
            CellTemplate = template,
            Width = new DataGridLength(60, DataGridLengthUnitType.Star),
        };
    }

    private StackPanel? BuildActionsTemplate()
    {
        var panel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
        };

        Button renameButton = CreateRenameButton();
        Button deleteButton = CreateDeleteButton();

        panel.Children.Add(renameButton);
        panel.Children.Add(deleteButton);

        return panel;
    }

    private Button CreateRenameButton()
    {
        Button button = ButtonFactory.CreateFontIconButton(Constants.Glyphs.RENAME_SYMBOL_UNICODE);
        button.Margin = new Thickness(2);

        var tagBinding = new Binding();

        button.SetBinding(FrameworkElement.TagProperty, tagBinding);

        button.Click += logic.RenameButtonClicked;
        return button;
    }

    private Button CreateDeleteButton()
    {
        Button button = ButtonFactory.CreateFontIconButton(Constants.Glyphs.DELETE_SYMBOL_UNICODE);
        button.Margin = new Thickness(2);

        var tagBinding = new Binding();

        button.SetBinding(FrameworkElement.TagProperty, tagBinding);

        button.Click += logic.DeleteButtonClicked;
        return button;
    }

    private DataGridTextColumn BuildTitleColumn()
    {
        return new DataGridTextColumn()
        {
            Header = DataGridColumns.PLAYSETS.ToString().ScreamingSnakeCaseToTitleCase(),
            Binding = new Binding {Path = new PropertyPath(nameof(IPlayset.FileName)),},
            Width = new DataGridLength(40, DataGridLengthUnitType.Star),
            FontSize = 12,
        };
    }

    /// <inheritdoc />
    protected override void ConfigureContentGrid(Grid grid)
    {
        grid.DefineColumns(sizes: [50, 50,]).DefineRows(sizes: [85, 15,]);
    }

    /// <inheritdoc />
    protected override void AddChildrenToGrid(Grid grid)
    {
        var currentStatusRegion = ActivatorUtilities.CreateInstance<CurrentStatusRegion>(App.Startup.ServiceProvider);
        var editPlaysetRegion = ActivatorUtilities.CreateInstance<EditPlaysetRegion>(App.Startup.ServiceProvider);
        var playsetActionsRegion = ActivatorUtilities.CreateInstance<PlaysetActionsRegion>(App.Startup.ServiceProvider);

        grid.Children.Add(editPlaysetRegion.SetRow(0).SetColumn(0));
        grid.Children.Add(currentStatusRegion.SetRow(0).SetColumn(1));
        grid.Children.Add(playsetActionsRegion.SetRow(1).SetColumn(0, 2));
    }
}
