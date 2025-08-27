using ModManager.Abstractions.Models;
using ModManager.Abstractions.Services;

namespace ModManager.Presentation;

public class RenameDialogContentLogic
{
    private readonly RenameDialogContentViewModel viewModel;
    private readonly IFileService fileService;
    private readonly ILogger<RenameDialogContentLogic> logger;

    public RenameDialogContentLogic(RenameDialogContentViewModel viewModel, IFileService fileService)
    {
        this.viewModel = viewModel;
        this.fileService = fileService;
        logger =
            ActivatorUtilities.GetServiceOrCreateInstance<ILogger<RenameDialogContentLogic>>(
                App.Startup.ServiceProvider);
    }

    public void RenameDialogOnPrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        if (sender?.Tag is not IPlayset taggedPlayset)
        {
            logger.LogWarning(
                $"Expected Content Dialog in '{nameof(RenameDialogOnPrimaryButtonClick)}' to be tagged with a '{nameof(IPlayset)}'.");

            return;
        }

        string oldName = taggedPlayset.FileName;
        string newName = viewModel.RenameText.Trim();

        bool renamed = fileService.RenamePlayset(oldName, newName);

        if (!renamed)
        {
            return;
        }

        logger.LogInformation("Renamed playset '{OldName}' to '{NewName}'.", taggedPlayset.FileName, newName);
        taggedPlayset.FileName = newName;
    }
}
