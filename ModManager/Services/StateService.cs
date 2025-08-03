using System.Collections.ObjectModel;
using ModManager.Abstractions.Models;
using ModManager.Abstractions.Services;

namespace ModManager.Services;

/// <summary>
/// Singleton Observable service that manages the state of the application.
/// </summary>
public partial class StateService : ObservableObject, IStateService
{
    private readonly IFileService fileService;
    [ObservableProperty] private IModStatus currentModStatus;
    [ObservableProperty] private IPlayset editingPlayset;
    [ObservableProperty] private bool isPlaysetActive;
    [ObservableProperty] private ObservableCollection<IPlayset> playsets;

    public event EventHandler<IModStatus> CurrentModStatusChanged;
    public event EventHandler<IPlayset> EditingPlaysetChanged;

    public StateService(IFileService fileService)
    {
        this.fileService = fileService;

        _ = InitializeState();
    }

    partial void OnCurrentModStatusChanged(IModStatus value)
    {
        CurrentModStatusChanged?.Invoke(this, value);
    }

    partial void OnEditingPlaysetChanged(IPlayset value)
    {
        EditingPlaysetChanged?.Invoke(this, value);
    }

    private async Task InitializeState()
    {
        CurrentModStatus = await fileService.GetCurrentModStatus();

        await fileService.CreateDefaultPlaysetsIfNotExists(CurrentModStatus);

        Playsets = new ObservableCollection<IPlayset>(await fileService.LoadPlaysets());

        await fileService.UpdatePlaysetsProperties(CurrentModStatus, Playsets);

        EditingPlayset = Playsets.First();

        CurrentModStatusChanged += OnCurrentModStatusChanged;
        EditingPlaysetChanged += OnEditingPlaysetChanged;
        ReevaluateIsPlaysetActiveState();
    }

    private void OnEditingPlaysetChanged(object? sender, IPlayset e)
    {
        ReevaluateIsPlaysetActiveState();
    }

    private void ReevaluateIsPlaysetActiveState()
    {
        var currentMods = currentModStatus.Mods.Where(x => x.Enabled).ToList();
        var playsetMods = editingPlayset.ModStatus.Mods.Where(x => x.Enabled).ToList();

        IsPlaysetActive = currentMods.Count == playsetMods.Count &&
                          currentMods.All(currentMod => DoesPlaysetModsHaveMod(currentMod, playsetMods));
    }

    private bool DoesPlaysetModsHaveMod(IMod currentMod, IList<IMod> playsetMods)
    {
        return playsetMods.Any(playsetMod =>
            playsetMod.Title == currentMod.Title && playsetMod.Priority == currentMod.Priority);
    }

    private void OnCurrentModStatusChanged(object? sender, IModStatus e)
    {
        ReevaluateIsPlaysetActiveState();
    }
}
