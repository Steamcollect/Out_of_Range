using UnityEngine;

public interface ICameraTarget
{
    Vector3? GetCameraTargetPosition(ref bool isTagettingSomething);
    
    ITargetable GetCameraTarget();
}