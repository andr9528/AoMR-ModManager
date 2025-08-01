using System.Text.Json.Serialization;

namespace ModManager.Abstractions.Models;

public interface IMod
{
    public const string JSON_PROPERTY_WORKSHOP_ID_NAME = "WorkshopID";
    public const string JSON_PROPERTY_INSTALL_CRC_NAME = "InstallCRC";

    public string Title { get; set; }
    public string Author { get; set; }
    public string Description { get; set; }
    public string Path { get; set; }

    [JsonPropertyName(JSON_PROPERTY_WORKSHOP_ID_NAME)]
    public long WorkshopId { get; set; }

    public string LastUpdate { get; set; }
    public string InstallTime { get; set; }
    public int Priority { get; set; }
    public bool Enabled { get; set; }

    [JsonPropertyName(JSON_PROPERTY_INSTALL_CRC_NAME)]
    public uint? InstallCrc { get; set; }

    public bool Hidden { get; set; }
}
