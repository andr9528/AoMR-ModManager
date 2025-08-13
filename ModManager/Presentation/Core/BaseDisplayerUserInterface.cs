using CommunityToolkit.WinUI.UI.Controls;
using ModManager.Abstractions.Models;
using ModManager.Extensions;
using ModManager.Presentation.Converter;
using ModManager.Presentation.Factory;

namespace ModManager.Presentation.Core;

public abstract class BaseDisplayerUserInterface<TLogic, TViewModel> where TLogic : class, IDisplayerLogic
    where TViewModel : class, IViewModel
{
    protected readonly TLogic Logic;
    protected readonly TViewModel ViewModel;

    protected BaseDisplayerUserInterface(TLogic logic, TViewModel viewModel)
    {
        Logic = logic;
        ViewModel = viewModel;
    }

    public Grid CreateContentGrid()
    {
        Grid grid = GridFactory.CreateDefaultGrid();

        ConfigureContentGrid(grid);
        AddChildrenToGrid(grid);

        return grid;
    }

    protected abstract void ConfigureContentGrid(Grid grid);
    protected abstract void AddChildrenToGrid(Grid grid);

    protected DataGridTemplateColumn BuildIndicatorsColumn(string header)
    {
        var template = new DataTemplate(BuildIndicatorsTemplate);

        return new DataGridTemplateColumn()
        {
            Header = header.ScreamingSnakeCaseToTitleCase(),
            CellTemplate = template,
            Width = new DataGridLength(30, DataGridLengthUnitType.Star),
        };
    }

    private StackPanel? BuildIndicatorsTemplate()
    {
        var panel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
        };

        Button enabledIndicatorButton = CreateEnabledIndicatorButton();
        Button localIndicatorButton = CreateLocalIndicatorButton();

        panel.Children.Add(enabledIndicatorButton);
        panel.Children.Add(localIndicatorButton);

        return panel;
    }

    private Button CreateLocalIndicatorButton()
    {
        Button button = ButtonFactory.CreateDefaultButton();
        button.IsHitTestVisible = false;

        var contentBinding = new Binding()
        {
            Path = nameof(IMod.IsLocalMod),
            Converter = new BooleanToFontIconConverter()
            {
                TrueGlyph = Constants.Glyphs.FOLDER_SYMBOL_UNICODE,
                FalseGlyph = Constants.Glyphs.CLOUD_SYMBOL_UNICODE,
            },
        };

        var backgroundBinding = new Binding()
        {
            Path = nameof(IMod.IsLocalMod),
            Converter = new BooleanToBrushConverter()
            {
                TrueBrush = new SolidColorBrush(Colors.LimeGreen.WithAlpha(0.4)),
                FalseBrush = new SolidColorBrush(Colors.IndianRed.WithAlpha(0.4)),
            },
        };

        button.SetBinding(ContentControl.ContentProperty, contentBinding);
        button.SetBinding(FrameworkElement.BackgroundProperty, backgroundBinding);

        return button;
    }

    private Button CreateEnabledIndicatorButton()
    {
        Button button = ButtonFactory.CreateDefaultButton();

        var contentBinding = new Binding()
        {
            Path = nameof(IMod.IsEnabled),
            Converter = new BooleanToFontIconConverter()
            {
                TrueGlyph = Constants.Glyphs.CHECKMARK_SYMBOL_UNICODE,
                FalseGlyph = Constants.Glyphs.CROSS_SYMBOL_UNICODE,
            },
        };

        var backgroundBinding = new Binding()
        {
            Path = nameof(IMod.IsEnabled),
            Converter = new BooleanToBrushConverter()
            {
                TrueBrush = new SolidColorBrush(Constants.UiColors.OnButtonColor),
                FalseBrush = new SolidColorBrush(Constants.UiColors.OffButtonColor),
            },
        };

        var tagBinding = new Binding
        {
            Path = nameof(IMod.WorkshopId),
        };

        button.SetBinding(ContentControl.ContentProperty, contentBinding);
        button.SetBinding(FrameworkElement.BackgroundProperty, backgroundBinding);
        button.SetBinding(FrameworkElement.TagProperty, tagBinding);

        button.Click += Logic.EnabledIndicatorButtonClicked;

        return button;
    }

    protected DataGridTextColumn BuildModsColumn(string header)
    {
        return new DataGridTextColumn()
        {
            Header = header.ScreamingSnakeCaseToTitleCase(),
            Binding = new Binding {Path = nameof(IMod.Title),},
            Width = new DataGridLength(70, DataGridLengthUnitType.Star),
            FontSize = 12,
            Foreground = new SolidColorBrush(Colors.Black),
        };
    }

    protected DataGridTemplateColumn BuildActionsColumn(string header)
    {
        var template = new DataTemplate(BuildActionsTemplate);

        return new DataGridTemplateColumn()
        {
            Header = header.ScreamingSnakeCaseToTitleCase(),
            CellTemplate = template,
            Width = new DataGridLength(30, DataGridLengthUnitType.Star),
        };
    }

    protected abstract StackPanel BuildActionsTemplate();
}
