using ModManager.Abstractions.Services;
using ModManager.Presentation.UserInterface;

namespace ModManager.Presentation;

public sealed partial class PlaylistSelector : NavigationView
{
    public PlaylistSelector(IFileService fileService, IStateService stateService)
    {
        IsBackButtonVisible = NavigationViewBackButtonVisible.Collapsed;
        IsPaneToggleButtonVisible = false;
        IsSettingsVisible = false;

        PaneDisplayMode = NavigationViewPaneDisplayMode.LeftCompact;
        CompactPaneLength = 120;

        DataContext = new PlaylistSelectorViewModel(stateService);

        SetBinding(IsPaneOpenProperty,
            new Binding() {Path = nameof(PlaylistSelectorViewModel.IsMenuOpen), Mode = BindingMode.TwoWay,});

        var logic = new PlaylistSelectorLogic(fileService, stateService);
        var ui = new PlaylistSelectorUserInterface(logic, (PlaylistSelectorViewModel) DataContext, fileService,
            stateService);

        PaneCustomContent = ui.CreateNavigationPanel();
        Content = ui.CreateContentGrid();
    }
}
