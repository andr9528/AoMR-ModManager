using Microsoft.UI.Xaml.Controls;
using ModManager.Abstractions.Services;
using ModManager.Presentation.Factory;
using ModManager.Presentation.Logic;
using ModManager.Presentation.UserInterface;
using ModManager.Presentation.ViewModel;
using ModManager.Strings;

namespace ModManager.Presentation;

public class CurrentStatusRegion : Border
{
    public CurrentStatusRegion(IStateService stateService, ITranslationService translationService)
    {
        this.ConfigureDefaultBorder();

        DataContext = new CurrentStatusRegionViewModel(stateService);

        var logic = new CurrentStatusRegionLogic(stateService);
        var ui = new CurrentStatusRegionUserInterface(logic, translationService,
            (CurrentStatusRegionViewModel) DataContext);

        Child = ui.CreateContentGrid();
    }
}
