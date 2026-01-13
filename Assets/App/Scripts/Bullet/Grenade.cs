using System.Collections;
using UnityEngine.VFX;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [Header("Combat")]
    [SerializeField] float m_ExplosionRadius = 5;
    [SerializeField] int m_Damage = 1;

    [SerializeField] LayerMask m_HurtBoxLayers;

    [Header("Movement")]
    [SerializeField] float m_MovementTime = 1;
    [SerializeField] float m_MinHeight = 3;
    [SerializeField] AnimationCurve m_MovementCurve;

    [Header("References")]
    [SerializeField] VisualEffect m_WarningEffectPrefab;
    VisualEffect m_WarningEffect;

    Vector3 m_StartingPos, m_TargetPos;

    static Collider[] s_CollidHit = new Collider[100];

    public void Setup(Vector3 initPos, Vector3 targetPos)
    {
        m_StartingPos = initPos;
        m_TargetPos = targetPos;
    }

    public void Move()
    {
        m_WarningEffect = Instantiate(m_WarningEffectPrefab, m_TargetPos, Quaternion.identity);
        m_WarningEffect.SetFloat("ChargingTime", m_MovementTime);
        m_WarningEffect.SetFloat("ExplosionRadius", m_ExplosionRadius);

        StartCoroutine(Movement());
    }

    IEnumerator Movement()
    {
        float t = 0;
        float vt;

        while (t < m_MovementTime)
        {
            vt = t / m_MovementTime;

            transform.position = new Vector3(
                Mathf.Lerp(m_StartingPos.x, m_TargetPos.x, vt),
                Mathf.Lerp(m_StartingPos.y, m_TargetPos.y, vt) + m_MovementCurve.Evaluate(vt) * m_MinHeight,
                Mathf.Lerp(m_StartingPos.z, m_TargetPos.z, vt));

            t += Time.deltaTime;
            yield return null;
        }

        Explode();
    }

    void Explode()
    {
        Destroy(m_WarningEffect.gameObject);

        int length = Physics.OverlapSphereNonAlloc(transform.position, m_ExplosionRadius, s_CollidHit, m_HurtBoxLayers);

        if (s_CollidHit.Length > 0)
        {
            for (int i = 0; i < length; i++)
            {
                if (s_CollidHit[i].TryGetComponent(out HurtBox hurtBox))
                {
                    hurtBox.TakeDamage(m_Damage);
                }
            }
        }

        Destroy(gameObject);
    }

    public float GetRadius() => m_ExplosionRadius;
    public AnimationCurve GetMovementCurve() => m_MovementCurve;
    public float GetMinHeight() => m_MinHeight;
}