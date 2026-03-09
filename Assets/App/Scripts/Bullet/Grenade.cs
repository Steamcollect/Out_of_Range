using System.Collections;
using UnityEngine.VFX;
using UnityEngine;
using UnityEngine.Serialization;
using MVsToolkit.Dev;

public class Grenade : MonoBehaviour
{
    [Header("Combat")]
    [SerializeField] float m_ExplosionRadius = 5;
    [SerializeField] int m_Damage = 1;
    [SerializeField] bool m_IsPlayerGrenade = false;
    [SerializeField,ShowIf("m_IsPlayerGrenade", true)] float m_BossStunTime;

    [SerializeField] LayerMask m_HurtBoxLayers;

    [Header("Movement")]
    [SerializeField] float m_MovementTime = 1;
    [SerializeField] float m_MinHeight = 3;
    [SerializeField] AnimationCurve m_MovementCurve;

    [Header("References")]
    [SerializeField] VisualEffect m_WarningEffect;
    [FormerlySerializedAs("m_ExplosionEffect")]
    [Space]
    [SerializeField] GameObject m_ExplosionEffectPrefab;

    Vector3 m_StartingPos, m_TargetPos;

    static readonly Collider[] s_CollidHit = new Collider[100];

    public void Setup(Vector3 initPos, Vector3 targetPos)
    {
        m_StartingPos = initPos;
        m_TargetPos = targetPos;
    }

    public void Move(float moveTimeMult = 1, float explosionRadiusMult = 1)
    {
        m_WarningEffect.transform.SetParent(null);
        m_WarningEffect.transform.position = m_TargetPos;
        m_WarningEffect.gameObject.SetActive(true);
        
        m_WarningEffect.SetFloat("ChargingTime", m_MovementTime * moveTimeMult);
        m_WarningEffect.SetFloat("ExplosionRadius", m_ExplosionRadius * explosionRadiusMult);

        StartCoroutine(Movement(m_MovementTime * moveTimeMult, explosionRadiusMult));
    }

    IEnumerator Movement(float moveTime, float explodeRadiusMult)
    {
        float t = 0;
        float vt;

        while (t < moveTime)
        {
            vt = t / moveTime;

            transform.position = new Vector3(
                Mathf.Lerp(m_StartingPos.x, m_TargetPos.x, vt),
                Mathf.Lerp(m_StartingPos.y, m_TargetPos.y, vt) + m_MovementCurve.Evaluate(vt) * m_MinHeight,
                Mathf.Lerp(m_StartingPos.z, m_TargetPos.z, vt));

            t += Time.deltaTime;
            yield return null;
        }

        Explode(explodeRadiusMult);
    }

    void Explode(float explodeRadiusMult)
    {
        m_WarningEffect.gameObject.SetActive(false);

        PoolManager.Instance.Spawn(m_ExplosionEffectPrefab, transform.position, Quaternion.identity);
        
        // m_ExplosionEffectPrefab.transform.SetParent(null);
        // m_ExplosionEffectPrefab.gameObject.SetActive(true);

        int length = Physics.OverlapSphereNonAlloc(transform.position, m_ExplosionRadius * explodeRadiusMult, s_CollidHit, m_HurtBoxLayers);

        if (s_CollidHit.Length > 0)
        {
            for (int i = 0; i < length; i++)
            {
                if (s_CollidHit[i].TryGetComponent(out HurtBox hurtBox))
                {
                    hurtBox.TakeDamage(m_Damage);

                    if (m_IsPlayerGrenade
                        && hurtBox.transform.parent.parent.parent.TryGetComponent(out BossEnemyController boss)
                        && boss.GetMovement() is BossMovementController movement)
                        {
                            movement.Stun(m_BossStunTime);
                        }
                }
            }
        }

        Destroy(gameObject);
    }

    public float GetRadius() => m_ExplosionRadius;
    public AnimationCurve GetMovementCurve() => m_MovementCurve;
    public float GetMinHeight() => m_MinHeight;

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, m_ExplosionRadius);
    }
}