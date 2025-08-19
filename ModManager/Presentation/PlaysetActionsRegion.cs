using ModManager.Abstractions.Services;
using ModManager.Presentation.Factory;
using ModManager.Presentation.Logic;
using ModManager.Presentation.UserInterface;
using ModManager.Presentation.ViewModel;

namespace ModManager.Presentation;

public class PlaysetActionsRegion : Border
{
    public PlaysetActionsRegion(
        IStateService stateService, ITranslationService translationService, IFileService fileService)
    {
        this.ConfigureDefaultBorder();

        DataContext = new PlaysetActionsRegionViewModel(stateService);

        var logic = new PlaysetActionsRegionLogic(stateService, fileService);
        var ui = new PlaysetActionsRegionUserInterface(logic, translationService,
            (PlaysetActionsRegionViewModel) DataContext);

        Child = ui.CreateContentGrid();
    }
}
