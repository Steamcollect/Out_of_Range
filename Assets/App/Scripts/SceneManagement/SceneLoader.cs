using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : RegularSingleton<SceneLoader>
{
    [SerializeField] int m_MainMenuSceneIndex;
    [SerializeField] int m_GameplaySceneIndex;

    [SerializeField] ScreenFadeManager m_FadeManager;
 
    [SerializeField] private RSE_OpenScene m_OpenScene;
    
    private Scene m_CurrentScene;
    private bool m_IsTransitioning = false;

    private void OnEnable()
    {
        m_OpenScene.Action += OpenScene;
    }

    private void OnDisable()
    {
        m_OpenScene.Action -= OpenScene;
    }

    private void Start()
    {
        LoadMainMenuScene();
    }

    private void OpenScene(string sceneName)
    {
        switch (sceneName)
        {
            case "MainMenu":
                ChangeScene(m_MainMenuSceneIndex);
                return;
            case "Gameplay":
                ChangeScene(m_GameplaySceneIndex);
                return;
            default:
                Debug.LogError("Scene not found: " + sceneName);
                break;
        }
    }
    
    private async void ChangeScene(int newSceneBuildIndex)
    {
        if (m_IsTransitioning)
            return;

        m_IsTransitioning = true;

        try
        {
            // Si une scène est déjà chargée, on fade out, on attend la fin du fade, puis on unload
            if (m_CurrentScene.isLoaded)
            {
                await m_FadeManager.FadeOutAsync();
                await SceneManager.UnloadSceneAsync(m_CurrentScene);
            }
            
            // Charge la nouvelle scène en additive
            var loadOp = SceneManager.LoadSceneAsync(newSceneBuildIndex, LoadSceneMode.Additive);
            await loadOp;

            var loadedScene = SceneManager.GetSceneByBuildIndex(newSceneBuildIndex);

            // Assure que la nouvelle scène devienne la scène active
            if (loadedScene.IsValid())
            {
                SceneManager.SetActiveScene(loadedScene);
                m_CurrentScene = loadedScene;
            }

            // Après le load, on fade in et on attend la fin du fade
            if (m_FadeManager != null)
            {
                await m_FadeManager.FadeInAsync();
            }
        }
        finally
        {
            m_IsTransitioning = false;
        }
    }
    
    public void LoadMainMenuScene()
    {
        ChangeScene(m_MainMenuSceneIndex);
    }
    
    public void LoadGameplayScene()
    {
        ChangeScene(m_GameplaySceneIndex);
    }
}

