using UnityEngine;
using Unity.Netcode;
using VS.Utilities.Input;

namespace VS.NetcodeExampleProject.Player {
    [RequireComponent(typeof(CharacterController))]
    public class CharacterMovementController : NetworkBehaviour {
        [Header("References")]
        [SerializeField] private InputReader inputReader;
        [SerializeField] private Transform cameraTransform;
        
        [Header("Movement Settings")]
        [SerializeField] private float walkSpeed = 5f;
        [SerializeField] private float sprintSpeed = 8f;
        [SerializeField] private float acceleration = 10f;
        [SerializeField] private float deceleration = 10f;
        
        [Header("Jump Settings")]
        [SerializeField] private float jumpHeight = 2f;
        [SerializeField] private float gravity = 15f;
        
        [Header("Ground Check")]
        [SerializeField] private float groundCheckDistance = 0.2f;
        [SerializeField] private LayerMask groundMask;
        
        private CharacterController _controller;
        private Vector2 _moveInput;
        private Vector3 _velocity;
        private Vector3 _currentVelocity;
        private bool _isSprinting;
        private bool _jumpPressed;
        private bool _isGrounded;
        
        private void Awake() {
            _controller = GetComponent<CharacterController>();
        }
        
        public override void OnNetworkSpawn() {
            base.OnNetworkSpawn();

            if (!IsOwner) {
                enabled = false;
                return;
            }
            
            inputReader.OnMoveEvent += HandleMove;
            inputReader.OnSprintEvent += HandleSprint;
            inputReader.OnJumpEvent += HandleJump;
            inputReader.EnablePlayerActions();
        }
        
        public override void OnNetworkDespawn() {
            base.OnNetworkDespawn();

            if (!IsOwner) {
                return;
            }
            
            inputReader.OnMoveEvent -= HandleMove;
            inputReader.OnSprintEvent -= HandleSprint;
            inputReader.OnJumpEvent -= HandleJump;
            inputReader.DisablePlayerActions();
        }
        
        private void Update() {
            GroundCheck();
            ApplyGravity();
            HandleMovement();
            HandleJumping();
        }

        #region MOVEMENT METHODS (UNIMPORTANT)

        private void GroundCheck() {
            _isGrounded = Physics.CheckSphere(
                transform.position, 
                groundCheckDistance, 
                groundMask
            );
            
            if (_isGrounded && _velocity.y < 0) {
                _velocity.y = -2f;
            }
        }
        
        private void ApplyGravity() {
            _velocity.y -= gravity * Time.deltaTime;
        }
        
        private void HandleMovement() {
            Vector3 moveDirection = GetMoveDirection();
            float targetSpeed = _isSprinting ? sprintSpeed : walkSpeed;
            Vector3 targetVelocity = moveDirection * targetSpeed;
            
            float lerpSpeed = _moveInput.magnitude > 0 ? acceleration : deceleration;
            _currentVelocity = Vector3.Lerp(
                _currentVelocity, 
                targetVelocity, 
                lerpSpeed * Time.deltaTime
            );
            
            Vector3 movement = _currentVelocity + Vector3.up * _velocity.y;
            _controller.Move(movement * Time.deltaTime);
        }
        
        private void HandleJumping() {
            if (_jumpPressed && _isGrounded) {
                _velocity.y = Mathf.Sqrt(2f * gravity * jumpHeight);    
                _jumpPressed = false;
            }
        }
        
        private Vector3 GetMoveDirection() {
            if (cameraTransform == null) {
                return new Vector3(_moveInput.x, 0, _moveInput.y);
            }
            
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;
            
            forward.y = 0;
            right.y = 0;
            forward.Normalize();
            right.Normalize();
            
            return (forward * _moveInput.y + right * _moveInput.x).normalized;
        }
        
        private void HandleMove(Vector2 input) {
            _moveInput = input;
        }
        
        private void HandleSprint(bool sprinting) {
            _isSprinting = sprinting;
        }
        
        private void HandleJump(bool pressed) {
            if (pressed) {
                _jumpPressed = true;
            }
        }

        #endregion
        
        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, groundCheckDistance);
        }
    }
}