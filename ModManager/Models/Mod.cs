using System.Text.Json.Serialization;
using ModManager.Abstractions.Models;

namespace ModManager.Models;

public class Mod : IMod
{
    /// <inheritdoc />
    public string Title { get; set; }

    /// <inheritdoc />
    public string Author { get; set; }

    /// <inheritdoc />
    public string Description { get; set; }

    /// <inheritdoc />
    public string Path { get; set; }

    /// <inheritdoc />
    [JsonPropertyName(IMod.JSON_PROPERTY_WORKSHOP_ID_NAME)]
    public long WorkshopId { get; set; }

    /// <inheritdoc />
    public string LastUpdate { get; set; }

    /// <inheritdoc />
    public string InstallTime { get; set; }

    /// <inheritdoc />
    public int Priority { get; set; }

    /// <inheritdoc />
    public bool Enabled { get; set; }

    /// <inheritdoc />
    [JsonPropertyName(IMod.JSON_PROPERTY_INSTALL_CRC_NAME)]
    public uint? InstallCrc { get; set; }

    /// <inheritdoc />
    public bool Hidden { get; set; }
}
