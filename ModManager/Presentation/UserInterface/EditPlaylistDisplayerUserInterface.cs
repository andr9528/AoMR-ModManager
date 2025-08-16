using ModManager.Abstractions.Services;
using ModManager.Extensions;
using ModManager.Presentation.Core;
using ModManager.Presentation.Factory;
using ModManager.Strings;

namespace ModManager.Presentation;

public class EditPlaylistDisplayerUserInterface : BaseDisplayerUserInterface<EditPlaylistDisplayerLogic,
    EditPlaylistDisplayerViewModel>
{
    private enum DataGridColumns
    {
        MODS = 0,
        INDICATORS = 1,
        ACTIONS = 2,
    }

    private readonly ITranslationService translationService;

    public EditPlaylistDisplayerUserInterface(
        EditPlaylistDisplayerLogic logic, ITranslationService translationService,
        EditPlaylistDisplayerViewModel viewModel) : base(logic, viewModel)
    {
        this.translationService = translationService;
    }

    /// <inheritdoc />
    protected override void ConfigureContentGrid(Grid grid)
    {
        grid.RowDefinitions.Add(new RowDefinition() {Height = new GridLength(10, GridUnitType.Auto),});

        grid.DefineRows(sizes: [90,]);
    }

    /// <inheritdoc />
    protected override void AddChildrenToGrid(Grid grid)
    {
        TextBlock header = CreateHeader();

        Grid listViewGrid = CreateListViewGrid();

        grid.Children.Add(header.SetRow(0));
        grid.Children.Add(listViewGrid.SetRow(1));
    }

    private Grid CreateListViewGrid()
    {
        var columns = Enum.GetValues<DataGridColumns>();
        var columnHeaders = columns.Select(BuildColumnHeader).ToList();
        IList<int> columnSizes = [70, 50, 30,];

        var sourceBinding = new Binding()
        {
            Path = nameof(ViewModel.ShownMods),
        };

        return ListViewFactory.CreateListView(columnSizes.ToArray(), columnHeaders, TemplateFactory, sourceBinding);

        Border TemplateFactory()
        {
            return ListViewFactory.BuildListViewTemplate(columnSizes, columns.Select(BuildColumnTemplate).ToList());
        }
    }

    private Grid BuildColumnTemplate(DataGridColumns column)
    {
        return column switch
        {
            DataGridColumns.ACTIONS => BuildActionsTemplate(),
            DataGridColumns.MODS => BuildModsTemplate(),
            DataGridColumns.INDICATORS => BuildIndicatorsTemplate(),
            var _ => throw new ArgumentOutOfRangeException(nameof(column), column, null),
        };
    }

    private Grid BuildActionsTemplate()
    {
        Grid grid = GridFactory.CreateLeftAlignedGrid();

        Button addModButton = CreateRemoveModButton();

        grid.Children.Add(addModButton);

        return grid;
    }

    private Button CreateRemoveModButton()
    {
        Button button = ButtonFactory.CreateFontIconButton(Constants.Glyphs.TRASH_CAN_SYMBOL_UNICODE);
        button.Background = new SolidColorBrush(Constants.UiColors.InteractableButtonColor);

        var tagBinding = new Binding();

        button.SetBinding(FrameworkElement.TagProperty, tagBinding);

        button.Click += Logic.RemoveModClicked;

        return button;
    }

    private TextBlock BuildColumnHeader(DataGridColumns column)
    {
        return TextBlockFactory.CreateLabel(translationService[GetColumnTitleKey(column)]);
    }

    private string GetColumnTitleKey(DataGridColumns column)
    {
        return column switch
        {
            DataGridColumns.MODS => ResourceKeys.Edit.COLUMN_ONE_HEADER,
            DataGridColumns.INDICATORS => ResourceKeys.Edit.COLUMN_TWO_HEADER,
            DataGridColumns.ACTIONS => ResourceKeys.Edit.COLUMN_THREE_HEADER,
            var _ => throw new ArgumentOutOfRangeException(nameof(column), column, null),
        };
    }

    private TextBlock CreateHeader()
    {
        TextBlock label = TextBlockFactory.CreateDefaultTextBlock();

        var textBinding = new Binding()
        {
            Path = nameof(EditPlaylistDisplayerViewModel.HeaderText),
        };

        label.HorizontalAlignment = HorizontalAlignment.Center;
        label.FontSize = Constants.Fonts.SECTION_HEADER_FONT_SIZE;

        label.SetBinding(TextBlock.TextProperty, textBinding);

        return label;
    }
}
