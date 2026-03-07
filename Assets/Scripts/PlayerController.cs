using EnergyShield;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerInput))] 
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float moveSpeedZ = 4f; 
    [SerializeField] private float rotationSpeed = 10f; 
    [SerializeField] private float jumpForce = 7.5f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer = ~0;

    private Rigidbody _rb;
    private bool _isGrounded;
    private float _inputX;
    private float _inputZ;
        
    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _interactAction;

    [Header("Shield reference")] 
    public EnergyShieldController shield;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
            
        var playerInput = GetComponent<PlayerInput>();
        _moveAction = playerInput.actions["Move"]; 
        _jumpAction = playerInput.actions["Jump"]; 
        _interactAction = playerInput.actions["Interact"]; 
            
        _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY |
                          RigidbodyConstraints.FreezeRotationZ;
    }

    private void Update()
    {
        Vector2 moveInput = _moveAction.ReadValue<Vector2>();
        _inputX = moveInput.x;
        _inputZ = moveInput.y;
            
        if (_jumpAction.triggered && _isGrounded)
        {
            Jump();
        }
            
        ApplyRotation();

        if (_interactAction.triggered && shield != null)
        {
            shield.Play(); 
        }
    }

    private void ApplyRotation()
    {
        Vector3 moveDirection = new Vector3(_inputX * moveSpeed, 0f, _inputZ * moveSpeedZ);
            
        if (moveDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
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
        vel.z = _inputZ * moveSpeedZ; 
        _rb.linearVelocity = vel;
    }

    private void Jump()
    {
        var vel = _rb.linearVelocity;
        vel.y = 0f; 
        _rb.linearVelocity = vel;
        _rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
    }
        
}