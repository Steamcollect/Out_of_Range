using System.Collections.Generic;
using UnityEngine;

public class RoomEntityConnector : MonoBehaviour
{
    //[Header("Settings")]
    //[Header("References")]
    List<EnemyController> m_Childs = new();

    //[Header("Input")]
    //[Header("Output")]

    private void Start()
    {
        foreach (EnemyController enemy in transform.GetComponentsInChildren<EnemyController>(true))
        {
            m_Childs.Add(enemy);
            enemy.GetHealth().OnTakeDamage += OnChildTakeDamage;
        }
    }

    void OnChildTakeDamage()
    {
        foreach (EnemyController child in m_Childs)
        {
            child.SetAware();
        }
    }
}