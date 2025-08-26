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

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(CanActivatePlayset))]
    private bool isPlaysetActive;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(CanActivatePlayset))]
    private bool playsetHasMissingMods;

    [ObservableProperty] private ObservableCollection<IPlayset> playsets;

    /// <inheritdoc />
    public bool CanActivatePlayset => !IsPlaysetActive && !PlaysetHasMissingMods;

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
            logger.LogInformation("Initializing StateService...");

            CurrentModStatus = await InitializeCurrentModStatus();

            await fileService.CreateDefaultPlaysetsIfNotExists(CurrentModStatus);

            Playsets = new ObservableCollection<IPlayset>(await fileService.LoadPlaysets());

            await fileService.UpdatePlaysetsProperties(CurrentModStatus, Playsets);

            EditingPlayset = InitializeEditingPlayset();
            PlaysetHasMissingMods = EditingPlayset.ModStatus.Mods.Any(x => x.IsMissing);

            AttachEventHandlers();

            ReevaluateIsPlaysetActiveState();
            SetStartingLanguage();

            InitializationCompleted?.Invoke(this, true);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to initialize state service.");
            throw;
        }
    }

    private void AttachEventHandlers()
    {
        CurrentModStatusChanged += OnCurrentModStatusChanged;
        EditingPlaysetChanged += OnEditingPlaysetChanged;

        AttachEventHandlersToCurrentMods();
        AttachEventHandlersToEditMods();
    }

    private void AttachEventHandlersToEditMods()
    {
        EditingPlayset?.ModStatus.Mods.ForEach(mod =>
        {
            mod.IsEnabledChanged -= EditModOnIsEnabledChanged;
            mod.IsEnabledChanged += EditModOnIsEnabledChanged;

            mod.IsHiddenChanged -= EditModOnIsHiddenChanged;
            mod.IsHiddenChanged += EditModOnIsHiddenChanged;

            mod.PriorityChanged -= EditModOnPriorityChanged;
            mod.PriorityChanged += EditModOnPriorityChanged;
        });
    }

    private void EditModOnPriorityChanged(object? sender, int e)
    {
        ReevaluateIsPlaysetActiveState();
        SaveEditPlaysetChanges();
    }

    private void EditModOnIsEnabledChanged(object? sender, bool e)
    {
        ReevaluateIsPlaysetActiveState();
        SaveEditPlaysetChanges();
    }

    private void EditModOnIsHiddenChanged(object? sender, bool e)
    {
        ReevaluateIsPlaysetActiveState();
        SaveEditPlaysetChanges();
    }

    private IPlayset InitializeEditingPlayset()
    {
        IPlayset playset = Playsets.First();
        playset.ModStatus.Mods =
            new ObservableCollection<IMod>(playset.ModStatus.Mods.OrderBy(x => x.Priority).ToList());
        return playset;
    }

    private async Task<IModStatus> InitializeCurrentModStatus()
    {
        IModStatus status = await fileService.GetCurrentModStatus();
        status.Mods = new ObservableCollection<IMod>(status.Mods.OrderBy(x => x.Priority).ToList());
        return status;
    }


    private void SaveEditPlaysetChanges()
    {
        fileService.SavePlayset(EditingPlayset ?? throw new InvalidOperationException($"Expected a non-null Playset"));
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
        if (EditingPlayset == null)
        {
            return;
        }

        ReevaluateIsPlaysetActiveState();

        PlaysetHasMissingMods = EditingPlayset.ModStatus.Mods.Any(x => x.IsMissing);

        EditingPlaysetChanged -= OnEditingPlaysetChanged;

        IPlayset? playset = EditingPlayset;
        playset.ModStatus.Mods =
            new ObservableCollection<IMod>(playset.ModStatus.Mods.OrderBy(x => x.Priority).ToList());
        EditingPlayset = playset;

        EditingPlaysetChanged += OnEditingPlaysetChanged;

        AttachEventHandlersToEditMods();
    }

    private void ReevaluateIsPlaysetActiveState()
    {
        if (CurrentModStatus == null)
        {
            return;
        }

        var currentMods = CurrentModStatus.Mods.Where(x => x.IsEnabled).ToList();
        if (EditingPlayset == null)
        {
            return;
        }

        var playsetMods = EditingPlayset.ModStatus.Mods.Where(x => x.IsEnabled).ToList();

        IsPlaysetActive = currentMods.Count == playsetMods.Count &&
                          currentMods.All(currentMod => DoesPlaysetModsHaveMod(currentMod, playsetMods));
    }

    private bool DoesPlaysetModsHaveMod(IMod currentMod, IList<IMod> playsetMods)
    {
        return playsetMods.Any(playsetMod =>
            playsetMod.Title == currentMod.Title && playsetMod.Priority == currentMod.Priority &&
            currentMod.IsLocalMod == playsetMod.IsLocalMod);
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

        AttachEventHandlersToCurrentMods();
    }

    private void AttachEventHandlersToCurrentMods()
    {
        CurrentModStatus?.Mods.ForEach(mod =>
        {
            mod.IsEnabledChanged -= CurrentModOnIsEnabledChanged;
            mod.IsEnabledChanged += CurrentModOnIsEnabledChanged;
        });
    }

    private void CurrentModOnIsEnabledChanged(object? sender, bool e)
    {
        ReevaluateIsPlaysetActiveState();

        fileService.SaveModStatusChanges(CurrentModStatus);
    }
}
