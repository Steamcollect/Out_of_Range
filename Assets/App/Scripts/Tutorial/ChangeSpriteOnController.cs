using System.Linq;
using UnityEngine;

public class ChangeSpriteOnController : MonoBehaviour
{
    [SerializeField] SpriteRenderer graphics;
    [SerializeField]
    States[] states;
    [SerializeField] RSO_CurrentInputDeviceType m_CurrentInputDevice;

    [System.Serializable]
    public class States
    {
        public InputDeviceType type;

        public Sprite icon;
        public Color color;
    }

    private void OnEnable()
    {
        m_CurrentInputDevice.OnChanged += ChangeIcon;
    }

    private void OnDisable()
    {
        m_CurrentInputDevice.OnChanged -= ChangeIcon;
    }

    void ChangeIcon(InputDeviceType type)
    {
        States state = states.FirstOrDefault(c => c.type == type);
        if (state != null)
        {
            graphics.sprite = state.icon;
            graphics.color = state.color;
        }
    }
}
