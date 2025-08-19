using ModManager.Abstractions.Services;
using ModManager.Presentation.Logic;
using ModManager.Presentation.UserInterface;
using ModManager.Presentation.ViewModel;

namespace ModManager.Presentation;

public sealed partial class PlaysetSelector : NavigationView
{
    public PlaysetSelector(IFileService fileService, IStateService stateService)
    {
        IsBackButtonVisible = NavigationViewBackButtonVisible.Collapsed;
        IsPaneToggleButtonVisible = false;
        IsSettingsVisible = false;

        PaneDisplayMode = NavigationViewPaneDisplayMode.LeftCompact;
        CompactPaneLength = 160;

        DataContext = new PlaylistSelectorViewModel(stateService);

        SetBinding(IsPaneOpenProperty,
            new Binding() {Path = nameof(PlaylistSelectorViewModel.IsMenuOpen), Mode = BindingMode.TwoWay,});

        var logic = new PlaysetSelectorLogic(fileService, stateService);
        var ui = new PlaysetSelectorUserInterface(logic, (PlaylistSelectorViewModel) DataContext);

        PaneCustomContent = ui.CreateNavigationPanel();
        Content = ui.CreateContentGrid();
    }
}
