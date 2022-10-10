using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using MyBox;

namespace Nankink.Controller
{
    public class PlayerInputHandler : MonoBehaviour
    {
        internal PlayerInputMap PlayerInput { get { return playerInput; } }
        private PlayerInputMap playerInput;

        internal PlayerInputMap.CharacterControlsActions InputMap { get { return inputMap; } }
        private PlayerInputMap.CharacterControlsActions inputMap;

        internal Vector2 InputDirection { get { return inputDirection; } }
        private Vector2 inputDirection;

        bool isMovementPressed = false;
        public bool MovementPressed { get { return isMovementPressed; } }
        bool isAttacking = false;
        public bool Attacking { get { return isAttacking; } }
        bool isSpecialMove = false;
        public bool SpecialMove { get { return isSpecialMove; } }

        public void InitializeInputHandler()
        {
            playerInput = new PlayerInputMap();
            inputMap = playerInput.CharacterControls;
            InputAssigner();
        }

        public void EnableControls(bool isEnable)
        {
            if (isEnable)
                playerInput.Enable();
            else playerInput.Disable();
        }

        private void InputAssigner()
        {
            playerInput.CharacterControls.Move.started += onMoveInput;
            playerInput.CharacterControls.Move.performed += onMoveInput;
            playerInput.CharacterControls.Move.canceled += onMoveInput;

            playerInput.CharacterControls.Attack.started += onAttackInput;
            playerInput.CharacterControls.Attack.canceled += onAttackInput;

            playerInput.CharacterControls.Special.started += onSpecialInput;
            playerInput.CharacterControls.Special.canceled += onSpecialInput;
        }
        private void onMoveInput(InputAction.CallbackContext context)
        {
            inputDirection = context.ReadValue<Vector2>();
            isMovementPressed = inputDirection.x != 0 || inputDirection.y != 0;
        }
        private void onAttackInput(InputAction.CallbackContext context)
        {
            isAttacking = context.ReadValueAsButton();
        }
        private void onSpecialInput(InputAction.CallbackContext context)
        {
            isSpecialMove = context.ReadValueAsButton();
        }

        // ** THESE DOESNT SEEM TO WORK!
        public void SubscribeToAction(Action<InputAction.CallbackContext> inputAction, Action<InputAction.CallbackContext> method)
        {
            inputAction += method;
        }
        public void UnsubscribeToAction(Action<InputAction.CallbackContext> inputAction, Action<InputAction.CallbackContext> method)
        {
            inputAction -= method;
        }
    }
}
