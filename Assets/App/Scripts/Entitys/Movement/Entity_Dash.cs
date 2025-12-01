using System.Collections;
using UnityEngine;

public class Entity_Dash : MonoBehaviour
{
    [Header("SETTINGS")]
    [SerializeField] private float dashCooldown;
    [Space(10)]
    [SerializeField] private ForceMode dashForceMode;
    [Space(10)]
    [SerializeField] private float dashDrag;
    [SerializeField] private float dashForce;
    [SerializeField] private float dadhTime;
    [SerializeField] private float invicibilityTime;

    private float beginDrag;
    private bool canDash = true;

    [Header("REFERENCES")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private EntityHealth entityHealth;

    public void Dash(Vector3 input)
    {
        if (!canDash) return;

        beginDrag = rb.linearDamping;
        rb.linearDamping = dashDrag;
        entityHealth.GainInvincibility(invicibilityTime);

        rb.AddForce(input * dashForce, dashForceMode);

        StartCoroutine(DashTime());
        StartCoroutine(DashCooldown());
    }

    IEnumerator DashTime()
    {
        yield return new WaitForSeconds(dadhTime);
        rb.linearVelocity = rb.linearVelocity.normalized;
        rb.linearDamping = beginDrag;
    }

    IEnumerator DashCooldown()
    {
        canDash = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}