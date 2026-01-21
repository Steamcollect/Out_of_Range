using MVsToolkit.Wrappers;
using UnityEngine;

[CreateAssetMenu(fileName = "RSO_Mana", menuName = "RSO/Player/RSO_Mana")]
public class RSO_Mana : RuntimeScriptableObject<Mana>
{
   
}

public struct Mana
{
    public int CurrentMana;
    public int MaxMana;

    public bool HaveEngough(int value) => CurrentMana >= value;

    public void Remove(int value)
    {
        CurrentMana = Mathf.Clamp(CurrentMana - value, 0, MaxMana);
    }

    public void Add(int value)
    {
        CurrentMana = Mathf.Clamp(CurrentMana + value, 0, MaxMana);
    }
}