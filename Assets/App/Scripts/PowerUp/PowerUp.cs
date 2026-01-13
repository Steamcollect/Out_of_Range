[System.Serializable]
public class PowerUp
{
    public PowerUpType PowerUpType;

    public PowerUp(PowerUpType type) {  PowerUpType = type; }
}

public enum PowerUpType
{
    Strenght,
    AttackSpeed,
    Clone,
    Ammo
}