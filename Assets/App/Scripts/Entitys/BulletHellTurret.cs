using System;
using System.Collections;
using UnityEngine;

public class BulletHellTurret : MonoBehaviour
{
    public GameObject muzzle;
    [SerializeField] private int m_BulletsPerShot = 5;
    [SerializeField] private float m_TimeBetweenBullets = 0.1f;
    [SerializeField] private bool m_ResetRotationAfterPattern = true;
    [SerializeField] private float m_TimeBetweenPatterns = 5f;
    [SerializeField] private float m_RotationSpeed;
    [SerializeField] private Bullet m_BulletPrefab;
    
    private int currentShotIndex = 0;
    
    private void Start()
    {
        StartCoroutine(RotationCoroutine());
        StartCoroutine(ShootRoutine());
    }

    private void ResetRotation()
    {
        transform.rotation = Quaternion.identity;
    }
    
    private IEnumerator ShootRoutine()
    {
        while (true)
        {
            Shoot();
            if (currentShotIndex >= m_BulletsPerShot)
            {
                yield return new WaitForSeconds(m_TimeBetweenPatterns);
                currentShotIndex = 0;
                if (m_ResetRotationAfterPattern)
                    ResetRotation();
            }
            else
            {
                yield return new WaitForSeconds(m_TimeBetweenBullets);
            }
        }
    }
    
    private IEnumerator RotationCoroutine()
    {
        while (true)
        {
            transform.Rotate(Vector3.up, m_RotationSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private void Shoot()
    {
        var bullet = PoolManager.Instance.Spawn(m_BulletPrefab, muzzle.transform.position, muzzle.transform.rotation);
        bullet.Setup();
        currentShotIndex++;
    }
    
    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
