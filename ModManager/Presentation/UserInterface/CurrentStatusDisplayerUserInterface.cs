using CommunityToolkit.WinUI.UI.Controls;
using ModManager.Abstractions.Models;
using ModManager.Extensions;
using ModManager.Presentation.Converter;
using ModManager.Presentation.Core;
using ModManager.Presentation.Factory;
using ModManager.Presentation.Logic;
using ModManager.Presentation.ViewModel;
using Uno.Toolkit.WinUI.Markup;
using Border = Microsoft.UI.Xaml.Controls.Border;

namespace ModManager.Presentation.UserInterface;

public class
    CurrentStatusDisplayerUserInterface : BaseDisplayerUserInterface<CurrentStatusDisplayerLogic,
    CurrentStatusDisplayerViewModel>
{
    private readonly int[] columnSizes = [30, 70, 30,];

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
        grid.DefineColumns(sizes: columnSizes);

        grid.RowDefinitions.Add(new RowDefinition() {Height = new GridLength(10, GridUnitType.Auto),});
        grid.RowDefinitions.Add(new RowDefinition() {Height = new GridLength(10, GridUnitType.Auto),});

        grid.DefineRows(sizes: [80,]);
    }

    /// <inheritdoc />
    protected override void AddChildrenToGrid(Grid grid)
    {
        TextBlock headerOneLabel = CreateActionsLabel();
        TextBlock headerTwoLabel = CreateModsLabel();
        TextBlock headerThreeLabel = CreateIndicatorsLabel();
        ListView lisView = CreateListView();

        grid.Children.Add(headerOneLabel.SetRow(1).SetColumn(0));
        grid.Children.Add(headerTwoLabel.SetRow(1).SetColumn(1));
        grid.Children.Add(headerThreeLabel.SetRow(1).SetColumn(2));
        grid.Children.Add(lisView.SetRow(2).SetColumn(0, 3));
    }

    private TextBlock CreateIndicatorsLabel()
    {
        return TextBlockFactory.CreateLabel(DataGridColumns.INDICATORS.ToString().ScreamingSnakeCaseToTitleCase());
    }

    private TextBlock CreateModsLabel()
    {
        return TextBlockFactory.CreateLabel(DataGridColumns.MODS.ToString().ScreamingSnakeCaseToTitleCase());
    }

    private TextBlock CreateActionsLabel()
    {
        return TextBlockFactory.CreateLabel(DataGridColumns.ACTIONS.ToString().ScreamingSnakeCaseToTitleCase());
    }

    private ListView CreateListView()
    {
        var listView = new ListView()
        {
            ItemTemplate = new DataTemplate(() => BuildListViewTemplateContent(Enum.GetValues<DataGridColumns>()
                .Select(BuildColumnTemplate).ToList())),
        };

        listView.ScrollViewer((builder) => builder.VerticalScrollBarVisibility(ScrollBarVisibility.Hidden));

        var sourceBinding = new Binding()
        {
            Path =
                $"{nameof(ViewModel.StateService)}.{nameof(ViewModel.StateService.CurrentModStatus)}.{nameof(ViewModel.StateService.CurrentModStatus.Mods)}",
        };

        listView.SetBinding(ItemsControl.ItemsSourceProperty, sourceBinding);

        return listView;
    }

    private Border BuildListViewTemplateContent(List<Grid> columnTemplates)
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

        Grid rowGrid = GridFactory.CreateDefaultGrid().DefineColumns(sizes: columnSizes).DefineRows(sizes: [100,]);
        columnTemplates.ForEach((index, panel) => rowGrid.Children.Add(panel.SetColumn(index)));

        border.Child = rowGrid;

        return border;
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

    protected Grid BuildActionsTemplate()
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
