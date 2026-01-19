using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCursor : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float m_RotationSpeed;
    
    [Header("References")]
    [SerializeField] Transform m_CursorImg;

    [Header("Input")]
    [SerializeField] InputActionReference m_MousePositionIA;

    //[Header("Output")]

    private void Start()
    {
        Cursor.visible = false;
    }

    private void Update()
    {
        transform.position = m_MousePositionIA.action.ReadValue<Vector2>();
        m_CursorImg.Rotate(Vector3.forward * m_RotationSpeed * Time.deltaTime);
    }
}