using UnityEngine;

public interface ICameraTarget
{
    Vector3? GetCameraTargetPosition();
    
    ITargetable GetCameraTarget();
}