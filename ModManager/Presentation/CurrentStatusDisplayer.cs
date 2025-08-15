using Microsoft.UI.Xaml.Controls;
using ModManager.Abstractions.Services;
using ModManager.Presentation.Factory;
using ModManager.Presentation.Logic;
using ModManager.Presentation.UserInterface;
using ModManager.Presentation.ViewModel;
using ModManager.Strings;

namespace ModManager.Presentation;

public class CurrentStatusDisplayer : Border
{
    public CurrentStatusDisplayer(IStateService stateService, ITranslationService translationService)
    {
        this.ConfigureDefaultBorder();

        DataContext = new CurrentStatusDisplayerViewModel(stateService);

        var logic = new CurrentStatusDisplayerLogic(stateService);
        var ui = new CurrentStatusDisplayerUserInterface(logic, translationService,
            (CurrentStatusDisplayerViewModel) DataContext);

        Child = ui.CreateContentGrid();
    }
}
