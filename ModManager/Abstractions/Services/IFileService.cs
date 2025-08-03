using ModManager.Abstractions.Models;

namespace ModManager.Abstractions.Services;

public interface IFileService
{
    Task<IModStatus> GetCurrentModStatus();
    Task ApplyModStatus(IModStatus newStatus);
    Task SavePlaysetModStatus(string fileName, IModStatus modStatus);
    Task<IModStatus> LoadModStatus(string fileName);
    Task<IList<IPlayset>> LoadPlaysets();
    Task CreateDefaultPlaysetsIfNotExists(IModStatus currentModStatus);
    Task UpdatePlaysetsProperties(IModStatus currentModStatus, IList<IPlayset> playsets);
    Task NewPlayset(string fileName, IModStatus currentModStatus, bool makeEmptyPlayset = true);
}
