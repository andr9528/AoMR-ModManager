using Microsoft.UI.Xaml.Controls;
using ModManager.Abstractions.Services;
using ModManager.Presentation.Factory;
using ModManager.Presentation.Logic;
using ModManager.Presentation.UserInterface;
using ModManager.Presentation.ViewModel;

namespace ModManager.Presentation;

public class CurrentStatusDisplayer : Border
{
    public CurrentStatusDisplayer(IStateService stateService)
    {
        this.ConfigureDefaultBorder();

        DataContext = new CurrentStatusDisplayerViewModel(stateService);

        var logic = new CurrentStatusDisplayerLogic(stateService);
        var ui = new CurrentStatusDisplayerUserInterface(logic, (CurrentStatusDisplayerViewModel) DataContext);

        Child = ui.CreateContentGrid();
    }
}
