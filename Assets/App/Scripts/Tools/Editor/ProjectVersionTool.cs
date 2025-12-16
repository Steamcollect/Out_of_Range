using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public static class ProjectVersionTool
{
    private const int k_MenuPriority = 2000; // place le menu vers la fin

    private static string s_CurrentVersion
    {
        get
        {
            if (string.IsNullOrEmpty(PlayerSettings.bundleVersion))
                PlayerSettings.bundleVersion = "0.0";
            return PlayerSettings.bundleVersion;
        }
        set
        {
            PlayerSettings.bundleVersion = value;
            Debug.Log($"Nouvelle version du projet : {value}");
        }
    }

    // --- Affiche la version courante comme label ---
    [MenuItem("Tools/Version/Version actuelle : X.Y", false, k_MenuPriority)]
    private static void ShowVersion() { }

    [MenuItem("Tools/Version/Version actuelle : X.Y", true)]
    private static bool ValidateShowVersion()
    {
        // On affiche toujours, mais on d�sactive le clic
        string path = "Version/Version actuelle : X.Y";
        // On ne peut pas changer dynamiquement le texte du chemin,
        // donc on affiche la valeur dans la console pour rappel :
        Menu.SetChecked(path, false);
        return false;
    }

    // --- Incr�ments ---
    [MenuItem("Tools/Version/Incremente Major", false, k_MenuPriority + 1)]
    private static void IncrementMajor()
    {
        string[] parts = ParseVersion();

        if (int.TryParse(parts[0], out var major))
        {
            parts[0] = (major + 1).ToString();
        }
        else
        {
            parts[0] = "0";
        }
        
        SaveVersion(parts);
    }
    
    [MenuItem("Tools/Version/Incremente Minor Auto (git commit Id)", false, k_MenuPriority + 2)]
    private static void IncrementMinor()
    {
        string[] parts = ParseVersion();
        parts[1] = GetGitCommitHash();
        SaveVersion(parts);
    }   
    
    [MenuItem("Tools/Version/Incremente Minor Manually", false, k_MenuPriority + 3)]
    private static void SetMinorManually()
    {
        ManualMinorInputWindow.ShowWindow();
    }
    
    private static string GetGitCommitHash()
    {
        string gitCommitHash = "";
        
        try
        {
            Process process = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = "git",
                    Arguments = "rev-parse --short HEAD",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            gitCommitHash = process.StandardOutput.ReadLine();
            process.WaitForExit();
            process.Close();
            gitCommitHash = gitCommitHash?.Trim();

        }
        catch
        {
            Debug.LogWarning("Failed to get git commit hash.");
            gitCommitHash = "0";
        }

        return gitCommitHash;
    }

    // --- Helpers ---
    private static string[] ParseVersion()
    {
        string[] tokens = s_CurrentVersion.Split('.');
        if (tokens.Length < 2)
            return new string[] { "0", "0" };
        return new string[] { tokens[0], tokens[1] };
    }

    private static void SaveVersion(string[] parts)
    {
        string newVersion = "";
        for (var i = 0; i < parts.Length; i++)
        {
            if (i == 0)
            {
                newVersion = parts[i];
            }
            else
            {
                newVersion += $".{parts[i]}";
            }
        }
        s_CurrentVersion = newVersion;
        Debug.Log($"Version mise a jour : {newVersion}");
    }
    
    private class ManualMinorInputWindow : EditorWindow
    {
        private string m_minorValue = "";
    
        public static void ShowWindow()
        {
            var window = GetWindow<ManualMinorInputWindow>("Set Minor Version");
            window.minSize = new Vector2(200, 70);
        }
    
        private void OnGUI()
        {
            GUILayout.Label("Enter new minor version:", EditorStyles.boldLabel);
            m_minorValue = EditorGUILayout.TextField("Minor", m_minorValue);
    
            GUILayout.Space(10);
    
            if (GUILayout.Button("Set Minor"))
            {
                if (!string.IsNullOrEmpty(m_minorValue))
                {
                    string[] parts = ProjectVersionTool.ParseVersion();
                    parts[1] = m_minorValue;
                    typeof(ProjectVersionTool)
                        .GetMethod("SaveVersion", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
                        .Invoke(null, new object[] { parts });
                    Close();
                }
                else
                {
                    EditorUtility.DisplayDialog("Error", "Please enter a value.", "OK");
                }
            }
        }
    }
    
}