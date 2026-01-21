using UnityEngine;
using MVsToolkit.Wrappers;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "RSO_CurrentpowerUp", menuName = "RSO/powerUp/RSO_CurrentpowerUp")]
public class RSO_CurrentPowerUp : RuntimeScriptableObject<HashSet<PowerUp>>
{
    public bool ContainPowerUp(PowerUpType type)
    {
        foreach (PowerUp powerUp in value)
        {
            if (powerUp.PowerUpType == type) return true;
        }

        return false;
    }
}