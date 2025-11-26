using UnityEngine;

public class EntityShield : MonoBehaviour, IShield
{
    [Header("Settings")]
    [SerializeField] int maxShield;
    int currentShield;

    //[Header("References")]
    //[Header("Input")]
    //[Header("Output")]

    void Start()
    {
        currentShield = maxShield;
    }

    public int TakeDamage(int damage)
    {
        currentShield -= damage;

        if(currentShield < 0)
        {
            return currentShield * -1;
        }

        return 0;
    }

    public bool IsDestroy() {  return currentShield <= 0; }
}