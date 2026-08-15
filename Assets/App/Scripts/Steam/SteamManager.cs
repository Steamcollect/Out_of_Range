using UnityEngine;

public class SteamManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] int m_MaxTimeForRobotKiller;
    float m_GameTime;
    public static bool canCountGameTime = false;
    public static bool haveTakeDamage = false;

    //[Header("References")]
    [Header("Input")]
    [SerializeField] RSE_UnlockSteamAchievment m_UnlockAchievment;
    [SerializeField] RSO_PlayerController m_PlayerController;

    //[Header("Output")]

    private void OnEnable()
    {
        m_UnlockAchievment.Action += UnlockAchievment;
        m_PlayerController.Value.GetHealth().OnTakeDamage += () => haveTakeDamage = true;
    }
    private void OnDisable()
    {
        m_UnlockAchievment.Action -= UnlockAchievment;   
    }

    private void Start()
    {
        try
        {
            Steamworks.SteamClient.Init(5114230);
            canCountGameTime = false;
            haveTakeDamage = false;
        }
        catch(System.Exception e)
        {
            Debug.Log(e);
        }
    }

    private void Update()
    {
        if(canCountGameTime)
            m_GameTime += Time.deltaTime;
    }

    private void OnApplicationQuit()
    {
        Steamworks.SteamClient.Shutdown();
    }

    public void UnlockAchievment(string id)
    {
        print(m_GameTime);
        var ach = new Steamworks.Data.Achievement(id);
        ach.Trigger();

        if(id == "TheEnd")
        {
            if(m_GameTime <= m_MaxTimeForRobotKiller && !haveTakeDamage)
            {
                ach = new Steamworks.Data.Achievement("RobotKiller");
                ach.Trigger();
            }
        }
    }
}