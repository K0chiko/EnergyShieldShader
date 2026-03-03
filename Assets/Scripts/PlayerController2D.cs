using UnityEngine;
using UnityEngine.InputSystem; 

namespace SideViewShooter
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(PlayerInput))] 
    public class PlayerController2D : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 6f;
        [SerializeField] private float jumpForce = 7.5f;
        [SerializeField] private bool faceRightByDefault = true;

        [Header("Ground Check")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundCheckRadius = 0.15f;
        [SerializeField] private LayerMask groundLayer = ~0;

        private Rigidbody _rb;
        private bool _isGrounded;
        private float _inputX;
        private bool _facingRight;
        
        private InputAction _moveAction;
        private InputAction _jumpAction;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            
            var playerInput = GetComponent<PlayerInput>();
            _moveAction = playerInput.actions["Move"]; 
            _jumpAction = playerInput.actions["Jump"]; 

            _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY |
                              RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionZ;

            _facingRight = faceRightByDefault;
        }

        private void Update()
        {
            Vector2 moveInput = _moveAction.ReadValue<Vector2>();
            _inputX = moveInput.x;
            
            if (_jumpAction.triggered && _isGrounded)
            {
                Jump();
            }

            if (_inputX > 0.01f && !_facingRight)
            {
                Flip(true);
            }
            else if (_inputX < -0.01f && _facingRight)
            {
                Flip(false);
            }
        }

        private void FixedUpdate()
        {
            if (groundCheck != null)
            {
                _isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer, QueryTriggerInteraction.Ignore);
            }


            var vel = _rb.linearVelocity; 
            vel.x = _inputX * moveSpeed;
            _rb.linearVelocity = vel;
        }

        private void Jump()
        {
            var vel = _rb.linearVelocity;
            vel.y = 0f; 
            _rb.linearVelocity = vel;
            _rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
        }

        private void Flip(bool toRight)
        {
            _facingRight = toRight;
            var scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (toRight ? 1f : -1f);
            transform.localScale = scale;
        }

        private void OnDrawGizmosSelected()
        {
            if (groundCheck != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
            }
        }
    }
}