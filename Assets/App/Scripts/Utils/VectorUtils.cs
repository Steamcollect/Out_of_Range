using UnityEngine;
using Vector2 = System.Numerics.Vector2;

public static class VectorUtils
{
    public static float Max(this Vector2 vector)
    {
        return Mathf.Max(vector.X, vector.Y);
    }

    public static float Min(this Vector2 vector)
    {
        return Mathf.Min(vector.X, vector.Y);
    }

    public static float Max(this Vector3 vector)
    {
        return Mathf.Max(vector.x, Mathf.Max(vector.y, vector.z));
    }

    public static float Min(this Vector3 vector)
    {
        return Mathf.Min(vector.x, Mathf.Min(vector.y, vector.z));
    }

    public static float Distance(this Vector3 a, Vector3 b)
    {
        return Vector3.Distance(a, b);
    }
}