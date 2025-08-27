using ModManager.Abstractions.Models;

namespace ModManager.Presentation.ViewModel;

public class RenamePlaysetDialogContentViewModel
{
    public IPlayset Playset { get; }
    public string PlaysetName { get; init; }

    public RenamePlaysetDialogContentViewModel(IPlayset playset)
    {
        Playset = playset;
        PlaysetName = playset.FileName;
    }
}
