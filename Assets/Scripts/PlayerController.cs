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

    [Header("Enable/Disable (by Input)")]
    [Tooltip("GameObjects toggled by Jump input")]
    [SerializeField] private GameObject[] enemiesToEnableDisable;
    
    [Tooltip("GameObject toggled by Crouch input")]
    [SerializeField] private GameObject hintToEnableDisable;

    private Rigidbody _rb;
    private float _inputX;
    private float _inputZ;

    private InputAction _moveAction;
    private InputAction _jumpAction;   
    private InputAction _crouchAction; 
    private InputAction _interactAction;

    [Header("Shield reference")]
    public EnergyShieldController shield;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        var playerInput = GetComponent<PlayerInput>();
        _moveAction = playerInput.actions["Move"];     
        _jumpAction = playerInput.actions["Jump"];     
        _crouchAction = playerInput.actions["Crouch"];  
        _interactAction = playerInput.actions["Interact"];
        
        _rb.constraints = RigidbodyConstraints.FreezeRotationX |
                          RigidbodyConstraints.FreezeRotationY |
                          RigidbodyConstraints.FreezeRotationZ;
    }

    private void Update()
    {
        Vector2 moveInput = _moveAction.ReadValue<Vector2>();
        _inputX = moveInput.x;
        _inputZ = moveInput.y;
        
        if (_jumpAction.triggered)
        {
            if (enemiesToEnableDisable != null)
            {
                for (int i = 0; i < enemiesToEnableDisable.Length; i++)
                {
                    Toggle(enemiesToEnableDisable[i]);
                }
            }
        }
        
        if (_crouchAction != null && _crouchAction.triggered)
        {
            Toggle(hintToEnableDisable);
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
        var vel = _rb.linearVelocity;
        vel.x = _inputX * moveSpeed;
        vel.z = _inputZ * moveSpeedZ;
        _rb.linearVelocity = vel;
    }

    private static void Toggle(GameObject go)
    {
        if (go == null) return;
        go.SetActive(!go.activeSelf);
    }
}