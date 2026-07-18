using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerInput
{
    public class InputManager : MonoBehaviour, Controls.IFootActions
    {
        // Instance
        public static InputManager Instance { get; private set; }

        // Input Properties
        public Vector2 MoveInput { get; private set; }
        public Vector2 LookInput { get; private set; }
        public bool IsSprinting { get; private set; }
        public bool IsCrouching { get; private set; }
        public bool IsSliding { get; private set; }

        // Events
        public event Action OnJumpPressed;

        private Controls _controls;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            _controls = new Controls();
            _controls.Foot.SetCallbacks(this);
            ToggleInput(true);
            ToggleCursor(false);
        }

        public void ToggleInput(bool isEnabled)
        {
            if (isEnabled)
            {
                _controls.Enable();
            }
            else
            {
                _controls.Disable();
            }
        }

        public void ToggleCursor(bool isVisible)
        {
            Cursor.visible = isVisible;
            Cursor.lockState = isVisible ? CursorLockMode.None : CursorLockMode.Locked;
        }

        public void OnMovement(InputAction.CallbackContext context)
        {
            MoveInput = context.ReadValue<Vector2>();
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if(context.performed)
            {
                OnJumpPressed?.Invoke();
            }
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            LookInput = context.ReadValue<Vector2>();
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
            IsSprinting = context.ReadValueAsButton();
        }

        public void OnCrouch(InputAction.CallbackContext context)
        {
            IsCrouching = context.ReadValueAsButton();
        }

        public void OnSlide(InputAction.CallbackContext context)
        {
            IsSliding = context.ReadValueAsButton();
        }
    }
}
