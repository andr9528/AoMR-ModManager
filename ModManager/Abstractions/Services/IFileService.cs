using ModManager.Abstractions.Models;

namespace ModManager.Abstractions.Services;

public interface IFileService
{
    Task<IModStatus> GetCurrentModStatus();
    Task ActivatePlayset(IPlayset playset);
    Task SavePlayset(IPlayset playset);
    Task<IList<IPlayset>> LoadPlaysets();
    Task CreateDefaultPlaysetsIfNotExists(IModStatus currentModStatus);
    Task UpdatePlaysetsProperties(IModStatus currentModStatus, IList<IPlayset> playsets);
    Task UpdatePlaysetProperties(IModStatus currentModStatus, IPlayset playset);
    Task SaveModStatusChanges(IModStatus? modStatus);
    bool DeletePlayset(IPlayset playset);
    bool RenamePlayset(string oldName, string newName);
}
