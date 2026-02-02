using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public sealed class FakeOverloadCombatStyle : MonoBehaviour
{
    [FoldoutGroup("Settings")]
    [SerializeField] private float m_CurrentTemperature;
    [SerializeField, FoldoutGroup("Overload Settings")] private OverloadWeaponState m_CurrentState;

    [Space(10)]
    [SerializeField, FoldoutGroup("Overload Settings")] private Vector2 m_RangeToReset = new Vector2(25, 35);
    [SerializeField, FoldoutGroup("Overload Settings")] private Vector2 m_RangeToBuff = new Vector2(35, 45);
    [SerializeField, FoldoutGroup("Overload Settings")] private Vector2 m_RangeToNerf = new Vector2(45, 55);

    [Space(10)]
    [SerializeField, FoldoutGroup("Simulation Settings")] private float m_DefaultCoolSpeed = 20f;
    [SerializeField, FoldoutGroup("Simulation Settings")] private float m_BuffCoolSpeed = 40f;
    [SerializeField, FoldoutGroup("Simulation Settings")] private float m_NerfCoolSpeed = 10f;

    
    [Header("References")]
    [SerializeField] private Bullet m_BulletPrefab;
    [SerializeField] private Transform m_AttackPoint;
    
    public Action<float, float> OnAmmoChange;
    public Action OnOverloadStart;
    public Action OnOverloadEnd;
    public Action<OverloadWeaponState> OnOverloadStateChange;

    private Coroutine m_CurrentSimulation;
    public Coroutine CurrentSimulation => m_CurrentSimulation;

    #region Getters (compatibles avec OverloadCombatStyle)
    public float GetCurrentTemperature() => m_CurrentTemperature;
    public OverloadWeaponState GetState() => m_CurrentState;
    public Vector2 GetRangeToBuff() => m_RangeToBuff;
    public Vector2 GetRangeToReset() => m_RangeToReset;
    #endregion

    #region Simulation Methods
    
    public void SimulateBuff()
    {
        StopCurrentSimulation();
        m_CurrentSimulation = StartCoroutine(SimulateBuffSequence());
    }

    public void SimulateReset()
    {
        StopCurrentSimulation();
        m_CurrentSimulation = StartCoroutine(SimulateResetSequence());
    }

    public void SimulateNerf()
    {
        StopCurrentSimulation();
        m_CurrentSimulation = StartCoroutine(SimulateNerfSequence());
    }
    
    public void SimulateFullOverload()
    {
        StopCurrentSimulation();
        m_CurrentSimulation = StartCoroutine(SimulateFullOverloadSequence());
    }

    
    public void ResetState()
    {
        StopCurrentSimulation();
        m_CurrentTemperature = 0;
        m_CurrentState = OverloadWeaponState.CanShoot;
        OnAmmoChange?.Invoke(m_CurrentTemperature, 100);
    }

    #endregion

    #region Simulation Sequences

    private IEnumerator SimulateBuffSequence()
    {
        yield return StartCoroutine(SimulateHeatUp());

        float targetTemp = (m_RangeToBuff.x + m_RangeToBuff.y) / 2f;
        yield return StartCoroutine(SimulateCoolDown(targetTemp, m_DefaultCoolSpeed));

        m_CurrentState = OverloadWeaponState.CoolBuffed;
        m_CurrentTemperature = 100;
        OnOverloadStateChange?.Invoke(m_CurrentState);
        OnOverloadEnd?.Invoke();
        OnAmmoChange?.Invoke(m_CurrentTemperature, 100);

        yield return StartCoroutine(SimulateCoolDown(0, m_BuffCoolSpeed, true));

        m_CurrentState = OverloadWeaponState.CanShoot;
        OnAmmoChange?.Invoke(m_CurrentTemperature, 100);
    }

    private IEnumerator SimulateResetSequence()
    {
        yield return StartCoroutine(SimulateHeatUp());

        float targetTemp = (m_RangeToReset.x + m_RangeToReset.y) / 2f;
        yield return StartCoroutine(SimulateCoolDown(targetTemp, m_DefaultCoolSpeed));

        m_CurrentState = OverloadWeaponState.CanShoot;
        m_CurrentTemperature = 0;
        OnOverloadStateChange?.Invoke(m_CurrentState);
        OnOverloadEnd?.Invoke();
        OnAmmoChange?.Invoke(m_CurrentTemperature, 100);
    }

    private IEnumerator SimulateNerfSequence()
    {
        yield return StartCoroutine(SimulateHeatUp());

        float targetTemp = (m_RangeToNerf.x + m_RangeToNerf.y) / 2f;
        yield return StartCoroutine(SimulateCoolDown(targetTemp, m_DefaultCoolSpeed));

        m_CurrentState = OverloadWeaponState.CoolNerfed;
        m_CurrentTemperature = 100;
        OnOverloadStateChange?.Invoke(m_CurrentState);
        OnOverloadEnd?.Invoke();
        OnAmmoChange?.Invoke(m_CurrentTemperature, 100);

        yield return StartCoroutine(SimulateCoolDown(0, m_NerfCoolSpeed));

        m_CurrentState = OverloadWeaponState.CanShoot;
        OnAmmoChange?.Invoke(m_CurrentTemperature, 100);
    }

    private IEnumerator SimulateFullOverloadSequence()
    {
        yield return StartCoroutine(SimulateHeatUp());

        yield return StartCoroutine(SimulateCoolDown(0, m_DefaultCoolSpeed));

        m_CurrentState = OverloadWeaponState.CanShoot;
        OnOverloadEnd?.Invoke();
        OnAmmoChange?.Invoke(m_CurrentTemperature, 100);
    }

    private IEnumerator SimulateHeatUp()
    {
        m_CurrentState = OverloadWeaponState.CanShoot;
        
        while (m_CurrentTemperature < 100)
        {
            m_CurrentTemperature += 15f;
            m_CurrentTemperature = Mathf.Min(m_CurrentTemperature, 100);
            
            Bullet bullet = PoolManager.Instance.Spawn(m_BulletPrefab, m_AttackPoint.position, m_AttackPoint.rotation);
            bullet.Setup();
            
            OnAmmoChange?.Invoke(m_CurrentTemperature, 100);
            yield return new WaitForSeconds(0.2f);
        }

        m_CurrentTemperature = 100;
        m_CurrentState = OverloadWeaponState.Overload;
        OnOverloadStart?.Invoke();
        OnAmmoChange?.Invoke(m_CurrentTemperature, 100);
        
        yield return new WaitForSeconds(0.5f);

        m_CurrentState = OverloadWeaponState.OverloadCool;
    }

    private IEnumerator SimulateCoolDown(float targetTemp, float coolSpeed, bool shootBullets = false)
    {
        float fireTimer = 0f;
        float fireRate = 0.1f;
        
        while (m_CurrentTemperature > targetTemp)
        {
            if (shootBullets)
            {
                fireTimer += Time.deltaTime;
                if (fireTimer >= fireRate)
                {
                    fireTimer = 0f;
                    Bullet bullet = PoolManager.Instance.Spawn(m_BulletPrefab, m_AttackPoint.position, m_AttackPoint.rotation);
                    bullet.Setup();
                }
            }
            
            m_CurrentTemperature -= coolSpeed * Time.deltaTime;
            m_CurrentTemperature = Mathf.Max(m_CurrentTemperature, targetTemp);
            OnAmmoChange?.Invoke(m_CurrentTemperature, 100);
            yield return null;
        }
    }

    #endregion

    private void StopCurrentSimulation()
    {
        if (m_CurrentSimulation == null) return;
        StopCoroutine(m_CurrentSimulation);
        m_CurrentSimulation = null;
    }
}

