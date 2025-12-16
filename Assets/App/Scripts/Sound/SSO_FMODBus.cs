using UnityEngine;

[CreateAssetMenu(fileName = "SSO_FMODBus", menuName = "SSO/FMOD/SSO_FMODBus")]
public class SSO_FMODBus : ScriptableObject
{
    [SerializeField] private string m_BusPath;
    
    public FMOD.Studio.Bus Bus => FMODUnity.RuntimeManager.GetBus(m_BusPath);
}