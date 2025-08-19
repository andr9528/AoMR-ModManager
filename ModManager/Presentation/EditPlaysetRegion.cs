using Microsoft.UI.Xaml.Controls;
using ModManager.Abstractions.Services;
using ModManager.Presentation.Converter;
using ModManager.Presentation.Factory;
using ModManager.Presentation.Logic;
using ModManager.Presentation.UserInterface;
using ModManager.Presentation.ViewModel;
using Uno.Extensions.Specialized;

namespace ModManager.Presentation;

public class EditPlaysetRegion : Border
{
    public EditPlaysetRegion(IStateService stateService, ITranslationService translationService)
    {
        this.ConfigureDefaultBorder();

        DataContext = new EditPlaysetRegionViewModel(stateService, translationService);

        var logic = new EditPlaysetRegionLogic(stateService, (EditPlaysetRegionViewModel) DataContext);
        var ui = new EditPlaysetRegionUserInterface(logic, translationService,
            (EditPlaysetRegionViewModel) DataContext);

        Child = ui.CreateContentGrid();
    }
}
