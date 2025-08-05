using ModManager.Abstractions.Services;

namespace ModManager.Presentation;

public partial class PlaylistSelectorViewModel : ObservableObject
{
    [ObservableProperty] private bool isMenuOpen;

    public IStateService StateService { get; }

    public event EventHandler<bool> IsMenuOpenChanged;

    partial void OnIsMenuOpenChanged(bool value)
    {
        IsMenuOpenChanged?.Invoke(this, value);
    }

    public PlaylistSelectorViewModel(IStateService stateService)
    {
        StateService = stateService;
    }
}
