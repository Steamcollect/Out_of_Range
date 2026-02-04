using MVsToolkit.Wrappers;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "RSE_OnInputDeviceChanged", menuName = "RSE/Input/RSE_OnInputDeviceChanged")]
public class RSE_OnInputDeviceChanged : RuntimeScriptableEvent<InputDeviceType> { }

