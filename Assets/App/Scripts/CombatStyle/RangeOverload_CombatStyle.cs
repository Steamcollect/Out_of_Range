using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class RangeOverload_CombatStyle : CombatStyle
{
    [SerializeField, MVsToolkit.Dev.ReadOnly] float m_CurentTemperature;
    [SerializeField, MVsToolkit.Dev.ReadOnly] RangeOverloadWeaponState m_CurrentState;
    public enum RangeOverloadWeaponState
    {
        CanShoot = 0,
        Overload = 1,
        DefaultCool = 2,
        OverloadCool = 3,
        CoolBuffed = 4,
        CoolNerfed = 5
    }

    [Space(10)]
    [SerializeField, Tooltip("Delay between attacks")] float m_AttackCooldown;

    [Space(5)]
    [SerializeField, Tooltip("Temperature donnée par shoot")] float m_ShootTemperature;
    
    [Space(10)]
    [SerializeField, Tooltip("Refroidissement par seconde quand maximum pas atteint")] float m_DefaultCoolsPerSec;
    [SerializeField, Tooltip("Refroidissement par seconde quand maximum pas atteint")] float m_OverloadCoolsPerSec;
    [SerializeField, Tooltip("Refroidissement par seconde quand input raté")] float m_NerfCoolsPerSec;
    [SerializeField, Tooltip("Refroidissement par seconde quand input réussis")] float m_BuffCoolsPerSec;

    [Space(5)]
    [SerializeField] Vector2 m_RangeToReset;
    [SerializeField] Vector2 m_RangeToBuff;
    [SerializeField] Vector2 m_RangeToNerf;

    [Space(10)]
    [SerializeField] MeshRenderer m_MeshRenderer;
    [SerializeField] Gradient m_ColorOverTemperature;

    [SerializeField] Transform m_AttackPoint;

    [SerializeField] GameObject m_MuzzleFlashPrefab;
    [SerializeField] Bullet m_BulletPrefab;

    [Space(10)]
    [SerializeField] UnityEvent m_OnAttackFeedback;
    [SerializeField] UnityEvent m_OnReloadFeedback;

    Material m_RendererMat;

    [Header("Input")]
    [SerializeField] InputActionReference m_HandleCoolsInput;
    [SerializeField] InputActionReference m_HandleCoolsSkillInput;

    //[Header("Output")]

    private void OnEnable()
    {
        m_HandleCoolsInput.action.Enable();
        m_HandleCoolsSkillInput.action.Enable();

        m_HandleCoolsInput.action.started += Cool;
        m_HandleCoolsSkillInput.action.started += TryBuffOnCool;
    }

    private void OnDisable()
    {
        m_HandleCoolsInput.action.started -= Cool;
        m_HandleCoolsSkillInput.action.started -= TryBuffOnCool;
    }

    private void Start()
    {
        m_RendererMat = new Material(m_MeshRenderer.material);
        m_MeshRenderer.material = m_RendererMat;
        SetRendererColor();
    }

    private void Update()
    {
        switch (m_CurrentState)
        {
            case RangeOverloadWeaponState.DefaultCool:
                HandleCool(m_DefaultCoolsPerSec);
                break;
            case RangeOverloadWeaponState.OverloadCool:
                HandleCool(m_DefaultCoolsPerSec);
                break;

            case RangeOverloadWeaponState.CoolBuffed:
                HandleCool(m_BuffCoolsPerSec);
                break;
            case RangeOverloadWeaponState.CoolNerfed:
                HandleCool(m_NerfCoolsPerSec);
                break;
        }
    }

    void HandleCool(float coolSpeed)
    {
        m_CurentTemperature -= coolSpeed * Time.deltaTime;

        if(m_CurentTemperature <= 0)
        {
            m_CurentTemperature = 0;
            m_CurrentState = RangeOverloadWeaponState.CanShoot;
        }
    }

    public override IEnumerator Attack()
    {
        if (m_CanAttack 
            && (m_CurrentState == RangeOverloadWeaponState.CanShoot || m_CurrentState == RangeOverloadWeaponState.CoolBuffed))
        {
            OnAttack?.Invoke();
            
            Bullet bullet = PoolManager.Instance.Spawn(m_BulletPrefab, m_AttackPoint.position, m_AttackPoint.rotation);
            bullet.Setup();

            GameObject muzzleVFX = Instantiate(m_MuzzleFlashPrefab, m_AttackPoint);
            Destroy(muzzleVFX, muzzleVFX.GetComponent<ParticleSystem>().main.duration);

            StartCoroutine(AttackCooldown());
            
            m_OnAttackFeedback?.Invoke();

            m_CurentTemperature += m_ShootTemperature;
            if (m_CurentTemperature >= 100)
                OnOverload();

            SetRendererColor();
            OnAmmoChange?.Invoke(m_CurentTemperature, 100);

            yield break;
        }
    }

    void OnOverload()
    {
        m_CurentTemperature = 100;
        m_CurrentState = RangeOverloadWeaponState.Overload;
    }

    void Cool(InputAction.CallbackContext ctx)
    {
        switch (m_CurrentState)
        {
            case RangeOverloadWeaponState.CanShoot:
                m_CurrentState = RangeOverloadWeaponState.DefaultCool;
                break;
            case RangeOverloadWeaponState.Overload:
                m_CurrentState = RangeOverloadWeaponState.OverloadCool;
                break;
        }
    }

    void TryBuffOnCool(InputAction.CallbackContext ctx)
    {
        if (m_CurrentState != RangeOverloadWeaponState.OverloadCool) return;

        if (m_CurentTemperature.InRange(m_RangeToBuff))
        {
            m_CurrentState = RangeOverloadWeaponState.CoolBuffed;
            m_CurentTemperature = 100;
        }
        else if (m_CurentTemperature.InRange(m_RangeToReset))
        {
            m_CurrentState = RangeOverloadWeaponState.CanShoot;
            m_CurentTemperature = 0;
        }
        else if (m_CurentTemperature.InRange(m_RangeToNerf))
        {
            m_CurrentState = RangeOverloadWeaponState.CoolNerfed;
            m_CurentTemperature = 100;
        }
    }

    private IEnumerator AttackCooldown()
    {
        m_CanAttack = false;
        yield return new WaitForSeconds(m_AttackCooldown);
        m_CanAttack = true;
    }

    private void SetRendererColor()
    {
        float value = Mathf.Clamp01(m_CurentTemperature * .001f);
        m_RendererMat.color = m_ColorOverTemperature.Evaluate(value);
    }

    private void OnValidate()
    {
        m_RangeToReset.x = Mathf.Clamp(m_RangeToReset.x, 0, 100);
        m_RangeToReset.y = Mathf.Clamp(m_RangeToReset.y, 0, 100);

        m_RangeToBuff.x = Mathf.Clamp(m_RangeToBuff.x, 0, 100);
        m_RangeToBuff.y = Mathf.Clamp(m_RangeToBuff.y, 0, 100);

        m_RangeToNerf.x = Mathf.Clamp(m_RangeToNerf.x, 0, 100);
        m_RangeToNerf.y = Mathf.Clamp(m_RangeToNerf.y, 0, 100);
    }
}