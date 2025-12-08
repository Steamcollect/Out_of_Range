using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class BulletManager : MonoBehaviour
{
    //[Header("Input")]
    //[Header("Output")]

    public static BulletManager S_Instance;

    [FormerlySerializedAs("startingBulletCount")]
    [Header("Settings")]
    [SerializeField] private int m_StartingBulletCount = 30;

    [FormerlySerializedAs("bulletPrefab")]
    [Header("References")]
    [SerializeField] private Bullet m_BulletPrefab;

    private readonly Queue<Bullet> m_Bullets = new();

    private void Awake()
    {
        S_Instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < m_StartingBulletCount; i++)
        {
            Bullet bullet = CreateBullet();
            bullet.gameObject.SetActive(false);
            m_Bullets.Enqueue(bullet);
        }
    }

    public Bullet GetBullet()
    {
        if (m_Bullets.Count <= 0) return CreateBullet();

        Bullet bullet = m_Bullets.Dequeue();
        bullet.gameObject.SetActive(true);
        return bullet;
    }

    public void ReturnBullet(Bullet bullet)
    {
        bullet.gameObject.SetActive(false);
        m_Bullets.Enqueue(bullet);
    }

    private Bullet CreateBullet()
    {
        return Instantiate(m_BulletPrefab, transform);
    }
}