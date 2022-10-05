using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections;
using MyBox;

namespace Nankink.Controller
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerInputHandler))]
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerController : MonoBehaviour
    {
        #region VARIABLES
        [Header("Movement")]
        public float MovementSpeed = 2f;
        public float SpeedChangeRate = 10f;
        [Range(0f, 0.3f)] public float RotationSmoothTime = 0.12f;

        [Space(10)]
        [Header("Jump & Gravity")]
        public float JumpHeight = 1.2f;
        public float JumpTimeout = 0.5f;
        public float FallTimeout = 0.15f;
        public float Gravity = -15f;

        [Space(10)]
        [Header("Ground")]
        [ReadOnly] public bool Grounded;
        public float GroundedOffset = -0.14f;
        public float GroundedRadius = 0.28f;
        public LayerMask GroundLayers;

        // player 
        private float m_speed;
        private float m_targetRotation;
        private float m_rotationVelocity;
        private float m_verticalVelocity;
        private float m_terminalVelocity = 53f;

        // timeout deltatime
        private float m_jumpTimeoutDelta;
        private float m_fallTimeoutDelta;
        private float m_attackTimeoutDelta = 0f;

        // References
        CharacterController controller;
        public Animator animator;
        PlayerInput playerInput;
        PlayerInputHandler inputHandler;
        Rigidbody rb;
        Camera cam;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return playerInput.currentControlScheme == "KeyboardMouse";
#else
                return false;
#endif
            }
        }
        #endregion

        #region MONOBEHAVIOUR METHODS
        void Start()
        {
            controller = GetComponent<CharacterController>();
            if (animator != null) animator = GetComponentInChildren<Animator>();
            rb = GetComponent<Rigidbody>();
            playerInput = GetComponent<PlayerInput>();
            inputHandler = GetComponent<PlayerInputHandler>();
            cam = Camera.main;

            m_jumpTimeoutDelta = JumpTimeout;
            m_fallTimeoutDelta = FallTimeout;
        }

        void Update()
        {
            JumpAndGravity();
            GroundCheck();
            Move();
        }
        #endregion

        void Move()
        {

            float targetSpeed = MovementSpeed;
            if (inputHandler.move == Vector2.zero) targetSpeed = 0f;

            float currentHorizontalSpeed = new Vector3(controller.velocity.x, 0f, controller.velocity.z).magnitude;
            float speedOffset = 0.1f;
            float inputMagnitude = inputHandler.analogMovement ? inputHandler.move.magnitude : 1f;

            if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                m_speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);
                m_speed = Mathf.Round(m_speed * 1000f) / 1000f;
            }
            else
            {
                m_speed = targetSpeed;
            }

            Vector3 inputDirection = new Vector3(inputHandler.move.x, 0f, inputHandler.move.y).normalized;
            animator.SetFloat("Movement", inputDirection.magnitude);

            // Rotation
            if (inputHandler.move != Vector2.zero)
            {
                m_targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, m_targetRotation, ref m_rotationVelocity, RotationSmoothTime);

                transform.rotation = Quaternion.Euler(transform.rotation.x, rotation, transform.rotation.z);
            }

            // Set vector movement based on player's rotation
            Vector3 targetDirection = Quaternion.Euler(0f, m_targetRotation, 0f) * Vector3.forward;

            // Move player
            controller.Move(targetDirection.normalized * (m_speed * Time.deltaTime) + new Vector3(0f, m_verticalVelocity, 0f) * Time.deltaTime);
        }

        void JumpAndGravity()
        {
            if (Grounded)
            {
                m_fallTimeoutDelta = FallTimeout;

                if (m_verticalVelocity < 0f) m_verticalVelocity = -2f;

                if (inputHandler.jump && m_jumpTimeoutDelta <= 0f)
                {
                    m_verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                }
                if (m_jumpTimeoutDelta >= 0f)
                {
                    m_jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                m_jumpTimeoutDelta = JumpTimeout;
                if (m_jumpTimeoutDelta >= 0f)
                {
                    m_jumpTimeoutDelta -= Time.deltaTime;
                }

                inputHandler.jump = false;
            }

            if (m_verticalVelocity < m_terminalVelocity)
            {
                m_verticalVelocity += Gravity * Time.deltaTime;
            }
        }
        void GroundCheck()
        {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
        }
    }
}
