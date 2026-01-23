using UnityEngine;

[CreateAssetMenu(fileName = "SSO_PowerUp", menuName = "SSO/PowerUp")]
public class SSO_PowerUp : ScriptableObject
{
    [Header("Settings")]
    public PowerUpType PowerUpType;
    
    [Header("Render")]
    public Sprite Icon;

    public Color ColorIcon;
}

public enum PowerUpType
{
    Strength,
    AttackSpeed,
    Clone,
    Ammo
}