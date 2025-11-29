using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerHealthUI : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] Color enableColor;
    [SerializeField] Color disableColor;

    [Header("References")]
    [SerializeField] Transform content;
    [SerializeField] Image healthPointRefImg;

    List<Image> points = new();

    [Header("Input")]
    [SerializeField] RSO_PlayerController playerController;

    //[Header("Output")]

    private void Start()
    {
        EntityHealth health = playerController.Get().GetHealth();
        health.OnTakeDamage += ()=>
        {
            SetFillValue(health.GetCurrentHealth(), health.GetMaxHealth());
        };
        
        health.OnDeath+= ()=>
        {
            SetFillValue(0, health.GetMaxHealth());
        };

        SetFillValue(health.GetCurrentHealth(), health.GetMaxHealth());
    }

    public void SetFillValue(int value, int max)
    {
        if (points.Count != max)
        {
            foreach (Image point in points)
            {
                Destroy(point.gameObject);
                points.Clear();
            }

            for (int i = 0; i < max; i++)
            {
                points.Add(Instantiate(healthPointRefImg, content));
                points[i].gameObject.SetActive(true);
            }
        }

        for (int i = 0; i < points.Count; i++)
        {
            if (i + 1 <= value)
                points[i].color = enableColor;
            else 
                points[i].color = disableColor;
        }
    }
}