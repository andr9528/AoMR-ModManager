using CommunityToolkit.WinUI.UI.Controls;
using ModManager.Abstractions.Models;
using ModManager.Extensions;
using ModManager.Presentation.Converter;
using ModManager.Presentation.Factory;

namespace ModManager.Presentation.Core;

public abstract class BaseModsDisplayerUserInterface<TLogic, TViewModel> : BaseUserInterface
    where TLogic : class, IDisplayerLogic where TViewModel : class, IViewModel
{
    protected readonly TLogic Logic;
    protected readonly TViewModel ViewModel;

    protected BaseModsDisplayerUserInterface(TLogic logic, TViewModel viewModel)
    {
        Logic = logic;
        ViewModel = viewModel;
    }

    protected Grid BuildIndicatorsTemplate()
    {
        Grid grid = GridFactory.CreateLeftAlignedGrid().DefineColumns(sizes: [50, 50, 50,]);

        Button enabledIndicatorButton = CreateEnabledIndicatorButton();
        Button localIndicatorButton = CreateLocalIndicatorButton();
        Button priorityIndicatorButton = CreatePriorityIndicatorButton();

        grid.Children.Add(enabledIndicatorButton.SetColumn(0));
        grid.Children.Add(localIndicatorButton.SetColumn(1));
        grid.Children.Add(priorityIndicatorButton.SetColumn(2));

        return grid;
    }

    private Button CreatePriorityIndicatorButton()
    {
        Button button = ButtonFactory.CreateDefaultButton();
        button.IsHitTestVisible = false;
        button.Background = new SolidColorBrush(Constants.UiColors.InformationButtonColor);

        var contentBinding = new Binding()
        {
            Path = nameof(IMod.Priority),
        };

        button.SetBinding(ContentControl.ContentProperty, contentBinding);

        return button;
    }

    protected Grid BuildModsTemplate()
    {
        Grid grid = GridFactory.CreateLeftAlignedGrid();

        TextBlock modTitle = TextBlockFactory.CreateDefaultTextBlock();

        var titleBinding = new Binding() {Path = nameof(IMod.Title),};

        modTitle.SetBinding(TextBlock.TextProperty, titleBinding);

        grid.Children.Add(modTitle);

        return grid;
    }

    protected Grid BuildAuthorTemplate()
    {
        Grid grid = GridFactory.CreateDefaultGrid();

        TextBlock modAuthor = TextBlockFactory.CreateDefaultTextBlock();

        var authorBinding = new Binding() {Path = nameof(IMod.Author),};

        modAuthor.SetBinding(TextBlock.TextProperty, authorBinding);

        grid.Children.Add(modAuthor);

        return grid;
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
                TrueBrush = new SolidColorBrush(Constants.UiColors.OnButtonColor),
                FalseBrush = new SolidColorBrush(Constants.UiColors.OffButtonColor),
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

        var tagBinding = new Binding();

        button.SetBinding(ContentControl.ContentProperty, contentBinding);
        button.SetBinding(FrameworkElement.BackgroundProperty, backgroundBinding);
        button.SetBinding(FrameworkElement.TagProperty, tagBinding);


        button.Click += Logic.EnabledIndicatorButtonClicked;

        return button;
    }
}
