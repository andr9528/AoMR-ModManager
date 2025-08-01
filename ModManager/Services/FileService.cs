using System.Reflection;
using ModManager.Abstractions.Models;
using ModManager.Abstractions.Services;
using Newtonsoft.Json;
using Path = System.IO.Path;

namespace ModManager.Services;

public class FileService : IFileService
{
    private const string MOD_STATUS_FILE_NAME = "myth-mod-status.json";
    private const string MOD_MANAGER_FOLDER = "ModManager";
    private const string PLAYSET_FOLDER = "Playsets";
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
        return Path.Combine(exeDir, MOD_STATUS_FILE_NAME);
    }

    /// <inheritdoc />
    public async Task ApplyModStatus(IModStatus newStatus)
    {
        string jsonPath = GetModStatusFilePath();

        string content = JsonConvert.SerializeObject(newStatus, Formatting.Indented);

        await File.WriteAllTextAsync(jsonPath, content);
    }

    /// <inheritdoc />
    public async Task SaveModStatus(string fileName, IModStatus modStatus)
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
}
