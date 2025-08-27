using ModManager.Abstractions.Models;
using ModManager.Abstractions.Services;
using ModManager.Presentation.ViewModel;

namespace ModManager.Presentation.Logic;

public class RenamePlaysetDialogContentLogic
{
    private readonly RenamePlaysetDialogContentViewModel viewModel;
    private readonly IFileService fileService;
    private readonly ILogger<RenamePlaysetDialogContentLogic> logger;

    public RenamePlaysetDialogContentLogic(RenamePlaysetDialogContentViewModel viewModel, IFileService fileService)
    {
        this.viewModel = viewModel;
        this.fileService = fileService;
        logger =
            ActivatorUtilities.GetServiceOrCreateInstance<ILogger<RenamePlaysetDialogContentLogic>>(App.Startup
                .ServiceProvider);
    }

    public void RenamePlaysetDialogOnPrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        if (sender?.Tag is not IPlayset taggedPlayset)
        {
            logger.LogWarning(
                $"Expected Content Dialog in '{nameof(RenamePlaysetDialogOnPrimaryButtonClick)}' to be tagged with a '{nameof(IPlayset)}'.");

            return;
        }

        string oldName = taggedPlayset.FileName;
        string newName = viewModel.PlaysetName.Trim();

        bool renamed = fileService.RenamePlayset(oldName, newName);

        if (!renamed)
        {
            return;
        }

        logger.LogInformation("Renamed playset '{OldName}' to '{NewName}'.", taggedPlayset.FileName, newName);
        taggedPlayset.FileName = newName;
    }
}
