using Microsoft.UI.Xaml.Controls;
using ModManager.Abstractions.Services;
using ModManager.Presentation.Converter;
using ModManager.Presentation.Factory;
using ModManager.Presentation.Logic;
using ModManager.Presentation.UserInterface;
using ModManager.Presentation.ViewModel;
using Uno.Extensions.Specialized;

namespace ModManager.Presentation;

public class EditPlaylistDisplayer : Border
{
    public EditPlaylistDisplayer(IStateService stateService, ITranslationService translationService)
    {
        this.ConfigureDefaultBorder();

        DataContext = new EditPlaylistDisplayerViewModel(stateService, translationService);

        var logic = new EditPlaylistDisplayerLogic(stateService, (EditPlaylistDisplayerViewModel) DataContext);
        var ui = new EditPlaylistDisplayerUserInterface(logic, translationService,
            (EditPlaylistDisplayerViewModel) DataContext);

        Child = ui.CreateContentGrid();
    }
}
