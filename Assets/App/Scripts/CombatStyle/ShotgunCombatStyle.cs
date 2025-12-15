using System.Collections;
using UnityEngine;

public class ShotgunCombatStyle : CombatStyle
{
    [SerializeField] private int m_ShotsPerMagazine = 2;
    [SerializeField] private int m_BulletsPerShot = 8;
    [SerializeField] private float m_SpreadAngle = 15f;
    [SerializeField] private float m_AttackCooldown = 1f;
    [SerializeField] private float m_ReloadCooldown = 1f;

    [Space(10)]
    [SerializeField] int m_BulletDamage;
    [SerializeField] float m_BulletSpeed;
    [SerializeField] float m_KnockBackForce;
    
    [SerializeField] Transform m_AttackPoint;
    
    [SerializeField] private Bullet m_BulletPrefab;
    
    public override IEnumerator Attack()
    {
        if (!m_CanAttack) yield break;
        
        if (m_CurrentMana.Get() >= manaCostPerAttack)
        {
            m_CurrentMana.Set(m_CurrentMana.Get() - manaCostPerAttack);
        }
        else
        {
            yield break;
        }
        
        m_IsAttacking = true;
        m_CanAttack = false;
        
        OnAttack?.Invoke();
        
        for (int i = 0; i < m_BulletsPerShot; i++)
        {
            float angle = Random.Range(-m_SpreadAngle / 2, m_SpreadAngle / 2);
            Quaternion rotation = Quaternion.Euler(0, angle, 0);
            Vector3 direction = rotation * m_AttackPoint.forward;

            Bullet bullet = PoolManager.Instance.Spawn(m_BulletPrefab, m_AttackPoint.position, rotation);
            bullet.transform.up = direction;

            bullet.Setup();
        }
        
        m_ShotsPerMagazine--;

        if (m_ShotsPerMagazine <= 0)
        {
            Reload();
        }
        else
        {
            StartCoroutine(AttackCooldown());
        }
    }
    
    public override void Reload()
    {
        StartCoroutine(ReloadCooldown());
        OnReload?.Invoke();
    }
    
    private IEnumerator ReloadCooldown()
    {
        yield return new WaitForSeconds(m_ReloadCooldown);
        m_CanAttack = true;
        m_IsAttacking = false;
    }

    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(m_AttackCooldown);
        m_CanAttack = true;
        m_IsAttacking = false;
    }
}
