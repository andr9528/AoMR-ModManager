using System.Reflection;
using ModManager.Abstractions.Models;
using ModManager.Abstractions.Services;
using Newtonsoft.Json;
using Path = System.IO.Path;

namespace ModManager.Services;

public class FileService : IFileService
{
    private const string MOD_STATUS_FILE_NAME = "myth-mod-status";
    private const string MOD_MANAGER_FOLDER = "ModManager";
    private const string PLAYSET_FOLDER = "Playsets";
    private const string DEFAULT_OFF_PLAYSET_FILE_NAME = "all-off";
    private const string DEFAULT_ON_PLAYSET_FILE_NAME = "all-on";
    private static string PlaysetsLocation;

    public FileService()
    {
        PlaysetsLocation = Path.Combine(GetExecutionDirectory(), MOD_MANAGER_FOLDER, PLAYSET_FOLDER);
        if (!Directory.Exists(PlaysetsLocation))
        {
            Directory.CreateDirectory(PlaysetsLocation);
        }
    }

    /// <inheritdoc />
    public async Task<IModStatus> GetCurrentModStatus()
    {
        string jsonPath = GetModStatusFilePath();

        string content = await File.ReadAllTextAsync(jsonPath);

        var modStatus = JsonConvert.DeserializeObject<ModStatus>(content);

        if (modStatus == null)
        {
            throw new InvalidOperationException("Failed to deserialize mod status.");
        }

        return modStatus;
    }

    private string GetExecutionDirectory()
    {
        string exePath = Assembly.GetExecutingAssembly().Location;
        return Path.GetDirectoryName(exePath) ?? string.Empty;
    }

    private string GetModStatusFilePath()
    {
        string exeDir = GetExecutionDirectory();
        return Path.Combine(exeDir, EnsureExtension(MOD_STATUS_FILE_NAME, ".json"));
    }

    /// <inheritdoc />
    public async Task ApplyModStatus(IModStatus newStatus)
    {
        string jsonPath = GetModStatusFilePath();

        string content = JsonConvert.SerializeObject(newStatus, Formatting.Indented);

        await File.WriteAllTextAsync(jsonPath, content);
    }

    /// <inheritdoc />
    public async Task SavePlaysetModStatus(string fileName, IModStatus modStatus)
    {
        string fileNameWithExtension = EnsureExtension(fileName, ".json");
        string savePath = Path.Combine(PlaysetsLocation, fileNameWithExtension);

        string content = JsonConvert.SerializeObject(modStatus, Formatting.Indented);

        await File.WriteAllTextAsync(savePath, content);
    }

    private string EnsureExtension(string path, string targetExt)
    {
        if (path == null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        if (string.IsNullOrEmpty(targetExt))
        {
            throw new ArgumentException("Target extension must be non-empty", nameof(targetExt));
        }

        // Ensure extension starts with dot
        if (!targetExt.StartsWith("."))
        {
            targetExt = "." + targetExt;
        }

        if (!Path.HasExtension(path))
        {
            return path + targetExt;
        }

        string existing = Path.GetExtension(path)!; // includes dot
        return string.Equals(existing, targetExt, StringComparison.OrdinalIgnoreCase)
            ? path
            : Path.ChangeExtension(path, targetExt);
    }

    /// <inheritdoc />
    public async Task<IModStatus> LoadModStatus(string fileName)
    {
        string fileNameWithExtension = EnsureExtension(fileName, ".json");
        string loadPath = Path.Combine(PlaysetsLocation, fileNameWithExtension);

        if (!File.Exists(loadPath))
        {
            throw new FileNotFoundException(
                $"Mod status file '{fileNameWithExtension}' not found in '{PlaysetsLocation}'.");
        }

        string content = await File.ReadAllTextAsync(loadPath);

        var modStatus = JsonConvert.DeserializeObject<ModStatus>(content);

        if (modStatus == null)
        {
            throw new InvalidOperationException("Failed to deserialize mod status.");
        }

        return modStatus;
    }

    /// <inheritdoc />
    public async Task<IList<IPlayset>> LoadPlaysets()
    {
        string[] files = Directory.GetFiles(PlaysetsLocation, "*.json");
        var modStatuses = await Task.WhenAll(files.Select(LoadModStatus));

        var playsets = files.Select((file, index) => new Playset
        {
            FileName = Path.GetFileName(file),
            ModStatus = modStatuses[index],
        }).ToList<IPlayset>();

        return playsets;
    }

    /// <inheritdoc />
    public async Task CreateDefaultPlaysetsIfNotExists(IModStatus currentModStatus)
    {
        await CreateAllOffPlayset(currentModStatus);

        await CreateAllOnPlayset(currentModStatus);
    }

    /// <inheritdoc />
    public async Task UpdatePlaysetsProperties(IModStatus currentModStatus, IList<IPlayset> playsets)
    {
        foreach (IMod mod in currentModStatus.Mods)
        {
            foreach (IPlayset playset in playsets)
            {
                IMod? playsetMod = playset.ModStatus.Mods.FirstOrDefault(m => m.Title == mod.Title);

                if (playsetMod == null)
                {
                    IMod clonedMod = FastCloner.FastCloner.DeepClone(mod) ??
                                     throw new InvalidOperationException($"Failed to Clone the mod: {mod.Title}");
                    clonedMod.Hidden = true;
                    playset.ModStatus.Mods.Add(clonedMod);
                    continue;
                }

                playsetMod.Description = mod.Description;
                playsetMod.InstallCrc = mod.InstallCrc;
                playsetMod.InstallTime = mod.InstallTime;
                playsetMod.LastUpdate = mod.LastUpdate;
            }
        }

        await Task.WhenAll(playsets.Select(x => SavePlaysetModStatus(x.FileName, x.ModStatus)));
    }

    /// <inheritdoc />
    public async Task NewPlayset(string fileName, IModStatus currentModStatus, bool makeEmptyPlayset = true)
    {
        if (DoesPlaysetExist(fileName))
        {
            return;
        }

        IModStatus clonedModStatus = FastCloner.FastCloner.DeepClone(currentModStatus) ??
                                     throw new InvalidOperationException($"Failed to Clone the current Mod Status");

        if (makeEmptyPlayset)
        {
            clonedModStatus.Mods = clonedModStatus.Mods.Select(mod =>
            {
                mod.Hidden = true;
                return mod;
            }).ToList();
        }

        await SavePlaysetModStatus(fileName, clonedModStatus);
    }

    private async Task CreateAllOnPlayset(IModStatus currentModStatus)
    {
        if (DoesPlaysetExist(DEFAULT_ON_PLAYSET_FILE_NAME))
        {
            return;
        }

        IModStatus? allOnStatus = FastCloner.FastCloner.DeepClone(currentModStatus) ??
                                  throw new InvalidOperationException($"Failed to Clone the current Mod Status");
        allOnStatus.Mods = allOnStatus.Mods.Select(mod =>
        {
            mod.Enabled = true;
            return mod;
        }).ToList();

        await SavePlaysetModStatus(DEFAULT_ON_PLAYSET_FILE_NAME, allOnStatus);
    }

    private async Task CreateAllOffPlayset(IModStatus currentModStatus)
    {
        if (DoesPlaysetExist(DEFAULT_OFF_PLAYSET_FILE_NAME))
        {
            return;
        }

        IModStatus allOffStatus = FastCloner.FastCloner.DeepClone(currentModStatus) ??
                                  throw new InvalidOperationException($"Failed to Clone the current Mod Status");

        allOffStatus.Mods = allOffStatus.Mods.Select(mod =>
        {
            mod.Enabled = false;
            return mod;
        }).ToList();

        await SavePlaysetModStatus(DEFAULT_OFF_PLAYSET_FILE_NAME, allOffStatus);
    }

    /// <summary>
    /// Checks if a playset with the given file name already exists.
    /// Adds the ".json" extension if not present.
    /// </summary>
    /// <param name="fileName">
    /// The name of the playset file to check for existence.
    /// </param>
    /// <returns>
    /// True if the playset exists, false otherwise.
    /// </returns>
    private bool DoesPlaysetExist(string fileName)
    {
        string filePath = Path.Combine(PlaysetsLocation, EnsureExtension(fileName, ".json"));
        return File.Exists(filePath);
    }
}
