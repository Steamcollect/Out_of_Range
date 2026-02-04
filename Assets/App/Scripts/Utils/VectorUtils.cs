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
    
    public static Vector3 DirectionRelativeToCamera(this Vector3 directionInput)
    {
        if (Camera.main == null) return directionInput;
        float targetAngle = Mathf.Atan2(directionInput.x, directionInput.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
        return  Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
    }
}