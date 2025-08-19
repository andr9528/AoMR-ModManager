using Microsoft.UI.Xaml.Controls;
using ModManager.Abstractions.Services;
using ModManager.Presentation.Converter;
using ModManager.Presentation.Factory;
using ModManager.Presentation.Logic;
using ModManager.Presentation.UserInterface;
using ModManager.Presentation.ViewModel;
using Uno.Extensions.Specialized;

namespace ModManager.Presentation;

public class EditPlaylistRegion : Border
{
    public EditPlaylistRegion(IStateService stateService, ITranslationService translationService)
    {
        this.ConfigureDefaultBorder();

        DataContext = new EditPlaylistRegionViewModel(stateService, translationService);

        var logic = new EditPlaylistRegionLogic(stateService, (EditPlaylistRegionViewModel) DataContext);
        var ui = new EditPlaylistRegionUserInterface(logic, translationService,
            (EditPlaylistRegionViewModel) DataContext);

        Child = ui.CreateContentGrid();
    }
}
