using UnityEngine;
using UnityEngine.InputSystem;
using MyBox;
namespace Nankink.Controller
{
    public class PlayerInputHandler : MonoBehaviour
    {
        [Header("Character Input Values")]
        [ReadOnly]public Vector2 move;
        [ReadOnly]public bool jump;
        [ReadOnly]public bool iai;
        [ReadOnly]public bool attack;

        [Header("Movement Settings")]
        public bool analogMovement;

        [Header("Mouse Cursor Settings")]
        public bool cursorLocked = true;
        public bool cursorInputForLook = false;

        public void OnMove(InputValue value)
        {
            MoveInput(value.Get<Vector2>());
        }
        public void OnJump(InputValue value)
        {
            JumpInput(value.isPressed);
        }
        public void OnIai(InputValue value)
        {
            IaiInput(value.isPressed);
        }
        public void OnAttack(InputValue value)
        {
            AttackInput(value.isPressed);
        }

        public void MoveInput(Vector2 newMoveDirection)
        {
            move = newMoveDirection;
        }
        public void JumpInput(bool newJumpState)
        {
            jump = newJumpState;
        }
        public void IaiInput(bool newActionState)
        {
            iai = newActionState;
        }
        public void AttackInput(bool newAttackState)
        {
            attack = newAttackState;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            SetCursorState(cursorLocked);
        }
        private void SetCursorState(bool newState)
        {
            Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }
}
