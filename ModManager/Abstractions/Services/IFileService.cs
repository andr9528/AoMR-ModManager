using ModManager.Abstractions.Models;

namespace ModManager.Abstractions.Services;

public interface IFileService
{
    Task<IModStatus> GetCurrentModStatus();
    Task ActivatePlayset(IPlayset playset);
    Task SavePlayset(IPlayset playset);
    Task<IModStatus> LoadModStatus(string fileName);
    Task<IList<IPlayset>> LoadPlaysets();
    Task CreateDefaultPlaysetsIfNotExists(IModStatus currentModStatus);
    Task UpdatePlaysetsProperties(IModStatus currentModStatus, IList<IPlayset> playsets);
    Task NewPlayset(IPlayset playset, bool makeEmptyPlayset = true);
    Task SaveModStatusChanges(IModStatus? modStatus);
}
