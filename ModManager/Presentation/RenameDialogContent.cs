using ModManager.Abstractions.Models;
using ModManager.Abstractions.Services;

namespace ModManager.Presentation;

public class RenameDialogContent : Border
{
    public RenameDialogContent(ITranslationService translationService, IPlayset playset)
    {
        var fileService = ActivatorUtilities.GetServiceOrCreateInstance<IFileService>(App.Startup.ServiceProvider);

        DataContext = new RenameDialogContentViewModel(playset);

        Logic = new RenameDialogContentLogic((RenameDialogContentViewModel) DataContext, fileService);
        var ui = new RenameDialogContentUserInterface(Logic, (RenameDialogContentViewModel) DataContext,
            translationService);

        Child = ui.CreateContentGrid();
    }

    public RenameDialogContentLogic Logic { get; set; }
}
