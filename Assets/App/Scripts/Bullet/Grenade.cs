using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class Grenade : MonoBehaviour
{
    [SerializeField] float m_ExplosionRadius = 5;
    [SerializeField] int m_Damage = 1;

    [SerializeField] GameObject m_WarningEffect;

    Vector3 m_StartingPos, m_TargetPos;

    public void Setup(Vector3 initPos, Vector3 targetPos)
    {
        m_StartingPos = initPos;
        m_TargetPos = targetPos;
    }

    public void Move()
    {
        Vector3 direction = m_TargetPos - m_StartingPos;
        Vector3 groundDirection = new Vector3(direction.x, 0, direction.z);

        Vector3 targetPos = new Vector3(groundDirection.magnitude, direction.y, 0);

        float height = targetPos.y + targetPos.magnitude / 2f;
        height = Mathf.Max(.01f, height);
        float angle, v0, time;
        CalculatePathWithHeight(targetPos, height, out v0, out angle, out time);

        Instantiate(m_WarningEffect, m_TargetPos, Quaternion.identity); 
        StartCoroutine(Movement(groundDirection.normalized, v0, angle, time));
    }

    float QuadraticEquation(float a, float b, float c, float sign)
    {
        return (-b + sign * Mathf.Sqrt(b * b - 4 * a * c)) / (2 * a);
    }

    void CalculatePathWithHeight(Vector3 targetPos, float h, out float v0, out float angle, out float time)
    {
        float xt = targetPos.x;
        float yt = targetPos.y;
        float g = -Physics.gravity.y;

        float b = Mathf.Sqrt(2 * g * h);
        float a = (-0.5f * g);
        float c = -yt;

        float tplus = QuadraticEquation(a, b, c, 1);
        float tmin = QuadraticEquation(a, b, c, -1);
        time = tplus > tmin ? tplus : tmin;

        angle = Mathf.Atan(b * time / xt);

        v0 = b / Mathf.Sin(angle);
    }

    IEnumerator Movement(Vector3 direction, float v0, float angle, float time)
    {
        float t = 0;

        while (t < time)
        {
            float x = v0 * t * Mathf.Cos(angle);
            float y = v0 * t * Mathf.Sin(angle) - .5f * -Physics.gravity.y * Mathf.Pow(t, 2);
            transform.position = m_StartingPos + direction * x + Vector3.up * y;

            t += Time.deltaTime;
            yield return null;
        }

        Explode();
    }

    void Explode()
    {
        Collider[] collidHit = Physics.OverlapSphere(transform.position, m_ExplosionRadius);

        if (collidHit.Length > 0)
        {
            foreach (Collider collid in collidHit)
            {
                if(collid.TryGetComponent(out HurtBox hurtBox))
                {
                    hurtBox.TakeDamage(m_Damage);
                }
            }
        }

        Destroy(gameObject);
    }
}