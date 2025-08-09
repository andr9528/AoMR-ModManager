namespace ModManager.Extensions;

public static class VisualTreeExtensions
{
    public static T? FindDescendantOfType<T>(this DependencyObject parent) where T : class
    {
        switch (parent)
        {
            case null:
                return null;
            // Check if the current element is of the desired type
            case T t:
                return t;
        }

        // Recurse through all child elements
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            DependencyObject? child = VisualTreeHelper.GetChild(parent, i);
            var result = child.FindDescendantOfType<T>();
            if (result != null)
            {
                return result;
            }
        }

        return null;
    }

    public static T? FindDescendantOfType<T>(this DependencyObject parent, Func<T, bool> predicate) where T : class
    {
        switch (parent)
        {
            case null:
                return null;
            // Check if the current element is of the desired type and matches the predicate
            case T t when predicate(t):
                return t;
        }

        // Recurse through all child elements
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            DependencyObject? child = VisualTreeHelper.GetChild(parent, i);
            T? result = child.FindDescendantOfType(predicate);
            if (result != null)
            {
                return result;
            }
        }

        return null;
    }
}
