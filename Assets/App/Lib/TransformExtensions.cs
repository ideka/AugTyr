using UnityEngine;

public static class TransformExtensions
{
    public static T Instantiate<T>(this Transform parent, T prefab) where T : Component
        => Object.Instantiate(prefab.gameObject, parent).GetComponent<T>();
}
