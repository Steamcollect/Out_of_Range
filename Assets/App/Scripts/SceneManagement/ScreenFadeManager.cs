using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

    public class ScreenFadeManager : MonoBehaviour
    {
        public Action OnFadeInComplete;
        public Action OnFadeOutComplete;

        [SerializeField] private Image m_FadeImage;
        [SerializeField] private float m_FadeDuration = 1f;
        
        private void Start()
        {
            FadeIn();
        }

        /// <summary>
        /// Fade qui rend la scene visible
        /// </summary>
        public void FadeIn()
        {
            StopAllCoroutines();
            if (m_FadeImage == null)
            {
                OnFadeInComplete?.Invoke();
                return;
            }

            // Démarre un fade de l'alpha actuel vers 0
            float startAlpha = m_FadeImage.color.a;
            StartCoroutine(FadeCoroutine(startAlpha, 0f, true));  
        }
        
        /// <summary>
        /// Fade qui rend la scene invisible
        /// </summary>
        public void FadeOut()
        {
            StopAllCoroutines();
            if (m_FadeImage == null)
            {
                OnFadeOutComplete?.Invoke();
                return;
            }

            // Démarre un fade de l'alpha actuel vers 1
            float startAlpha = m_FadeImage.color.a;
            StartCoroutine(FadeCoroutine(startAlpha, 1f, false));
        }

        /// <summary>
        /// Returns a Task that completes when the fade in finishes.
        /// </summary>
        public Task FadeInAsync()
        {
            if (m_FadeImage == null)
                return Task.CompletedTask;

            var tcs = new TaskCompletionSource<bool>();
            System.Action handler = null;
            handler = () =>
            {
                OnFadeInComplete -= handler;
                tcs.TrySetResult(true);
            };

            OnFadeInComplete += handler;
            FadeIn();
            return tcs.Task;
        }

        /// <summary>
        /// Returns a Task that completes when the fade out finishes.
        /// </summary>
        public Task FadeOutAsync()
        {
            if (m_FadeImage == null)
                return Task.CompletedTask;

            var tcs = new TaskCompletionSource<bool>();
            Action handler = null;
            handler = () =>
            {
                OnFadeOutComplete -= handler;
                tcs.TrySetResult(true);
            };

            OnFadeOutComplete += handler;
            FadeOut();
            return tcs.Task;
        }
        
        private IEnumerator FadeCoroutine(float fromAlpha, float toAlpha, bool isFadeIn)
        {
            if (m_FadeImage == null)
                yield break;

            // Si la durée est nulle ou négative, appliquer immédiatement
            if (m_FadeDuration <= 0f)
            {
                Color instant = m_FadeImage.color;
                instant.a = toAlpha;
                m_FadeImage.color = instant;
                m_FadeImage.enabled = !Mathf.Approximately(toAlpha, 0f);

                if (isFadeIn)
                    OnFadeInComplete?.Invoke();
                else
                    OnFadeOutComplete?.Invoke();

                yield break;
            }

            m_FadeImage.enabled = true;

            float elapsed = 0f;
            Color color = m_FadeImage.color;
            // Assure le départ depuis fromAlpha
            color.a = fromAlpha;
            m_FadeImage.color = color;

            while (elapsed < m_FadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / m_FadeDuration);
                float a = Mathf.Lerp(fromAlpha, toAlpha, t);
                color = m_FadeImage.color;
                color.a = a;
                m_FadeImage.color = color;
                yield return null;
            }

            // Assure la valeur finale exacte
            color = m_FadeImage.color;
            color.a = toAlpha;
            m_FadeImage.color = color;

            // Désactive l'image si totalement transparente
            if (Mathf.Approximately(toAlpha, 0f))
                m_FadeImage.enabled = false;

            if (isFadeIn)
                OnFadeInComplete?.Invoke();
            else
                OnFadeOutComplete?.Invoke();
        }
    }