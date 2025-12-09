using UnityEngine;

public static class ComponentUtils
{
    public static bool TryGetComponentInChildrens<T>(this GameObject go, out T component) where T : class
    {
        if (go == null)
        {
            component = null;
            return false;
        }

        // if not interface and component
        if (!typeof(T).IsInterface && typeof(Component).IsAssignableFrom(typeof(T)))
        {
            component = go.GetComponentInChildren<T>(true);
            return component != null;
        }

        // if interface
        Component[] found = go.GetComponentsInChildren(typeof(T), true);
        foreach (var c in found)
        {
            if (c is T t)
            {
                component = t;
                return true;
            }
        }

        component = null;
        return false;
    }

    public static bool TryGetComponentInChildrens<T>(this Component c, out T component) where T : class
    {
        if (c == null || c.gameObject == null)
        {
            component = null;
            return false;
        }

        // if not interface and component
        if (!typeof(T).IsInterface && typeof(Component).IsAssignableFrom(typeof(T)))
        {
            component = c.GetComponentInChildren<T>(true);
            return component != null;
        }

        // if interface
        Component[] found = c.GetComponentsInChildren(typeof(T), true);
        foreach (var comp in found)
        {
            if (comp is T t)
            {
                component = t;
                return true;
            }
        }

        component = null;
        return false;
    }
}