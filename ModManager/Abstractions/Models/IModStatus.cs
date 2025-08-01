namespace ModManager.Abstractions.Models;

public interface IModStatus
{
    public List<object> Actions { get; set; }
    public List<IMod> Mods { get; set; }
}
