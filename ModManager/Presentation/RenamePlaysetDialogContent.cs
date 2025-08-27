using ModManager.Abstractions.Models;
using ModManager.Abstractions.Services;
using ModManager.Presentation.Logic;
using ModManager.Presentation.UserInterface;
using ModManager.Presentation.ViewModel;

namespace ModManager.Presentation;

public class RenamePlaysetDialogContent : Border
{
    public RenamePlaysetDialogContent(ITranslationService translationService, IPlayset playset)
    {
        var fileService = ActivatorUtilities.GetServiceOrCreateInstance<IFileService>(App.Startup.ServiceProvider);

        DataContext = new RenamePlaysetDialogContentViewModel(playset);

        Logic = new RenamePlaysetDialogContentLogic((RenamePlaysetDialogContentViewModel) DataContext, fileService);
        var ui = new RenamePlaysetDialogContentUserInterface(Logic, (RenamePlaysetDialogContentViewModel) DataContext,
            translationService);

        Child = ui.CreateContentGrid();
    }

    public RenamePlaysetDialogContentLogic Logic { get; set; }
}
