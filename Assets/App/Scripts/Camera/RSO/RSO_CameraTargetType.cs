using UnityEngine;
using MVsToolkit.Wrappers;

[CreateAssetMenu(fileName = "RSO_CameraTargetType", menuName = "RSO/Camera/RSO_CameraTargetType")]
public class RSO_CameraTargetType : RuntimeScriptableObject<CameraTargetType>{}

public enum CameraTargetType
{
    AutoFocus,
    FreeLook,
}