using ModManager.Abstractions.Models;
using ModManager.Abstractions.Services;
using ModManager.Extensions;
using ModManager.Presentation.Core;
using ModManager.Presentation.Factory;
using ModManager.Strings;
using Windows.ApplicationModel.DataTransfer;
using Microsoft.UI.Dispatching;
using Newtonsoft.Json;

namespace ModManager.Presentation;

public class NewPlaysetDialogContent : Border
{
    public NewPlaysetDialogContent(
        IStateService stateService, IFileService fileService, ITranslationService translationService)
    {
        DataContext = new NewPlaysetDialogContentViewModel();

        Logic = new NewPlaysetDialogContentLogic((NewPlaysetDialogContentViewModel) DataContext, stateService,
            fileService);
        var ui = new NewPlaysetDialogContentUserInterface(Logic, (NewPlaysetDialogContentViewModel) DataContext,
            translationService);

        Child = ui.CreateContentGrid();
    }

    public NewPlaysetDialogContentLogic Logic { get; set; }
}

public class NewPlaysetDialogContentUserInterface : BaseUserInterface
{
    private readonly NewPlaysetDialogContentLogic logic;
    private readonly NewPlaysetDialogContentViewModel viewModel;
    private readonly ITranslationService translationService;

    public NewPlaysetDialogContentUserInterface(
        NewPlaysetDialogContentLogic logic, NewPlaysetDialogContentViewModel viewModel,
        ITranslationService translationService)
    {
        this.logic = logic;
        this.viewModel = viewModel;
        this.translationService = translationService;
    }

    /// <inheritdoc />
    protected override void ConfigureContentGrid(Grid grid)
    {
        grid.DefineRows(sizes: [50, 50, 50, 50,]);
    }

    /// <inheritdoc />
    protected override void AddChildrenToGrid(Grid grid)
    {
        TextBlock messageBlockOne = CreateMessageBlockOne();
        TextBox nameTextBox = CreateNameTextBox();
        TextBlock messageBlockTwo = CreateMessageBlockTwo();
        ComboBox modsSourceComboBox = CreateModsSourceComboBox();

        grid.Children.Add(messageBlockOne.SetRow(0));
        grid.Children.Add(nameTextBox.SetRow(1));
        grid.Children.Add(messageBlockTwo.SetRow(2));
        grid.Children.Add(modsSourceComboBox.SetRow(3));
    }

    private ComboBox CreateModsSourceComboBox()
    {
        string[] options =
        [
            translationService[ResourceKeys.Dialog.Create.OPTION_ONE],
            translationService[ResourceKeys.Dialog.Create.OPTION_TWO],
            translationService[ResourceKeys.Dialog.Create.OPTION_THREE],
        ];

        ComboBox comboBox = ComboBoxFactory.CreateDefaultComboBox(options);

        var selectedIndexBinding = new Binding()
        {
            Path = nameof(viewModel.SelectedModsSource),
            Mode = BindingMode.TwoWay,
        };

        comboBox.SetBinding(ComboBox.SelectedIndexProperty, selectedIndexBinding);

        comboBox.SelectionChanged += (sender, args) => logic.ModsSourceSelectionChanged(sender, args);

        return comboBox;
    }

    private TextBlock CreateMessageBlockTwo()
    {
        TextBlock block = TextBlockFactory.CreateDefaultTextBlock();

        block.Text = translationService[ResourceKeys.Dialog.Create.MESSAGE_TWO];
        block.Foreground = new SolidColorBrush(Constants.UiColors.DialogLabelColor);

        return block;
    }

    private TextBox CreateNameTextBox()
    {
        TextBox box = TextBoxFactory.CreateDefaultTextBox();

        var textBinding = new Binding()
        {
            Path = nameof(viewModel.PlaysetName),
            Mode = BindingMode.TwoWay,
        };

        box.SetBinding(TextBox.TextProperty, textBinding);

        return box;
    }

    private TextBlock CreateMessageBlockOne()
    {
        TextBlock block = TextBlockFactory.CreateDefaultTextBlock();

        block.Text = translationService[ResourceKeys.Dialog.Create.MESSAGE_ONE];
        block.Foreground = new SolidColorBrush(Constants.UiColors.DialogLabelColor);

        return block;
    }
}

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

public class NewPlaysetDialogContentViewModel
{
    public string PlaysetName { get; init; } = string.Empty;
    public int SelectedModsSource { get; init; } = 0;
}
