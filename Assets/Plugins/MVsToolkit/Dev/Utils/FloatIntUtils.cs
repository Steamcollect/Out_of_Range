using Unity.VisualScripting;
using UnityEngine;

public static class FloatIntUtils
{
    public static bool InRange(this float value, Vector2 range)
    {
        return (value >= range.x && value <= range.y)
            || (value <= range.x && value >= range.y);
    }
}
