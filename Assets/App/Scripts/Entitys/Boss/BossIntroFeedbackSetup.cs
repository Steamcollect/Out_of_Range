using UnityEngine;

public class BossIntroFeedbackSetup : MonoBehaviour
{
    [SerializeField] GameObject[] m_ObjectToEnable;
    [SerializeField] GameObject[] m_ObjectToDisable;

    private void Start()
    {
        if (!BossSave.IsFirstTimeSeeingBoss)
        {
            foreach (var obj in m_ObjectToDisable)
            {
                obj.SetActive(false);
            }
            foreach (var obj in m_ObjectToEnable)
            {
                obj.SetActive(true);
            }
        }
    }

    public void HaveSeenBoss()
    {
        BossSave.IsFirstTimeSeeingBoss = false;
    }
}