using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputFlagsOptimizer : MonoBehaviour
{
    private void Start()
    {
        InputSystem.settings.SetInternalFeatureFlag("USE_OPTIMIZED_CONTROLS", true);
        InputSystem.settings.SetInternalFeatureFlag("USE_READ_VALUE_CACHING", true);
    }
}
