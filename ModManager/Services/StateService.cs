using System.Collections.ObjectModel;
using System.Globalization;
using Microsoft.UI.Dispatching;
using ModManager.Abstractions.Models;
using ModManager.Abstractions.Services;

namespace ModManager.Services;

/// <summary>
/// Singleton Observable service that manages the state of the application.
/// </summary>
public partial class StateService : ObservableObject, IStateService
{
    private readonly IFileService fileService;
    private readonly ILogger<StateService> logger;
    private readonly ILocalizationService localizationService;
    [ObservableProperty] private IModStatus? currentModStatus;
    [ObservableProperty] private IPlayset? editingPlayset;
    [ObservableProperty] private bool isPlaysetActive;
    [ObservableProperty] private ObservableCollection<IPlayset> playsets;

    public event EventHandler<IModStatus?> CurrentModStatusChanged;
    public event EventHandler<IPlayset?> EditingPlaysetChanged;
    public event EventHandler<bool> InitializationCompleted;

    public StateService(
        IFileService fileService, ILogger<StateService> logger, ILocalizationService localizationService)
    {
        this.fileService = fileService;
        this.logger = logger;
        this.localizationService = localizationService;

        DispatcherQueue? uiQueue = DispatcherQueue.GetForCurrentThread();
        uiQueue.TryEnqueue(async () => { await InitializeState(); });
    }

    partial void OnCurrentModStatusChanged(IModStatus? value)
    {
        CurrentModStatusChanged?.Invoke(this, value);
    }

    partial void OnEditingPlaysetChanged(IPlayset? value)
    {
        EditingPlaysetChanged?.Invoke(this, value);
    }

    private async Task InitializeState()
    {
        try
        {
            IModStatus status = await fileService.GetCurrentModStatus();
            status.Mods = new ObservableCollection<IMod>(status.Mods.OrderBy(x => x.Priority).ToList());
            CurrentModStatus = status;

            await fileService.CreateDefaultPlaysetsIfNotExists(CurrentModStatus);

            Playsets = new ObservableCollection<IPlayset>(await fileService.LoadPlaysets());

            await fileService.UpdatePlaysetsProperties(CurrentModStatus, Playsets);

            IPlayset playset = Playsets.First();
            playset.ModStatus.Mods =
                new ObservableCollection<IMod>(playset.ModStatus.Mods.OrderBy(x => x.Priority).ToList());
            EditingPlayset = playset;

            CurrentModStatusChanged += OnCurrentModStatusChanged;
            EditingPlaysetChanged += OnEditingPlaysetChanged;
            ReevaluateIsPlaysetActiveState();

            CurrentModStatus?.Mods.ForEach(mod =>
            {
                mod.IsEnabledChanged -= ModOnIsEnabledChanged;
                mod.IsEnabledChanged += ModOnIsEnabledChanged;
            });

            SetStartingLanguage();

            InitializationCompleted?.Invoke(this, true);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to initialize state service.");
            throw;
        }
    }

    private void SetStartingLanguage()
    {
        CultureInfo info = localizationService.CurrentCulture;

        logger.LogInformation($"This PC has the following default Localization: {info.Name}");

        CultureInfo englishInfo = localizationService.SupportedCultures.First();

        localizationService.SetCurrentCultureAsync(englishInfo);
    }

    private void OnEditingPlaysetChanged(object? sender, IPlayset? e)
    {
        ReevaluateIsPlaysetActiveState();

        if (EditingPlayset == null)
        {
            return;
        }

        EditingPlaysetChanged -= OnEditingPlaysetChanged;

        IPlayset? playset = EditingPlayset;
        playset.ModStatus.Mods =
            new ObservableCollection<IMod>(playset.ModStatus.Mods.OrderBy(x => x.Priority).ToList());
        EditingPlayset = playset;

        EditingPlaysetChanged += OnEditingPlaysetChanged;
    }

    private void ReevaluateIsPlaysetActiveState()
    {
        if (currentModStatus == null)
        {
            return;
        }

        var currentMods = currentModStatus.Mods.Where(x => x.IsEnabled).ToList();
        if (editingPlayset == null)
        {
            return;
        }

        var playsetMods = editingPlayset.ModStatus.Mods.Where(x => x.IsEnabled).ToList();

        IsPlaysetActive = currentMods.Count == playsetMods.Count &&
                          currentMods.All(currentMod => DoesPlaysetModsHaveMod(currentMod, playsetMods));
    }

    private bool DoesPlaysetModsHaveMod(IMod currentMod, IList<IMod> playsetMods)
    {
        return playsetMods.Any(playsetMod =>
            playsetMod.Title == currentMod.Title && playsetMod.Priority == currentMod.Priority);
    }

    private void OnCurrentModStatusChanged(object? sender, IModStatus? e)
    {
        ReevaluateIsPlaysetActiveState();

        if (CurrentModStatus == null)
        {
            return;
        }

        CurrentModStatusChanged -= OnCurrentModStatusChanged;

        IModStatus status = CurrentModStatus;
        status.Mods = new ObservableCollection<IMod>(status.Mods.OrderBy(x => x.Priority).ToList());
        CurrentModStatus = status;

        CurrentModStatusChanged += OnCurrentModStatusChanged;

        CurrentModStatus?.Mods.ForEach(mod =>
        {
            mod.IsEnabledChanged -= ModOnIsEnabledChanged;
            mod.IsEnabledChanged += ModOnIsEnabledChanged;
        });
    }

    private void ModOnIsEnabledChanged(object? sender, bool e)
    {
        ReevaluateIsPlaysetActiveState();

        fileService.SaveModStatusChanges(CurrentModStatus);
    }
}
