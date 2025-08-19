using System.Reflection.Metadata;
using CommunityToolkit.WinUI.UI.Controls;
using ModManager.Abstractions.Models;
using ModManager.Extensions;
using ModManager.Presentation.Converter;
using ModManager.Presentation.Factory;
using ModManager.Presentation.Logic;

namespace ModManager.Presentation.UserInterface;

public class PlaylistSelectorUserInterface
{
    //private const string DELETE_UNICODE = "&#xE710;";

    private enum DataGridColumns
    {
        PLAYSETS = 0,
        ACTIONS = 1,
    }

    private readonly PlaylistSelectorLogic logic;
    private readonly ViewModel.PlaylistSelectorViewModel viewModel;

    public PlaylistSelectorUserInterface(PlaylistSelectorLogic logic, ViewModel.PlaylistSelectorViewModel viewModel)
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

        Button button = CreatePanelToggleButton();
        DataGrid dataGrid = CreateDataGrid();

        panel.Children.Add(button);
        panel.Children.Add(dataGrid);

        return panel;
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

        button.SetBinding(FrameworkElement.TagProperty,
            new Binding {Path = new PropertyPath(nameof(IPlayset.FileName)),});
        button.Click += logic.RenameButtonClicked;
        return button;
    }

    private Button CreateDeleteButton()
    {
        Button button = ButtonFactory.CreateFontIconButton(Constants.Glyphs.DELETE_SYMBOL_UNICODE);
        button.Margin = new Thickness(2);

        button.SetBinding(FrameworkElement.TagProperty,
            new Binding {Path = new PropertyPath(nameof(IPlayset.FileName)),});

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

    public Grid CreateContentGrid()
    {
        Grid grid = GridFactory.CreateDefaultGrid();
        grid.DefineColumns(sizes: [50, 50,]).DefineRows(sizes: [80, 20,]);

        var currentStatus = ActivatorUtilities.CreateInstance<CurrentStatusRegion>(App.Startup.ServiceProvider);
        var editPlaylist = ActivatorUtilities.CreateInstance<EditPlaylistRegion>(App.Startup.ServiceProvider);

        grid.Children.Add(editPlaylist.SetRow(0).SetColumn(0));
        grid.Children.Add(currentStatus.SetRow(0).SetColumn(1));

        return grid;
    }
}
