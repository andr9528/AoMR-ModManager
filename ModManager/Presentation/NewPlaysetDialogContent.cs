using ModManager.Abstractions.Services;
using ModManager.Presentation.Logic;
using ModManager.Presentation.UserInterface;
using ModManager.Presentation.ViewModel;

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
