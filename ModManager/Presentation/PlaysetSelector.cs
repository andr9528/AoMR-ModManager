using ModManager.Abstractions.Services;
using ModManager.Presentation.Logic;
using ModManager.Presentation.UserInterface;
using ModManager.Presentation.ViewModel;

namespace ModManager.Presentation;

public sealed partial class PlaysetSelector : NavigationView
{
    public PlaysetSelector(IFileService fileService, IStateService stateService, ITranslationService translationService)
    {
        IsBackButtonVisible = NavigationViewBackButtonVisible.Collapsed;
        IsPaneToggleButtonVisible = false;
        IsSettingsVisible = false;

        PaneDisplayMode = NavigationViewPaneDisplayMode.LeftCompact;
        CompactPaneLength = 160;

        DataContext = new PlaysetSelectorViewModel(stateService);

        SetBinding(IsPaneOpenProperty,
            new Binding() {Path = nameof(PlaysetSelectorViewModel.IsMenuOpen), Mode = BindingMode.TwoWay,});

        var logic = new PlaysetSelectorLogic(fileService, stateService, translationService, this);
        var ui = new PlaysetSelectorUserInterface(logic, (PlaysetSelectorViewModel) DataContext);

        PaneCustomContent = ui.CreateNavigationPanel();
        Content = ui.CreateContentGrid();
    }
}
