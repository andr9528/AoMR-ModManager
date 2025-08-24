using System.Collections.ObjectModel;
using System.Reflection;
using ModManager.Abstractions.Models;
using ModManager.Abstractions.Services;
using ModManager.Extensions;
using Newtonsoft.Json;
using Path = System.IO.Path;

namespace ModManager.Services;

public class FileService : IFileService
{
    private readonly ILogger<FileService> logger;
    private const string MOD_STATUS_FILE_NAME = "myth-mod-status";
    public const string MOD_MANAGER_FOLDER = "ModManager";
    private const string PLAYSET_FOLDER = "Playsets";
    private const string DEFAULT_OFF_PLAYSET_FILE_NAME = "all-off";
    private const string DEFAULT_ON_PLAYSET_FILE_NAME = "all-on";
    private static string PlaysetsLocation;

    public FileService(ILogger<FileService> logger)
    {
        this.logger = logger;

        PlaysetsLocation = Path.Combine(GetExecutionDirectory(), MOD_MANAGER_FOLDER, PLAYSET_FOLDER);
        if (!Directory.Exists(PlaysetsLocation))
        {
            Directory.CreateDirectory(PlaysetsLocation);
        }
    }

    /// <inheritdoc />
    public async Task<IModStatus> GetCurrentModStatus()
    {
        try
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
        catch (Exception e)
        {
            logger.LogError(e, "Failed to get current mod status.");
            throw;
        }
    }

    private string GetExecutionDirectory()
    {
        return Environment.CurrentDirectory;
    }

    private string GetModStatusFilePath()
    {
        string exeDir = GetExecutionDirectory();
        return Path.Combine(exeDir, EnsureExtension(MOD_STATUS_FILE_NAME, ".json"));
    }

    /// <inheritdoc />
    public async Task ActivatePlayset(IPlayset playset)
    {
        try
        {
            await SaveModStatusChanges(playset.ModStatus);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to activate playset: {PlaysetName}", playset.FileName);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task SavePlayset(IPlayset playset)
    {
        try
        {
            string fileNameWithExtension = EnsureExtension(playset.FileName, ".json");
            string savePath = Path.Combine(PlaysetsLocation, fileNameWithExtension);

            string content = JsonConvert.SerializeObject(playset.ModStatus, Formatting.Indented);

            await File.WriteAllTextAsync(savePath, content);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to save playset: {PlaysetName}", playset.FileName);
            throw;
        }
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
        try
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
        catch (Exception e)
        {
            logger.LogError(e, "Failed to load mod status from file: {FileName}", fileName);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IList<IPlayset>> LoadPlaysets()
    {
        try
        {
            string[] files = Directory.GetFiles(PlaysetsLocation, "*.json");
            var modStatuses = await Task.WhenAll(files.Select(LoadModStatus));

            var playsets = files
                .Select((file, index) => new Playset(Path.GetFileNameWithoutExtension(file), modStatuses[index]))
                .ToList<IPlayset>();

            return playsets;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to load playsets from directory: {PlaysetsLocation}", PlaysetsLocation);
            throw;
        }
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
        try
        {
            foreach (IMod mod in currentModStatus.Mods)
            {
                foreach (IPlayset playset in playsets)
                {
                    IMod? playsetMod = playset.ModStatus.Mods.FirstOrDefault(x => x.IsMatchingMod(mod));

                    if (playsetMod == null)
                    {
                        IMod clonedMod = FastCloner.FastCloner.DeepClone(mod) ??
                                         throw new InvalidOperationException($"Failed to Clone the mod: {mod.Title}");
                        clonedMod.IsHidden = true;
                        playset.ModStatus.Mods.Add(clonedMod);
                        continue;
                    }

                    playsetMod.Description = mod.Description;
                    playsetMod.InstallCrc = mod.InstallCrc;
                    playsetMod.InstallTime = mod.InstallTime;
                    playsetMod.LastUpdate = mod.LastUpdate;
                    playsetMod.Path = mod.Path;
                    playsetMod.Title = mod.Title;
                }
            }

            await Task.WhenAll(playsets.Select(SavePlayset));
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to update playsets properties.");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task NewPlayset(IPlayset playset, bool makeEmptyPlayset = true)
    {
        try
        {
            if (DoesPlaysetExist(playset.FileName))
            {
                return;
            }

            IModStatus clonedModStatus = FastCloner.FastCloner.DeepClone(playset.ModStatus) ??
                                         throw new InvalidOperationException($"Failed to Clone the current Mod Status");

            if (makeEmptyPlayset)
            {
                clonedModStatus.Mods = new ObservableCollection<IMod>(clonedModStatus.Mods.Select(mod =>
                {
                    mod.IsHidden = true;
                    return mod;
                }));
            }

            var newPlayset = new Playset(playset.FileName, clonedModStatus);

            await SavePlayset(newPlayset);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to create new playset: {PlaysetName}", playset.FileName);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task SaveModStatusChanges(IModStatus? modStatus)
    {
        try
        {
            string jsonPath = GetModStatusFilePath();

            string content = JsonConvert.SerializeObject(modStatus, Formatting.Indented);

            await File.WriteAllTextAsync(jsonPath, content);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to save Mod Status");
            throw;
        }
    }

    private async Task CreateAllOnPlayset(IModStatus currentModStatus)
    {
        try
        {
            if (DoesPlaysetExist(DEFAULT_ON_PLAYSET_FILE_NAME))
            {
                return;
            }

            IModStatus? allOnStatus = FastCloner.FastCloner.DeepClone(currentModStatus) ??
                                      throw new InvalidOperationException($"Failed to Clone the current Mod Status");
            allOnStatus.Mods = new ObservableCollection<IMod>(allOnStatus.Mods.Select(mod =>
            {
                mod.IsEnabled = true;
                return mod;
            }).ToList());

            await SavePlayset(new Playset(DEFAULT_ON_PLAYSET_FILE_NAME, allOnStatus));
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to create 'All On' playset.");
            throw;
        }
    }

    private async Task CreateAllOffPlayset(IModStatus currentModStatus)
    {
        try
        {
            if (DoesPlaysetExist(DEFAULT_OFF_PLAYSET_FILE_NAME))
            {
                return;
            }

            IModStatus allOffStatus = FastCloner.FastCloner.DeepClone(currentModStatus) ??
                                      throw new InvalidOperationException($"Failed to Clone the current Mod Status");

            allOffStatus.Mods = new ObservableCollection<IMod>(allOffStatus.Mods.Select(mod =>
            {
                mod.IsEnabled = false;
                return mod;
            }).ToList());

            await SavePlayset(new Playset(DEFAULT_OFF_PLAYSET_FILE_NAME, allOffStatus));
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to create 'All Off' playset.");
            throw;
        }
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
