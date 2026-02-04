using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;

public sealed class VirtualMouseHandler : MonoBehaviour
{
   [Header("Settings")]
   [SerializeField] private float m_CursorSpeed = 400f;
   
   [Header("References")]
   [SerializeField] private InputActionReference m_StickMovementIA;
   
   [Header("Input")]
   [SerializeField] private RSE_OnInputDeviceChanged m_OnInputDeviceChanged;
   
   private Mouse m_VirtualMouse;
   private Mouse m_CurrentMouse;
   private InputDeviceType m_CurrentDeviceType;

   private void OnEnable()
   {
      m_CurrentMouse = Mouse.current;
      HandleVirtualMouseConnexion();
      InputSystem.onAfterUpdate += UpdateMotion;
      m_OnInputDeviceChanged.Action += HandleChangeDevice;
   }

   private void OnDisable()
   {
      InputSystem.onAfterUpdate -= UpdateMotion;
   }
   
   private void HandleVirtualMouseConnexion()
   {
      InputDevice virtualMouseInDevice = InputSystem.GetDevice("VirtualMouse");

      if (virtualMouseInDevice is not { added: true })
      {
         m_VirtualMouse = (Mouse)InputSystem.AddDevice("VirtualMouse");
      }
      else
         m_VirtualMouse = (Mouse)virtualMouseInDevice;
      
      InputUser.PerformPairingWithDevice(m_VirtualMouse);
   }

   private void HandleChangeDevice(InputDeviceType deviceType)
   {
      
      switch (deviceType)
      {
         case InputDeviceType.Gamepad:
            InputState.Change(m_VirtualMouse.position, m_CurrentMouse.position.ReadValue());
            break;
         case InputDeviceType.KeyboardMouse:
            m_CurrentMouse.WarpCursorPosition(m_VirtualMouse.position.ReadValue());
            break;
      }
   }

   private void UpdateMotion()
   {
      Vector2 stickValue = m_StickMovementIA.action.ReadValue<Vector2>();
      Vector2 delta = stickValue * m_CursorSpeed * Time.unscaledDeltaTime;
      Vector2 newPosition = m_VirtualMouse.position.ReadValue() + delta;
      newPosition.x = Mathf.Clamp(newPosition.x, 0,Screen.width);
      newPosition.y = Mathf.Clamp(newPosition.y, 0,Screen.height);
      InputState.Change(m_VirtualMouse.position, newPosition);
      InputState.Change(m_VirtualMouse.delta, delta);
   }
}

