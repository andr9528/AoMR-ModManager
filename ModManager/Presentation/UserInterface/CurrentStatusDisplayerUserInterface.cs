using CommunityToolkit.WinUI.UI.Controls;
using ModManager.Abstractions.Models;
using ModManager.Abstractions.Services;
using ModManager.Extensions;
using ModManager.Presentation.Converter;
using ModManager.Presentation.Core;
using ModManager.Presentation.Factory;
using ModManager.Presentation.Logic;
using ModManager.Presentation.ViewModel;
using ModManager.Strings;
using Uno.Toolkit.WinUI.Markup;
using Border = Microsoft.UI.Xaml.Controls.Border;

namespace ModManager.Presentation.UserInterface;

public class
    CurrentStatusDisplayerUserInterface : BaseDisplayerUserInterface<CurrentStatusDisplayerLogic,
    CurrentStatusDisplayerViewModel>
{
    private readonly ITranslationService translationService;

    private enum DataGridColumns
    {
        ACTIONS = 0,
        MODS = 1,
        INDICATORS = 2,
    }

    public CurrentStatusDisplayerUserInterface(
        CurrentStatusDisplayerLogic logic, ITranslationService translationService,
        CurrentStatusDisplayerViewModel viewModel) : base(logic, viewModel)
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

    private TextBlock CreateHeader()
    {
        TextBlock label = TextBlockFactory.CreateLabel(translationService[ResourceKeys.Status.HEADER]);
        label.HorizontalAlignment = HorizontalAlignment.Center;
        label.FontSize = Constants.Fonts.SECTION_HEADER_FONT_SIZE;

        return label;
    }

    private Grid CreateListViewGrid()
    {
        var columns = Enum.GetValues<DataGridColumns>();
        var columnHeaders = columns.Select(BuildColumnHeader).ToList();
        IList<int> columnSizes = [30, 70, 50,];

        var sourceBinding = new Binding()
        {
            Path =
                $"{nameof(ViewModel.StateService)}.{nameof(ViewModel.StateService.CurrentModStatus)}.{nameof(ViewModel.StateService.CurrentModStatus.Mods)}",
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

    private TextBlock BuildColumnHeader(DataGridColumns column)
    {
        return TextBlockFactory.CreateLabel(translationService[GetColumnTitleKey(column)]);
    }

    private string GetColumnTitleKey(DataGridColumns column)
    {
        return column switch
        {
            DataGridColumns.ACTIONS => ResourceKeys.Status.COLUMN_ONE_HEADER,
            DataGridColumns.MODS => ResourceKeys.Status.COLUMN_TWO_HEADER,
            DataGridColumns.INDICATORS => ResourceKeys.Status.COLUMN_THREE_HEADER,
            var _ => throw new ArgumentOutOfRangeException(nameof(column), column, null),
        };
    }

    private Grid BuildActionsTemplate()
    {
        Grid panel = GridFactory.CreateLeftAlignedGrid();

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

        var tagBinding = new Binding();

        button.SetBinding(UIElement.IsHitTestVisibleProperty, hitTestBinding);
        button.SetBinding(FrameworkElement.BackgroundProperty, backgroundBinding);
        button.SetBinding(FrameworkElement.TagProperty, tagBinding);

        button.Click += Logic.AddModClicked;

        return button;
    }
}
