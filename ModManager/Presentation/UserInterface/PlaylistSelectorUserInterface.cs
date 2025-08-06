using CommunityToolkit.WinUI.UI.Controls;
using ModManager.Abstractions.Models;
using ModManager.Abstractions.Services;
using ModManager.Presentation.Extensions;
using ModManager.Presentation.Factory;

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
    private readonly PlaylistSelectorViewModel dataContext;

    public PlaylistSelectorUserInterface(
        PlaylistSelectorLogic logic, PlaylistSelectorViewModel dataContext, IFileService fileService,
        IStateService stateService)
    {
        this.logic = logic;
        this.dataContext = dataContext;
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
        var converter = new BoolToSymbolIconConverter() {TrueSymbol = Symbol.OpenPane, FalseSymbol = Symbol.ClosePane,};
        button.SetBinding(ContentControl.ContentProperty,
            new Binding() {Path = new PropertyPath(nameof(dataContext.IsMenuOpen)), Converter = converter,});

        button.Click += (sender, args) => dataContext.IsMenuOpen = !dataContext.IsMenuOpen;
        return button;
    }

    private DataGrid CreateDataGrid()
    {
        var columns = Enum.GetValues<DataGridColumns>().Select(BuildColumn).ToList();
        DataGrid dataGrid = DataGridFactory.CreateDataGrid(dataContext,
            $"{nameof(dataContext.StateService)}.{nameof(dataContext.StateService.Playsets)}", columns);

        dataGrid.SelectionMode = DataGridSelectionMode.Single;
        dataGrid.AddHandler(UIElement.PointerPressedEvent, new PointerEventHandler(logic.DataGridPreviewPointerPressed),
            true);

        dataContext.IsMenuOpenChanged += (sender, e) => logic.UpdateDataGridColumnVisibility(e, dataGrid);
        logic.UpdateDataGridColumnVisibility(dataContext.IsMenuOpen, dataGrid);

        dataGrid.SelectionChanged += logic.DataGridRowSelectionChanged;

        return dataGrid;
    }

    private DataGridColumn BuildColumn(DataGridColumns dataGridColumns)
    {
        return dataGridColumns switch
        {
            DataGridColumns.PLAYSETS => BuildTitleColumn(),
            DataGridColumns.ACTIONS => BuildActionsColumn(),
            var _ => throw new ArgumentOutOfRangeException(nameof(dataGridColumns), dataGridColumns, null),
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
        Button button = ButtonFactory.CreateFontIconButton(ButtonFactory.RENAME_SYMBOL_UNICODE);
        button.Margin = new Thickness(2);

        button.SetBinding(FrameworkElement.TagProperty,
            new Binding {Path = new PropertyPath(nameof(IPlayset.FileName)),});
        button.Click += logic.RenameButtonClicked;
        return button;
    }

    private Button CreateDeleteButton()
    {
        Button button = ButtonFactory.CreateFontIconButton(ButtonFactory.DELETE_SYMBOL_UNICODE);
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

        return grid;
    }
}
