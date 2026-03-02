using DG.Tweening;
using MVsToolkit.Utilities;
using UnityEngine;

public class InfiniteManaCollectible : MonoBehaviour, IHealth, ITargetable
{
    [Header("Settings")]
    [SerializeField] float m_TimeToRespawn = 15;
    float m_StartScale;

    [Space(10)]
    [SerializeField] Vector3 m_TargetPos;
    [SerializeField] Vector3 m_TargetIndicatorPos;

    [Header("References")]
    [SerializeField] ManaCreator m_ManaCreator;
    public Transform Buttom;

    [Space(10)]
    [SerializeField] MeshRenderer m_Renderer;
    [SerializeField] Animator m_Anim;
    [SerializeField] Collider m_Collid, m_HealthCollid;

    //[Header("Input")]
    //[Header("Output")]

    void Start()
    {
        m_StartScale = transform.localScale.x;
        m_Anim.speed = Random.Range(.5f, 1.5f);
    }

    public void TakeDamage(float damage)
    {
        transform.DOKill();
        transform.localScale = Vector3.one;

        m_Collid.enabled = false;
        m_HealthCollid.enabled = false;
        m_Renderer.enabled = false;
        m_ManaCreator.Create();

        this.Delay(() =>
        {
            m_Collid.enabled = true;
            m_HealthCollid.enabled = true;
            m_Renderer.enabled = true;

            transform.DOScale(m_StartScale * 1.2f, .08f).OnComplete(() => { transform.DOScale(m_StartScale, .12f); }); ;
        }, m_TimeToRespawn);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            TakeDamage(0);
        }
    }

    public void Die(){}

    public virtual Vector3 GetTargetPosition()
    {
        return transform.position + m_TargetPos;
    }

    public Vector3 GetTargetIndicatorPosition()
    {
        return transform.position + m_TargetIndicatorPos;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(GetTargetPosition(), .2f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(GetTargetIndicatorPosition(), .2f);
    }
}