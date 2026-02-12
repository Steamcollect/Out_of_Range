using Sirenix.OdinInspector;
using UnityEngine;

public class BossPaternManager : MonoBehaviour
{
    [SerializeField] BossEnemyController m_Controller;

    [SerializeField] CombatPatern m_RiflePatern;
    [SerializeField] CombatPatern m_GrenadePatern;

    private void Start()
    {
        AddRifle();
    }

    [Button]
    public void AddRifle() => m_Controller.AddPatern(m_RiflePatern);
    [Button]
    public void RemoveRifle()=>m_Controller.RemovePatern(m_RiflePatern);

    [Button]
    public void AddGrenade()=>m_Controller.AddPatern(m_GrenadePatern);
    [Button]
    public void RemoveGrenade() => m_Controller.RemovePatern(m_GrenadePatern);
}

[System.Serializable]
public struct CombatPatern
{
    public EntityCombat Combat;
    [Range(0, 100)] public float UseProbability;
}