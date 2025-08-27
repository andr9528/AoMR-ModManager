using ModManager.Abstractions.Models;

namespace ModManager.Presentation;

public class RenameDialogContentViewModel
{
    public IPlayset Playset { get; }
    public string RenameText { get; init; }

    public RenameDialogContentViewModel(IPlayset playset)
    {
        Playset = playset;
        RenameText = playset.FileName;
    }
}
