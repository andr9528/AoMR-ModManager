using ModManager.Abstractions.Models;

namespace ModManager.Presentation;

public class RenamePlaysetDialogContentViewModel
{
    public IPlayset Playset { get; }
    public string RenameText { get; init; }

    public RenamePlaysetDialogContentViewModel(IPlayset playset)
    {
        Playset = playset;
        RenameText = playset.FileName;
    }
}
