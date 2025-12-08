using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class AmbianceManager : MonoBehaviour
{
    [SerializeField] private EventReference m_Ambiance;
    private EventInstance m_MusicInstance;

    private void Start()
    {
        m_MusicInstance = RuntimeManager.CreateInstance(m_Ambiance);
        m_MusicInstance.start();
    }
}