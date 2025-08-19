using ModManager.Abstractions.Services;
using ModManager.Presentation.Core;

namespace ModManager.Presentation.Logic;

public class PlaysetActionsRegionLogic : BaseLogic
{
    private readonly IFileService fileService;

    public PlaysetActionsRegionLogic(IStateService stateService, IFileService fileService) : base(stateService)
    {
        this.fileService = fileService;
    }

    public void ActivatePlaysetClicked(object sender, RoutedEventArgs e)
    {
        if (StateService.EditingPlayset == null)
        {
            return;
        }

        fileService.ActivatePlayset(StateService.EditingPlayset);
        StateService.CurrentModStatus = FastCloner.FastCloner.DeepClone(StateService.EditingPlayset.ModStatus);
    }
}
