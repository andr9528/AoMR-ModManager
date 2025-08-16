using ModManager.Abstractions.Models;

namespace ModManager.Extensions;

public static class ModelExtensions
{
    public static bool IsMatchingMod(this IMod mod, IMod taggedMod)
    {
        if (mod.WorkshopId != 0 && taggedMod.WorkshopId != 0 && mod.WorkshopId == taggedMod.WorkshopId)
        {
            return true;
        }

        return mod.Title.Equals(taggedMod.Title, StringComparison.InvariantCultureIgnoreCase) &&
               mod.IsLocalMod == taggedMod.IsLocalMod;
    }
}
