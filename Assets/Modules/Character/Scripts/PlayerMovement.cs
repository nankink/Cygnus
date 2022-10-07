using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using MyBox;
using TMPro;

namespace Nankink.Controller
{
    public class PlayerMovement : MonoBehaviour
    {
        // References
        private PlayerInputMap playerInput;
        private CharacterController controller;
        private Animator animator;
        private Camera mainCam;

        // Debug
        public TextMeshProUGUI debug_SpecialState;

        // Properties
        public float movementSpeed = 15f;
        public float rotationFactorPerFrame = 20f;
        public LayerMask groundLayer;

        //
        private Vector3 currentMovement;
        private Vector2 inputDirection;
        private bool isMovementPressed = false;

        private bool inSpecialPose = false;
        private bool isAttacking = false;
        private bool inAttack = false;


        #region MONOBEHAVIOUR METHODS
        private void Awake()
        {
            playerInput = new PlayerInputMap();
            InputAssigner();

            mainCam = Camera.main;
            controller = GetComponent<CharacterController>();
            animator = GetComponentInChildren<Animator>();
        }
        private void Start()
        {
            if (playerInput == null)
            {
                Debug.Log("Player input is null");
                playerInput = new PlayerInputMap();
                InputAssigner();
            }
        }

        private void OnEnable()
        {
            playerInput.Enable();
        }
        private void OnDisable()
        {
            playerInput.Disable();
        }

        private void Update()
        {
            Movement();
            moveAnimation();

            Rotation();

            Attack();
            Special();

            GravityHandler();
        }
        #endregion 

        void Movement()
        {
            if (!inSpecialPose && !isAttacking)
            {
                controller.Move(currentMovement * Time.deltaTime * movementSpeed);
            }
        }
        void Rotation()
        {
            Vector3 posToLookAt = new Vector3(currentMovement.x, 0, currentMovement.z);
            Quaternion currentRotation = transform.rotation;

            if (isMovementPressed && !inSpecialPose && !isAttacking)
            {
                Quaternion targetRotation = Quaternion.LookRotation(posToLookAt);
                transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);
            }
        }
        void Attack()
        {
            if(isAttacking && !inAttack)
            {
                StartCoroutine(AttackSubroutine());
            }
        }
        IEnumerator AttackSubroutine()
        {
            inAttack = true;
            Debug.Log("Attack once");
            yield return new WaitForSeconds(1f);
            inAttack = false;
        }

        void Special()
        {
            if (inSpecialPose)
            {
                DisplayText("Iai pose");

                Quaternion currentRotation = transform.rotation;
                Quaternion targetRotation = Quaternion.LookRotation(GetMousePositionInWorld());
                transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);
            }
            else
            {
                DisplayText(" ");
            }
        }

        void moveAnimation()
        {
            if (isMovementPressed && !inSpecialPose)
            {
                animator.SetBool("isMoving", true);
            }
            else
            {
                animator.SetBool("isMoving", false);
            }

        }
        void GravityHandler()
        {
            if (controller.isGrounded)
            {
                currentMovement.y = -.05f;
            }
            else
            {
                currentMovement.y += -9.8f;
            }
        }

        #region Inputs
        void InputAssigner()
        {
            playerInput.CharacterControls.Move.started += onMoveInput;
            playerInput.CharacterControls.Move.performed += onMoveInput;
            playerInput.CharacterControls.Move.canceled += onMoveInput;

            playerInput.CharacterControls.Attack.started += onAttackInput;
            playerInput.CharacterControls.Attack.canceled += onAttackInput;

            playerInput.CharacterControls.Special.started += onSpecialInput;
            playerInput.CharacterControls.Special.canceled += onSpecialInput;
        }
        void onMoveInput(InputAction.CallbackContext context)
        {
            inputDirection = context.ReadValue<Vector2>();
            currentMovement.x = inputDirection.x;
            currentMovement.z = inputDirection.y;

            currentMovement = Quaternion.Euler(0, mainCam.transform.eulerAngles.y, 0) * currentMovement;
            isMovementPressed = inputDirection.x != 0 || inputDirection.y != 0;
        }
        void onAttackInput(InputAction.CallbackContext context)
        {
            isAttacking = context.ReadValueAsButton();
            if (isAttacking) Debug.Log("Attacking");
        }
        void onSpecialInput(InputAction.CallbackContext context)
        {
            inSpecialPose = context.ReadValueAsButton();
        }
        #endregion

        void DisplayText(string message)
        {
            if (debug_SpecialState != null)
            {
                debug_SpecialState.text = message;
            }
        }
        Vector3 GetMousePositionInWorld()
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, groundLayer))
            {
                Vector3 direction = (hit.point - transform.position).normalized;
                return direction;
            }
            else return Vector3.zero;
        }

    }
}
