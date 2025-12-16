using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UI_Manager : MonoBehaviour
{
    [Title("SETTINGS")]
    [SerializeField] private string m_OpenPanelName = "Menu";
    [SerializeField] private string m_ClosePanelName = "Game";

    [Title("REFERENCES")]
    [SerializeField] private InputActionReference m_OpenPauseMenuAction;

    //[Title("LOADING SCREEN")]
    //[SerializeField] private UI_LoadingScreen m_LoadingScreen;

    [Title("INPUTS")]
    [SerializeField] private RSE_OpenScene m_OpenScene;
    [SerializeField] private RSE_QuitGame m_QuitGame;

    [Title("OUTPUTS")]
    [SerializeField] private RSE_OpenPanel m_OpenPanel;

    private bool m_IsOpen = false;

    private void OnEnable()
    {
        m_OpenPauseMenuAction.action.performed += PauseMenuButton;
        m_OpenPauseMenuAction.action.Enable();
        m_OpenScene.Action += OpenScene;
        m_QuitGame.Action += Quit;
    }

    private void OnDisable()
    {
        m_OpenPauseMenuAction.action.performed -= PauseMenuButton;
        m_OpenPauseMenuAction.action.Disable();
        m_OpenScene.Action -= OpenScene;
        m_QuitGame.Action -= Quit;
    }

    private void PauseMenuButton(InputAction.CallbackContext ctx)
    {
        if (!m_IsOpen)
            OpenPauseMenu();
        else
            ClosePauseMenu();
    }

    public void OpenPauseMenu()
    {
        m_IsOpen = true;
        Cursor.visible = true;
        Time.timeScale = 0f;
        m_OpenPanel.Call(m_OpenPanelName);
    }

    public void ClosePauseMenu()
    {
        m_IsOpen = false;
        Cursor.visible = false;
        Time.timeScale = 1f;
        m_OpenPanel.Call(m_ClosePanelName);
    }

    public void OpenScene(string sceneName)
    {
        StartCoroutine(OpenSceneWithAnimation(sceneName));
    }

    IEnumerator OpenSceneWithAnimation(string sceneName)
    {
        m_OpenPanel.Call("LoadingScreen");
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(sceneName);
        //m_LoadingScreen.LoadScene(sceneName);
    }

    public void Quit()
    {
        // Save Game Here
        Application.Quit();
    }
}