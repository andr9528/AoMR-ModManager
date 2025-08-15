using System.Globalization;
using ModManager.Strings;

namespace ModManager.Abstractions.Services;

public interface ITranslationService
{
    /// <summary>
    /// Retrieves a localized string based on the provided key.
    /// Use <see cref="ResourceKeys"/> for available keys.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public string GetLocalizedString(string key);

    public string this[string key] { get; }

    public CultureInfo GetCurrentCulture();
}
