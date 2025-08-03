namespace ModManager.Abstractions.Models;

public interface IPlayset
{
    IModStatus ModStatus { get; set; }
    string FileName { get; set; }
}
