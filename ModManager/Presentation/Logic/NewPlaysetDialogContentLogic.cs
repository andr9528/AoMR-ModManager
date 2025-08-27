using Windows.ApplicationModel.DataTransfer;
using Microsoft.UI.Dispatching;
using ModManager.Abstractions.Models;
using ModManager.Abstractions.Services;
using ModManager.Extensions;
using ModManager.Presentation.Core;
using ModManager.Presentation.ViewModel;
using Newtonsoft.Json;

namespace ModManager.Presentation.Logic;

public class NewPlaysetDialogContentLogic : BaseLogic
{
    private enum Options
    {
        FROM_CURRENT_STATUS = 0,
        FROM_EDITING_PLAYSET = 1,
        FROM_CLIPBOARD = 2,
    }

    private readonly NewPlaysetDialogContentViewModel viewModel;
    private readonly IFileService fileService;
    private readonly ILogger<NewPlaysetDialogContentLogic> logger;
    private readonly DispatcherQueue uiQueue;

    public NewPlaysetDialogContentLogic(
        NewPlaysetDialogContentViewModel viewModel, IStateService stateService,
        IFileService fileService) : base(stateService)
    {
        this.viewModel = viewModel;
        this.fileService = fileService;
        logger =
            ActivatorUtilities.GetServiceOrCreateInstance<ILogger<NewPlaysetDialogContentLogic>>(App.Startup
                .ServiceProvider);
        uiQueue = DispatcherQueue.GetForCurrentThread();
    }

    public void NewPlaysetDialogOnPrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        int index = viewModel.SelectedModsSource;

        var values = (Options[]) Enum.GetValues(typeof(Options));

        if (index < 0 || index >= values.Length)
        {
            args.Cancel = true;
            return;
        }

        Options selectedOption = values[index];

        if (!Enum.IsDefined(typeof(Options), selectedOption))
        {
            args.Cancel = true;
            return;
        }

        _ = CreatePlaysetFromOption(selectedOption);
    }

    private async Task CreatePlaysetFromOption(Options selectedOption)
    {
        Playset? newPlayset = selectedOption switch
        {
            Options.FROM_CURRENT_STATUS => CreatePlaysetFromCurrentStatus(),
            Options.FROM_EDITING_PLAYSET => CreatePlaysetFromEditingPlayset(),
            Options.FROM_CLIPBOARD => await CreatePlaysetFromClipboard(),
            var _ => null,
        };

        if (newPlayset is null)
        {
            logger.LogWarning("Failed to create new Playset from selected option.");
            return;
        }

        string trimmedName = viewModel.PlaysetName.Trim();
        newPlayset.FileName = trimmedName;

        await fileService.SavePlayset(newPlayset);

        uiQueue.TryEnqueue(() => StateService.Playsets.Add(newPlayset));

        if (selectedOption == Options.FROM_CLIPBOARD && StateService.CurrentModStatus != null)
        {
            await fileService.UpdatePlaysetProperties(StateService.CurrentModStatus, newPlayset);
        }
    }

    private Playset? CreatePlaysetFromEditingPlayset()
    {
        IModStatus? clonedModStatus = StateService.EditingPlayset?.ModStatus.DeepClone();
        if (clonedModStatus is null)
        {
            logger.LogWarning("Failed to clone ModStatus from Editing playset when creating new Playset from it.");
            return null;
        }

        var playset = new Playset() {ModStatus = clonedModStatus,};
        return playset;
    }

    private Playset? CreatePlaysetFromCurrentStatus()
    {
        IModStatus? clonedModStatus = StateService.CurrentModStatus.DeepClone();
        if (clonedModStatus is null)
        {
            logger.LogWarning("Failed to clone current ModStatus when creating new Playset from it.");
            return null;
        }

        var playset = new Playset() {ModStatus = clonedModStatus,};
        return playset;
    }

    private async Task<Playset?> CreatePlaysetFromClipboard()
    {
        Playset? playset = await GetPlaysetFromClipboard();
        bool isValid = await ValidateClipboardResult(playset);

        if (isValid && playset is not null)
        {
            return playset;
        }

        logger.LogWarning(
            "Somehow attempted to create Playset from Clipboard while it did not contain a valid playset JSON.");
        return null;
    }

    public async Task ModsSourceSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is not ComboBox box)
        {
            return;
        }

        if (box.SelectedIndex == 2)
        {
            bool isValid = await ValidateClipboardResult();
            if (!isValid)
            {
                logger.LogWarning(
                    "Clipboard does not contain a valid playset JSON. Resetting selected index back to one.");
                box.SelectedIndex = 0;
            }
        }
    }

    private async Task<bool> ValidateClipboardResult(Playset? playset = null)
    {
        playset ??= await GetPlaysetFromClipboard();

        return playset is not null;
    }

    private async Task<Playset?> GetPlaysetFromClipboard()
    {
        DataPackageView? clipboardContent = Clipboard.GetContent();
        string clipboardText = await clipboardContent.GetTextAsync();
        if (string.IsNullOrWhiteSpace(clipboardText))
        {
            return null;
        }

        try
        {
            return JsonConvert.DeserializeObject<Playset>(clipboardText);
        }
        catch (JsonException)
        {
            return null;
        }
    }
}
