using Sirenix.OdinInspector;
using UnityEngine;

public class BossPaternManager : MonoBehaviour
{
    [SerializeField] bool hasGrenadePatern = false;
    [SerializeField] bool hasRiflePatern = true;

    [SerializeField] BossEnemyController m_Controller;

    [SerializeField] CombatPatern m_RiflePatern;
    [SerializeField] CombatPatern m_GrenadePatern;

    [Space(10)]
    [SerializeField] RSE_AddGrenadePatern m_AddGrenade;
    [SerializeField] RSE_AddRiflePatern m_AddRifle;
    [SerializeField] RSE_RemoveRiflePatern m_RemoveRifle;
    [SerializeField] RSE_AddPowerUpsBoss m_AddPowerUps;



    private void OnEnable()
    {
        m_AddGrenade.Action += AddGrenade;
        m_AddRifle.Action += AddRifle;
        m_RemoveRifle.Action += RemoveRifle;
        m_AddPowerUps.Action += AddPowerUps;
    }

    private void OnDisable()
    {
        m_AddGrenade.Action -= AddGrenade;
        m_AddRifle.Action -= AddRifle;
        m_RemoveRifle.Action -= RemoveRifle;
        m_AddPowerUps.Action -= AddPowerUps;
    }

    private void Start()
    {
        if(hasRiflePatern) AddRifle();
        if(hasGrenadePatern) AddGrenade();
    }

    [Button]
    public void AddPowerUps()
    {
        m_Controller.HaveAtkSpeedPowerUp = true;
        m_Controller.HaveClonePowerUp = true;
        m_Controller.HaveStrenghtPowerUp = true;
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