using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using MyBox;
using TMPro;
using CooldownAPI;

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
        public float windowToSpecial = 4f;

        //
        private Vector3 currentMovement;
        private Vector2 inputDirection;
        private bool isMovementPressed = false;

        private bool inSpecialPose = false;
        bool specialSubscribed;
        bool duringSpecial;

        private bool isAttacking = false;
        private bool inAttack = false;

        public float specialCooldownDuration = 1f;
        Cooldown specialCooldown;
        Coroutine specialCoroutine;

        private void Awake()
        {
            playerInput = new PlayerInputMap();
            InputAssigner();

            mainCam = Camera.main;
            controller = GetComponent<CharacterController>();
            animator = GetComponentInChildren<Animator>();

            specialCooldown = new Cooldown(specialCooldownDuration);
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

        private void Movement()
        {
            if (!inSpecialPose && !isAttacking && !inAttack)
            {
                controller.Move(currentMovement * Time.deltaTime * movementSpeed);
            }
        }
        private void Rotation()
        {
            Vector3 posToLookAt = new Vector3(currentMovement.x, 0, currentMovement.z);
            Quaternion currentRotation = transform.rotation;

            if (isMovementPressed && !inSpecialPose && !isAttacking)
            {
                Quaternion targetRotation = Quaternion.LookRotation(posToLookAt);
                transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);
            }
        }

        private void Attack()
        {
            if (isAttacking && !inAttack)
            {
                StartCoroutine(AttackSubroutine());
            }
        }
        private IEnumerator AttackSubroutine()
        {
            inAttack = true;
            Debug.Log("Attack once");
            yield return new WaitForSeconds(1f);
            inAttack = false;
        }

        private void Special()
        {
            if (inSpecialPose && !duringSpecial && !specialCooldown.IsActive)
            {
                specialCoroutine = StartCoroutine(SpecialPose());
            }
            else
            {
                DisplayText(" ");
            }
        }
        private IEnumerator SpecialPose()
        {
            float timer = 0;
            duringSpecial = true;

            playerInput.CharacterControls.Special.canceled += ExecuteSpecialAttack;
            specialSubscribed = true;
            while (timer < windowToSpecial)
            {
                DisplayText("Charging");

                Quaternion currentRotation = transform.rotation;
                Quaternion targetRotation = Quaternion.LookRotation(GetMousePositionInWorld());
                transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);
                timer += Time.deltaTime;
                yield return null;
            }

            if (specialSubscribed)
            {
                playerInput.CharacterControls.Special.canceled -= ExecuteSpecialAttack;
                specialSubscribed = false;
            }

            duringSpecial = false;
            DisplayText("");

            specialCooldown.Activate();
        }
        private void ExecuteSpecialAttack(InputAction.CallbackContext context)
        {
            StopCoroutine(specialCoroutine);
            DisplayText("");
            duringSpecial = false;

            Debug.LogError("Katchau");

            playerInput.CharacterControls.Special.canceled -= ExecuteSpecialAttack;
            specialSubscribed = false;

            specialCooldown.Activate();
        }

        private void moveAnimation()
        {
            if (isMovementPressed && !inSpecialPose && !inAttack)
            {
                animator.SetBool("isMoving", true);
            }
            else
            {
                animator.SetBool("isMoving", false);
            }

        }
        private void GravityHandler()
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
            currentMovement.x = inputDirection.x;
            currentMovement.z = inputDirection.y;

            currentMovement = Quaternion.Euler(0, mainCam.transform.eulerAngles.y, 0) * currentMovement;
            isMovementPressed = inputDirection.x != 0 || inputDirection.y != 0;
        }
        private void onAttackInput(InputAction.CallbackContext context)
        {
            isAttacking = context.ReadValueAsButton();
            if (isAttacking) Debug.Log("Attacking");
        }
        private void onSpecialInput(InputAction.CallbackContext context)
        {
            inSpecialPose = context.ReadValueAsButton();
        }

        private void DisplayText(string message)
        {
            if (debug_SpecialState != null)
            {
                debug_SpecialState.text = message;
            }
        }
        private Vector3 GetMousePositionInWorld()
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
