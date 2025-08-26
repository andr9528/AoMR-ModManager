using ModManager.Abstractions.Models;
using ModManager.Abstractions.Services;

namespace ModManager.Presentation.ViewModel;

public partial class PlaysetSelectorViewModel : ObservableObject, IViewModel
{
    [ObservableProperty] private bool isMenuOpen;

    public IStateService StateService { get; }

    public event EventHandler<bool> IsMenuOpenChanged;

    partial void OnIsMenuOpenChanged(bool value)
    {
        IsMenuOpenChanged?.Invoke(this, value);
    }

    public PlaysetSelectorViewModel(IStateService stateService)
    {
        StateService = stateService;
    }
}
