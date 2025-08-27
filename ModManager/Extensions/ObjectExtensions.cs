namespace ModManager.Extensions;

public static class ObjectExtensions
{
    public static T DeepClone<T>(this T obj) where T : class?
    {
        return FastCloner.FastCloner.DeepClone<T>(obj) ??
               throw new InvalidOperationException($"Failed to Clone target object of type '{typeof(T).FullName}'");
    }
}
