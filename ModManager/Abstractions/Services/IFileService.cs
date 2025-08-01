using ModManager.Abstractions.Models;

namespace ModManager.Abstractions.Services;

public interface IFileService
{
    IModStatus GetCurrentModStatus();
    void ApplyModStatus(IModStatus newStatus);
    void SaveModStatus(string fileName, IModStatus modStatus);
    IModStatus LoadModStatus(string fileName);
}
