using UnityEditor;
using UnityEngine;

public class TpPlayerWindow : EditorWindow
{
    Checkpoint[] checkpoints;

    [MenuItem("Tool/TpPlayerCheckoints")]
    public static void Open()
    {
        GetWindow<TpPlayerWindow>("Tp Player Checkoints");
    }

    private void OnEnable()
    {
        RefreshCheckpoints();
    }

    private void OnGUI()
    {
        if(GUILayout.Button("Refresh"))
        {
            RefreshCheckpoints();
        }

        GUILayout.Space(10);

        foreach (Checkpoint checkpoint in checkpoints)
        {
            if(checkpoint == null)
            {
                RefreshCheckpoints();
                return;
            }

            if (GUILayout.Button(checkpoint.gameObject.name))
            {
                if (!Application.isPlaying) return;
                checkpoint.TpPlayer();
            }
        }
    }

    void RefreshCheckpoints()
    {
        checkpoints = FindObjectsByType<Checkpoint>(FindObjectsSortMode.InstanceID);
    }
}