using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimingBarAim : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TutorialAim m_TutorialLogic;
    [SerializeField] private List<TutorialAimTarget> m_Targets;

    [Header("UI Settings")]
    [SerializeField] private GameObject m_FeedbackPrefab;
    [SerializeField] private Vector3 m_Offset = new Vector3(0, 2f, 0);

    private Dictionary<TutorialAimTarget, Image> m_HudInstances = new Dictionary<TutorialAimTarget, Image>();

    private void Start()
    {
        foreach (var target in m_Targets)
        {
            GameObject go = Instantiate(m_FeedbackPrefab, transform);
            Image img = go.GetComponentInChildren<Image>();
            img.fillAmount = 0;
            img.transform.localScale = Vector3.zero; // Caché au début
            m_HudInstances.Add(target, img);
        }
    }

    private void Update()
    {
        foreach (var target in m_Targets)
        {
            UpdateTargetUI(target);
        }
    }

    private void UpdateTargetUI(TutorialAimTarget target)
    {
        Image hudImg = m_HudInstances[target];
        float progress = m_TutorialLogic.GetTargetProgress(target);

        // Positionner le HUD au dessus de la cible (World to Screen ou World Space)
        hudImg.transform.position = target.transform.position;
        hudImg.transform.localPosition += m_Offset;

        // Logique d'affichage
        if (progress > 0 && progress < 1f)
        {
            // Apparition si on commence à viser
            if (hudImg.transform.localScale.x == 0)
                hudImg.transform.DOScale(1f, 0.2f);

            hudImg.fillAmount = progress;
        }
        else if (progress >= 1f)
        {
            // Feedback de succès (Popup et disparition)
            if (hudImg.fillAmount < 1f)
            {
                hudImg.transform.DOScale(1.5f, 0.3f).SetEase(Ease.OutBack);
                hudImg.DOFade(0, 0.3f).OnComplete(() => hudImg.gameObject.SetActive(false));
            }
        }
        else
        {
            // Le joueur a arrêté de viser avant la fin : disparition simple
            if (hudImg.transform.localScale.x > 0 && !DOTween.IsTweening(hudImg.transform))
            {
                hudImg.transform.DOScale(0f, 0.2f);
                hudImg.fillAmount = 0;
            }
        }
    }
}
