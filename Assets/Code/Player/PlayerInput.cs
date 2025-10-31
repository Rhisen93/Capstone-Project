using UnityEngine;
using UnityEngine.InputSystem;

namespace ForestSlice.Player
{
    [RequireComponent(typeof(PlayerController))]
    public class PlayerInput : MonoBehaviour
    {
        private PlayerController controller;
        private PlayerInputActions inputActions;

        private void Awake()
        {
            controller = GetComponent<PlayerController>();
            inputActions = new PlayerInputActions();
        }

        private void OnEnable()
        {
            // Ensure inputActions is initialized
            if (inputActions == null)
            {
                inputActions = new PlayerInputActions();
            }

            inputActions.Enable();

            inputActions.Gameplay.Move.performed += OnMovePerformed;
            inputActions.Gameplay.Move.canceled += OnMoveCanceled;
            
            inputActions.Gameplay.Dash.performed += OnDashPerformed;
            inputActions.Gameplay.Attack.performed += OnAttackPerformed;
            
            inputActions.Gameplay.Ability1.performed += ctx => controller.OnAbilityInput(1);
            inputActions.Gameplay.Ability2.performed += ctx => controller.OnAbilityInput(2);
            inputActions.Gameplay.Ability3.performed += ctx => controller.OnAbilityInput(3);
            inputActions.Gameplay.Ability4.performed += ctx => controller.OnAbilityInput(4);
        }

        private void OnDisable()
        {
            if (inputActions == null) return;

            inputActions.Gameplay.Move.performed -= OnMovePerformed;
            inputActions.Gameplay.Move.canceled -= OnMoveCanceled;
            
            inputActions.Gameplay.Dash.performed -= OnDashPerformed;
            inputActions.Gameplay.Attack.performed -= OnAttackPerformed;

            inputActions.Disable();
        }

        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            Vector2 input = context.ReadValue<Vector2>();
            controller.SetMoveInput(input);
        }

        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            controller.SetMoveInput(Vector2.zero);
        }

        private void OnDashPerformed(InputAction.CallbackContext context)
        {
            controller.OnDashInput();
        }

        private void OnAttackPerformed(InputAction.CallbackContext context)
        {
            controller.OnAttackInput();
        }
    }
}
