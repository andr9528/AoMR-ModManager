using ModManager.Abstractions.Models;

namespace ModManager.Abstractions.Services;

public interface IFileService
{
    Task<IModStatus> GetCurrentModStatus();
    Task ApplyModStatus(IModStatus newStatus);
    Task SaveModStatus(string fileName, IModStatus modStatus);
    Task<IModStatus> LoadModStatus(string fileName);
}
