using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using MyBox;
using TMPro;
using CooldownAPI;

namespace Nankink.Controller
{
    [RequireComponent(typeof(PlayerInputHandler))]
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        // References
        private PlayerInputHandler inputHandler;
        private CharacterController controller;
        private Animator animator;
        private Camera mainCam;

        // Debug
        public TextMeshProUGUI debug_SpecialState;

        // Parameters
        public float movementSpeed = 15f;
        public float rotationFactorPerFrame = 20f;
        public LayerMask groundLayer;
        public float windowToSpecial = 4f;
        public float dashSpeed = 30f;
        public float timeInDash = 0.2f;

        // Properties
        bool MovingInput => inputHandler.MovementPressed;
        bool AttackingInput => inputHandler.Attacking;
        bool SpecialMoveInput => inputHandler.SpecialMove;

        //
        private Vector3 currentMovement;

        public Collider specialCollider;
        bool specialSubscribed;
        bool duringSpecial;

        bool canAttack;

        public float specialCooldownDuration = 1f;
        Cooldown specialCooldown;
        Coroutine specialCoroutine;

        private void Awake()
        {
            mainCam = Camera.main;
            controller = GetComponent<CharacterController>();
            controller.detectCollisions = false;

            inputHandler = GetComponent<PlayerInputHandler>();
            inputHandler.InitializeInputHandler();
            inputHandler.InputMap.Attack.canceled += CanAttack;

            animator = GetComponentInChildren<Animator>();

            specialCooldown = new Cooldown(specialCooldownDuration);
        }
        private void OnEnable()
        {
            inputHandler.EnableControls(true);
        }
        private void OnDisable()
        {
            inputHandler.EnableControls(false);
        }
        private void Update()
        {
            ConvertMovementInput();

            Movement();

            Rotation();

            Attack();
            Special();

            GravityHandler();
        }

        #region Movement
        private void Movement()
        {
            if (VerifyIfAbleToMove() && MovingInput)
            {
                animator.SetBool("isMoving", true);
                controller.Move(currentMovement * Time.deltaTime * movementSpeed);
            }
            else
            {
                animator.SetBool("isMoving", false);
            }
        }
        private void Rotation()
        {
            if (MovingInput && VerifyIfAbleToMove())
            {
                RotatePlayerTo(false, target: currentMovement);
            }
        }
        #endregion

        private void Attack()
        {
            if (AttackingInput && canAttack)
            {
                StartCoroutine(AttackSubroutine());
            }
        }
        private IEnumerator AttackSubroutine()
        {
            Debug.Log("Attacked");
            canAttack = false;

            animator.SetBool("isAttacking", true);
            yield return new WaitForSeconds(0.2f);
            animator.SetBool("isAttacking", false);
        }
        internal void CanAttack(InputAction.CallbackContext context)
        {
            canAttack = true;
        }

        private void Special()
        {
            if (SpecialMoveInput && !duringSpecial && !specialCooldown.IsActive)
            {
                specialCoroutine = StartCoroutine(SpecialPose());
            }
            else if (specialCooldown.IsActive)
            {
                DisplayText("Cooldown"); // Debug
            }
            else
            {
                DisplayText(" "); // Debug
            }
        }
        private IEnumerator SpecialPose()
        {
            Debug.Log("Entered special");

            float timer = 0;
            duringSpecial = true;

            inputHandler.InputMap.Special.canceled += ExecuteSpecialAttack;
            specialSubscribed = true;
            while (timer < windowToSpecial)
            {
                DisplayText("Charging"); // Debug
                RotatePlayerTo(true);

                timer += Time.deltaTime;
                yield return null;
            }

            if (specialSubscribed)
            {
                inputHandler.InputMap.Special.canceled -= ExecuteSpecialAttack;
                specialSubscribed = false;
            }

            DisplayText("");

            specialCooldown.Activate();
        }
        private void ExecuteSpecialAttack(InputAction.CallbackContext context)
        {
            StopCoroutine(specialCoroutine);

            Debug.Log("Released special button");
            DisplayText(""); // Debug

            StartCoroutine(Dash());
        }
        IEnumerator Dash()
        {
            specialCollider.isTrigger = true;
            inputHandler.InputMap.Special.canceled -= ExecuteSpecialAttack;
            specialSubscribed = false;

            float timer = 0;
            while (timer < timeInDash)
            {
                controller.Move(transform.forward * dashSpeed * Time.deltaTime);
                yield return null;
                timer += Time.deltaTime;
            }
            specialCooldown.Activate();
            duringSpecial = false;
            specialCollider.isTrigger = false;
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
        private void GravityHandler()
        {
            if (controller.isGrounded)
            {
                currentMovement.y = -.05f;
            }
            else
            {
                currentMovement.y += -0.4f;
            }
        }
        private void RotatePlayerTo(bool useMouse, Vector3? target = null)
        {
            Vector3 posToLookAt = Vector3.zero;
            if (target != null)
            {
                posToLookAt = new Vector3(target.Value.x, 0, target.Value.z);
            }
            Quaternion currentRot = transform.rotation;
            Quaternion targetRot = useMouse ? Quaternion.LookRotation(GetMousePositionInWorld()) : Quaternion.LookRotation(posToLookAt);
            transform.rotation = Quaternion.Slerp(currentRot, targetRot, rotationFactorPerFrame * Time.deltaTime);


        }
        bool VerifyIfAbleToMove()
        {
            return (!SpecialMoveInput && !AttackingInput && !canAttack && !duringSpecial);
        }
        private void ConvertMovementInput()
        {
            Vector3 mov = new Vector3(inputHandler.InputDirection.x, 0, inputHandler.InputDirection.y);
            currentMovement = Quaternion.Euler(0, mainCam.transform.eulerAngles.y, 0) * mov;
        }
    }
}
