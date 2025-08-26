using ModManager.Abstractions.Services;
using ModManager.Extensions;
using ModManager.Presentation.Converter;
using ModManager.Presentation.Core;
using ModManager.Presentation.Factory;
using ModManager.Presentation.Logic;
using ModManager.Presentation.ViewModel;
using ModManager.Strings;

namespace ModManager.Presentation.UserInterface;

public class PlaysetActionsRegionUserInterface : BaseUserInterface
{
    private readonly PlaysetActionsRegionLogic logic;
    private readonly ITranslationService translationService;
    private readonly PlaysetActionsRegionViewModel viewModel;

    public PlaysetActionsRegionUserInterface(
        PlaysetActionsRegionLogic logic, ITranslationService translationService,
        PlaysetActionsRegionViewModel viewModel)
    {
        this.logic = logic;
        this.translationService = translationService;
        this.viewModel = viewModel;
    }

    /// <inheritdoc />
    protected override void ConfigureContentGrid(Grid grid)
    {
        grid.DefineRows(sizes: [100,]);
        grid.DefineColumns(sizes: [20, 80,]);
    }

    /// <inheritdoc />
    protected override void AddChildrenToGrid(Grid grid)
    {
        Grid activateGroup = CreateActivateGroup();

        grid.Children.Add(activateGroup.SetColumn(0));
    }

    private Grid CreateActivateGroup()
    {
        Grid grid = GridFactory.CreateLeftAlignedGrid();

        grid.DefineRows(sizes: [50, 50,]);

        Button activatePlaysetButton = CreateActivatePlaysetButton();
        Button playsetActiveIndicator = CreatePlaysetActiveIndicatorButton();

        grid.Children.Add(activatePlaysetButton.SetRow(0));
        grid.Children.Add(playsetActiveIndicator.SetRow(1));

        return grid;
    }

    private Button CreatePlaysetActiveIndicatorButton()
    {
        Button button = ButtonFactory.CreateDefaultButton();

        button.Content = translationService[ResourceKeys.Actions.INDICATOR];
        button.IsHitTestVisible = false;

        var backgroundBinding = new Binding()
        {
            Path = $"{nameof(viewModel.StateService)}.{nameof(viewModel.StateService.IsPlaysetActive)}",
            Converter = new BooleanToBrushConverter()
            {
                TrueBrush = new SolidColorBrush(Constants.UiColors.OnButtonColor),
                FalseBrush = new SolidColorBrush(Constants.UiColors.OffButtonColor),
            },
        };

        button.SetBinding(FrameworkElement.BackgroundProperty, backgroundBinding);

        return button;
    }

    private Button CreateActivatePlaysetButton()
    {
        Button button = ButtonFactory.CreateDefaultButton();

        button.Content = translationService[ResourceKeys.Actions.ACTIVATE];

        var backgroundBinding = new Binding()
        {
            Path = $"{nameof(viewModel.StateService)}.{nameof(viewModel.StateService.CanActivatePlayset)}",
            Converter = new BooleanToBrushConverter()
            {
                TrueBrush = new SolidColorBrush(Constants.UiColors.InteractableButtonColor),
                FalseBrush = new SolidColorBrush(Constants.UiColors.DisabledButtonColor),
            },
        };

        var hitTestBinding = new Binding()
        {
            Path = $"{nameof(viewModel.StateService)}.{nameof(viewModel.StateService.CanActivatePlayset)}",
        };

        button.SetBinding(UIElement.IsHitTestVisibleProperty, hitTestBinding);
        button.SetBinding(FrameworkElement.BackgroundProperty, backgroundBinding);

        button.Click += logic.ActivatePlaysetClicked;

        return button;
    }
}
