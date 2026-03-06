using UnityEngine;
public static class GameObjectExtensions
{
    /// <summary>
    /// Disable a component of type T on the GameObject
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="go"></param>
    public static void DisableComponent<T>(this GameObject go) where T : Component
    {
        var c = go.GetComponent<T>();
        if (c == null) return;

        if (c is Behaviour b)
            b.enabled = false;
        else if (c is Collider col)
            col.enabled = false;
        else if (c is Renderer r)
            r.enabled = false;
    }

    /// <summary>
    /// Disable all components of type T on the GameObject and its children
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="go"></param>
    public static void DisableAllComponents<T>(this GameObject go) where T : Component
    {
        var components = go.GetComponentsInChildren<T>();
        foreach (var c in components)
        {
            if (c is Behaviour b)
                b.enabled = false;
            else if (c is Collider col)
                col.enabled = false;
            else if (c is Renderer r)
                r.enabled = false;
        }
    }
}