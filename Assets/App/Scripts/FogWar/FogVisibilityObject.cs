/*
 * Created :    Winter 2022
 * Author :     SeungGeon Kim (keithrek@hanmail.net)
 * Project :    FogWar
 * Filename :   csFogVisibilityAgent.cs (non-static monobehaviour module)
 * 
 * All Content (C) 2022 Unlimited Fischl Works, all rights reserved.
 */

/*
 * This script is just an example of what you can do with the visibility check interface.
 * You can create whatever agent that you want based on this script.
 * Also, I recommend you to change the part where the FogWar module is fetched with Find()...
 */

using UnityEngine;
using System.Collections.Generic;


namespace FischlWorks_FogWar
{
    public class FogVisibilityObject : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool m_Visibility = false;
        [SerializeField] [Range(0, 2)] private int m_AdditionalRadius = 0;
        
        [Header("References")]
        [SerializeField] private List<MeshRenderer> m_MeshRenderers = null;
        [SerializeField] private List<SkinnedMeshRenderer> m_SkinnedMeshRenderers = null;

        private void Update()
        {
            if (!IsInRangeFogWar()) return;

            UpdateVisibilityObject();
        }

        private bool IsInRangeFogWar() => FogWarManager.HasInstance() && FogWarManager.Instance.CheckWorldGridRange(transform.position);

        private void UpdateVisibilityObject()
        {
            m_Visibility = FogWarManager.Instance.CheckVisibility(transform.position, m_AdditionalRadius);

            foreach (MeshRenderer meshRenderer in m_MeshRenderers)
            {
                meshRenderer.enabled = m_Visibility;
            }

            foreach (SkinnedMeshRenderer skinnedMeshRenderer in m_SkinnedMeshRenderers)
            {
                skinnedMeshRenderer.enabled = m_Visibility;
            }
        }


#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (FogWarManager.Instance == null || Application.isPlaying == false)
            {
                return;
            }

            if (FogWarManager.Instance.CheckWorldGridRange(transform.position) == false)
            {
                Gizmos.color = Color.red;

                Gizmos.DrawWireSphere(
                    new Vector3(
                        Mathf.RoundToInt(transform.position.x),
                        0,
                        Mathf.RoundToInt(transform.position.z)),
                    (FogWarManager.Instance._UnitScale / 2.0f) + m_AdditionalRadius);

                return;
            }

            Gizmos.color = FogWarManager.Instance.CheckVisibility(transform.position, m_AdditionalRadius)
                ? Color.green : Color.yellow;

            Gizmos.DrawWireSphere(
                new Vector3(
                    Mathf.RoundToInt(transform.position.x),
                    0,
                    Mathf.RoundToInt(transform.position.z)),
                (FogWarManager.Instance._UnitScale / 2.0f) + m_AdditionalRadius);
        }
#endif
    }
}