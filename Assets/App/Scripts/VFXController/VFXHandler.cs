using UnityEngine;
using UnityEngine.VFX;

public class VFXHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private VisualEffect m_DeathEffect;


    public void CreateVfx()
    {
        Instantiate(m_DeathEffect, transform.position, Quaternion.identity).Play();
    }
}