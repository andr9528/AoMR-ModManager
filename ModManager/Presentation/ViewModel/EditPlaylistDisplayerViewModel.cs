using System.Collections.ObjectModel;
using ModManager.Abstractions.Models;
using ModManager.Abstractions.Services;
using ModManager.Strings;

namespace ModManager.Presentation;

public partial class EditPlaylistDisplayerViewModel : ObservableObject, IViewModel
{
    private readonly ITranslationService translationService;

    [ObservableProperty] private string headerText;
    [ObservableProperty] private ObservableCollection<IMod> shownMods;

    public EditPlaylistDisplayerViewModel(IStateService stateService, ITranslationService translationService)
    {
        this.translationService = translationService;
        StateService = stateService;

        UpdateHeaderText();
        UpdateShownMods();

        StateService.EditingPlaysetChanged += StateService_EditingPlaysetChanged;
    }

    private void UpdateHeaderText()
    {
        HeaderText = translationService[ResourceKeys.Edit.HEADER] + StateService.EditingPlayset?.FileName;
    }

    private void StateService_EditingPlaysetChanged(object? sender, IPlayset? e)
    {
        UpdateHeaderText();
        UpdateShownMods();

        StateService.EditingPlayset?.ModStatus.Mods.ForEach(mod =>
        {
            mod.IsHiddenChanged -= Mod_HiddenChanged;
            mod.IsHiddenChanged += Mod_HiddenChanged;
        });
    }

    private void Mod_HiddenChanged(object? sender, bool e)
    {
        UpdateShownMods();
    }

    private void UpdateShownMods()
    {
        ShownMods = new ObservableCollection<IMod>(StateService.EditingPlayset?.ModStatus.Mods.Where(x => !x.IsHidden)
            .ToList() ?? []);
    }

    /// <inheritdoc />
    public IStateService StateService { get; }
}
