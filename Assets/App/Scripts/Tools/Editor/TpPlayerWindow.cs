using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

public class TpPlayerWindow : EditorWindow
{
    Checkpoint[] checkpoints;
    ListView listView;

    [MenuItem("Tools/Tp Player Checkpoints (UIE)")]
    public static void Open()
    {
        GetWindow<TpPlayerWindow>("Tp Player Checkpoints");
    }

    private void OnEnable()
    {
        RefreshCheckpoints();
        CreateUI();
    }

    void CreateUI()
    {
        rootVisualElement.Clear();

        var refreshButton = new Button(() =>
        {
            RefreshCheckpoints();
            listView.itemsSource = checkpoints;
            listView.Rebuild();
        })
        {
            text = "Refresh"
        };

        rootVisualElement.Add(refreshButton);

        listView = new ListView(
            checkpoints,
            itemHeight: 24,
            makeItem: () =>
            {
                var row = new VisualElement();
                row.style.flexDirection = FlexDirection.Row;

                var btn = new Button();
                btn.style.flexGrow = 1;
                btn.style.unityTextAlign = TextAnchor.MiddleLeft;
                btn.style.paddingLeft = 6;

                var objField = new ObjectField
                {
                    objectType = typeof(Checkpoint),
                    allowSceneObjects = true,
                    style = { width = 120 }
                };

                row.Add(btn);
                row.Add(objField);

                return row;
            },
            bindItem: (element, i) =>
            {
                var checkpoint = checkpoints[i];
                var btn = element.Q<Button>();
                var objField = element.Q<ObjectField>();

                btn.text = checkpoint.name;
                objField.value = checkpoint;

                btn.clicked += () =>
                {
                    if (Application.isPlaying)
                        checkpoint.TpPlayer();
                };
            }
        );

        listView.selectionType = SelectionType.None;
        rootVisualElement.Add(listView);
    }

    void RefreshCheckpoints()
    {
        checkpoints = Resources.FindObjectsOfTypeAll<Checkpoint>()
            .Where(c => c != null && c.gameObject.scene.IsValid())
            .OrderBy(c => c.name)
            .ToArray();
    }
}