using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class VFXPrewarm : MonoBehaviour
{
    [SerializeField] private VisualEffect[] m_VFXToPrewarm;

    private IEnumerator Start()
    {
        foreach (var vfx in m_VFXToPrewarm)
        {
            vfx.Play();
            yield return new WaitForEndOfFrame();
            vfx.Stop();
        }
    }
}