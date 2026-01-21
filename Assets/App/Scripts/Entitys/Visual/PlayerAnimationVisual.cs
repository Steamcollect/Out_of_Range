using System.Collections;
using MVsToolkit.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerAnimationVisual : MonoBehaviour
{
    [FoldoutGroup("Idle Anim"), SerializeField] float m_Speed;
    [FoldoutGroup("Idle Anim"), SerializeField] float m_Amplitude;

    [FoldoutGroup("Run Anim"), SerializeField] float m_HeadMovementDistOffset;
    [FoldoutGroup("Run Anim"), SerializeField] float m_HeadMovementTime;

    [FoldoutGroup("Run Anim"), SerializeField] float m_ArmsMovementTime;

    Vector3 m_HeadMovementVelocity;
    Vector3 m_ArmsMovementVelocity;

    [FoldoutGroup("Look At Rotation"), SerializeField] float m_ArmsRotationTime;
    [FoldoutGroup("Look At Rotation"), SerializeField] float m_HeadRotationTime;

    Vector3 m_ArmsRotationVelocity;
    Vector3 m_HeadRotationVelocity;

    [FoldoutGroup("Dash Anim"), SerializeField] PlayerClone m_playerClonePrefab;
    [FoldoutGroup("Dash Anim"), SerializeField] int m_CloneCountPerDash;
    [FoldoutGroup("Dash Anim"), SerializeField] float m_CloneLifeTime;

    [Space(10)]
    [FoldoutGroup("Dash Anim"), SerializeField] bool m_ChangePlayerMat = true;
    [FoldoutGroup("Dash Anim"), SerializeField] GameObject[] m_PlayerMeshs;
    [FoldoutGroup("Dash Anim"), SerializeField] GameObject[] m_PlayerCloneMeshs;

    PlayerClone[] m_Clones;

    [Header("References")]
    [SerializeField] Transform m_ArmsPivot;
    [SerializeField] Transform m_HeadPivot;

    //[Header("Input")]
    //[Header("Output")]

    private void Start()
    {
        m_ArmsPivot.SetParent(null);

        Transform cloneParent = new GameObject("Player Clones").transform;
        m_Clones = new PlayerClone[m_CloneCountPerDash];
        for (int i = 0; i < m_Clones.Length; i++)
        {
            m_Clones[i] = Instantiate(m_playerClonePrefab, cloneParent);
            m_Clones[i].gameObject.SetActive(false);
            m_Clones[i].Init();
        }
    }

    private void Update()
    {
        m_HeadPivot.localPosition += (Vector3.up * Mathf.Sin(Time.time * m_Speed) * m_Amplitude);

        m_ArmsPivot.position = Vector3.SmoothDamp(
    m_ArmsPivot.position,
    transform.position,
    ref m_ArmsMovementVelocity,
    m_ArmsMovementTime);
    }

    public void OnMove(Vector3 direction)
    {
        m_HeadPivot.localPosition = Vector3.SmoothDamp(
            m_HeadPivot.localPosition,
            direction * m_HeadMovementDistOffset,
            ref m_HeadMovementVelocity,
            m_HeadMovementTime);
    }

    public void RotateToward(Vector3 target)
    {
        target.y = m_ArmsPivot.position.y;
        m_ArmsPivot.LookAtSmoothDamp(target, ref m_ArmsRotationVelocity, m_ArmsRotationTime);
        m_HeadPivot.LookAtSmoothDamp(target, ref m_HeadRotationVelocity, m_HeadRotationTime);
    }

    public IEnumerator OnDash(float dashTime)
    {
        if (m_ChangePlayerMat)
        {
            foreach (var mesh in m_PlayerCloneMeshs)
                mesh.SetActive(true);
            foreach (var mesh in m_PlayerMeshs)
                mesh.SetActive(false);
        }

        for (int i = 0; i < m_Clones.Length; i++)
        {
            m_Clones[i].transform.position = transform.position;
            m_Clones[i].SetPivots(m_HeadPivot.localPosition, m_ArmsPivot.localPosition - transform.position);
            m_Clones[i].SetRotations(m_HeadPivot.localRotation, m_ArmsPivot.localRotation);
            m_Clones[i].gameObject.SetActive(true);

            m_Clones[i].Fade(m_CloneLifeTime);

            yield return new WaitForSeconds(dashTime / m_CloneCountPerDash);
        }

        if (m_ChangePlayerMat)
        {
            foreach (var mesh in m_PlayerCloneMeshs)
                mesh.SetActive(false);
            foreach (var mesh in m_PlayerMeshs)
                mesh.SetActive(true);
        }
    }
}