using System.Collections;
using UnityEngine;

/// <summary>
/// Simple GrenadeLauncher lisible et autonome.
/// Hérite toujours de CombatStyle (conserve intégration avec le système du projet).
/// Fonction : instancier la grenade au niveau du canon et lui donner une vitesse initiale
/// pour qu'elle suive une trajectoire en cloche et atterrisse sur la target.
/// </summary>
public class GrenadeLauncherCombatStyle : CombatStyle
{
    [Header("References")]
    [SerializeField] private GameObject m_GrenadePrefab;
    [SerializeField] private Transform m_FirePoint;
    [SerializeField] private RSO_PlayerAimTarget m_AimTarget;
    private Transform m_AimTargetTransform;

    [Header("Arc settings")]
    [Tooltip("Hauteur d'apex MIN (au dessus de la plus haute des deux positions)")]
    [SerializeField] private float m_MinArcHeight = 1f;

    [Tooltip("Hauteur d'apex MAX (au dessus de la plus haute des deux positions)")]
    [SerializeField] private float m_MaxArcHeight = 6f;

    [Tooltip("Hauteur d'apex désirée (sera clampée entre min et max)")]
    [SerializeField] private float m_DesiredArcHeight = 3f;

    [Header("Fallback / tuning")]
    [SerializeField] private float m_FallbackDistance = 12f;
    [SerializeField] private float m_FallbackSpeed = 10f;
    [Tooltip("Temps de rechargement en secondes après le tir")]
    [SerializeField] private float m_ReloadTime = 2f;

    [Header("Gizmos")]
    [SerializeField] private int m_TrajectorySamples = 30;

    
    private void Start()
    {
        if (m_AimTarget != null)
            m_AimTargetTransform = m_AimTarget.Get();
    }

    // ------------------ API principale ------------------
    public override IEnumerator Attack()
    {
        if (!m_CanAttack) yield break;
        
        m_IsAttacking = true;

        
        
        if (m_GrenadePrefab == null || m_FirePoint == null)
        {
            Debug.LogWarning("GrenadeLauncher: prefab ou firePoint non assigné.");
            yield break;
        }

        Vector3 origin = m_FirePoint.position;
        Vector3 targetPos = (m_AimTargetTransform != null) ? m_AimTargetTransform.position : (origin + m_FirePoint.forward * m_FallbackDistance);

        // Instancie la grenade
        GameObject grenade = Instantiate(m_GrenadePrefab, origin, m_FirePoint.rotation);
        if (grenade == null)
        {
            Debug.LogWarning("GrenadeLauncher: échec instantiation grenade.");
            yield break;
        }

        Rigidbody rb = grenade.GetComponent<Rigidbody>() ?? grenade.GetComponentInChildren<Rigidbody>();
        if (rb == null)
        {
            Debug.LogWarning("GrenadeLauncher: prefab sans Rigidbody. Ajouter-en un.");
            yield break;
        }

        // Calcule la vélocité initiale pour une trajectoire en cloche
        float arc = Mathf.Clamp(m_DesiredArcHeight, m_MinArcHeight, m_MaxArcHeight);
        Vector3 launchVel;
        bool solved = CalculateVelocityArc(origin, targetPos, arc, out launchVel);

        if (!solved)
        {
            // Fallback simple : lancer droit vers la cible
            Vector3 dir = (targetPos - origin).normalized;
            launchVel = dir * m_FallbackSpeed + Vector3.up * (m_FallbackSpeed * 0.1f);
        }

        // Applique la vélocité initiale
        rb.AddForce(launchVel, ForceMode.VelocityChange);

        grenade.GetComponent<Grenade>()?.ShowExplosionRadius(targetPos);
        OnAttack?.Invoke();
        
        Reload();
    }

    public override void Reload()
    {
        StartCoroutine(ReloadCooldown());
    }

    private IEnumerator ReloadCooldown()
    {
        m_CanAttack = false;
        yield return new WaitForSeconds(m_ReloadTime);
        m_CanAttack = true;
        OnReload?.Invoke();
    }

    // ------------------ Calcul balistique ------------------
    // Calcule une vélocité initiale qui atteint target en passant par un apex situé à "highestY + arcExtra".
    // Retourne false si le calcul est invalide.
    private bool CalculateVelocityArc(Vector3 origin, Vector3 target, float arcExtra, out Vector3 velocity)
    {
        velocity = Vector3.zero;

        float g = Mathf.Abs(Physics.gravity.y);

        Vector3 toTarget = target - origin;
        Vector3 toTargetXZ = new Vector3(toTarget.x, 0f, toTarget.z);
        float horizontalDistance = toTargetXZ.magnitude;

        // Apex = plus haute des deux positions + arcExtra
        float highestY = Mathf.Max(origin.y, target.y);
        float apexY = highestY + arcExtra;

        if (apexY <= origin.y || apexY <= target.y)
            return false;

        // Vy initiale pour atteindre l'apex
        float vy = Mathf.Sqrt(2f * g * (apexY - origin.y));
        float tUp = vy / g; // temps de montée

        // Temps de descente depuis l'apex jusqu'à la target
        float fall = apexY - target.y;
        if (fall < 0f) return false;
        float tDown = Mathf.Sqrt(2f * fall / g);

        float totalTime = tUp + tDown;
        if (totalTime <= 0f) return false;

        Vector3 horizontalDir = (horizontalDistance > 0f) ? toTargetXZ.normalized : Vector3.zero;
        float vx = horizontalDistance / totalTime;

        Vector3 horizontalVel = horizontalDir * vx;
        velocity = horizontalVel + Vector3.up * vy;
        return true;
    }

    // ------------------ Gizmos (aide tuning) ------------------
    private void OnDrawGizmosSelected()
    {
        if (m_FirePoint == null || m_GrenadePrefab == null) return;

        Vector3 origin = (m_FirePoint != null) ? m_FirePoint.position : transform.position;
        Vector3 targetPos = (m_AimTargetTransform != null) ? m_AimTargetTransform.position : (origin + (m_FirePoint != null ? m_FirePoint.forward : transform.forward) * m_FallbackDistance);

        float arc = Mathf.Clamp(m_DesiredArcHeight, m_MinArcHeight, m_MaxArcHeight);
        Vector3 vel;
        if (!CalculateVelocityArc(origin, targetPos, arc, out vel)) return;

        Gizmos.color = Color.yellow;
        Vector3 pos = origin;
        Vector3 v = vel;
        float dt = 0.05f;
        for (int i = 0; i < m_TrajectorySamples; i++)
        {
            Vector3 next = pos + v * dt + 0.5f * Physics.gravity * dt * dt;
            v += Physics.gravity * dt;
            Gizmos.DrawLine(pos, next);
            pos = next;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(targetPos, 0.15f);
    }
}