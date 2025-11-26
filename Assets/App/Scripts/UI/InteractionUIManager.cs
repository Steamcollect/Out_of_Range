using UnityEngine;

public class InteractionUIManager : MonoBehaviour
{
    [SerializeField] PointerUI pointerUIPrefab;
    [SerializeField] Transform content;

    public static InteractionUIManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public PointerUI CreatePointerUI()
    {
        return Instantiate(pointerUIPrefab, content);
    }
}
