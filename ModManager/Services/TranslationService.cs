using System.Globalization;
using System.Resources;
using ModManager.Abstractions.Services;
using ModManager.Strings;

namespace ModManager.Services;

public class TranslationService : ITranslationService
{
    private const string RESOURCE_MANAGER_PATH = "ModManager.Strings.Resources";
    private readonly ILocalizationService localizationService;
    private readonly ILogger<TranslationService> logger;
    private readonly ResourceManager resourceManager;

    public TranslationService(ILocalizationService localizationService, ILogger<TranslationService> logger)
    {
        this.localizationService = localizationService;
        this.logger = logger;

        this.logger.LogInformation($"Creating new Resource Manager");
        resourceManager = new ResourceManager(RESOURCE_MANAGER_PATH, typeof(Resources).Assembly);
    }

    /// <inheritdoc />
    public string GetLocalizedString(string key)
    {
        CultureInfo currentCulture = GetCurrentCulture();

        string? localizedString = resourceManager.GetString(key, currentCulture);

        if (!string.IsNullOrWhiteSpace(localizedString))
        {
            return localizedString;
        }

        logger.LogWarning($"Failed to get a localized string for key '{key}' in culture '{currentCulture.Name}'.");
        return key;
    }

    public string this[string key] => GetLocalizedString(key);

    /// <inheritdoc />
    public CultureInfo GetCurrentCulture()
    {
        return localizationService.CurrentCulture;
    }
}
