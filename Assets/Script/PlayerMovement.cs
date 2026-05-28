using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;

public class PlayerMovement : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference movement;
    [SerializeField] private InputActionReference rotate;
    [SerializeField] private InputActionReference jump;
    [SerializeField] private InputActionReference interact;
    
    [Header("Movement")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float moveSpeed = 100f;
    [SerializeField] private float rotateSpeed = 20f;
    [SerializeField] private float smoothRotate = 5f;
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private Camera playerCamera;
    
    private Vector2 _movement;
    private Vector2 _rotate;
    private bool _jump;
    private bool _canJump;
    
    private CharacterController _controller;
    
    void Start()
    {
        _controller = GetComponent<CharacterController>();
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    
    private void OnEnable()
    {
        movement.action.Enable();
        rotate.action.Enable();
        jump.action.Enable();
        interact.action.Enable();
        
        movement.action.performed += OnMove;
        rotate.action.performed += OnRotate;
        jump.action.performed += OnJump;
        interact.action.performed += OnInteract;
        
        movement.action.canceled += OnMove;
        rotate.action.canceled += OnRotate;
        jump.action.canceled += OnJump;
        interact.action.canceled += OnInteract;
    }
    
    private void OnDisable()
    {
        movement.action.performed -= OnMove;
        rotate.action.performed -= OnRotate;
        jump.action.performed -= OnJump;
        interact.action.performed -= OnInteract;
        
        movement.action.canceled -= OnMove;
        rotate.action.canceled -= OnRotate;
        jump.action.canceled -= OnJump;
        interact.action.performed -= OnInteract;
        
        movement.action.Disable();
        rotate.action.Disable();
        jump.action.Disable();
        interact.action.Disable();
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        _movement = context.ReadValue<Vector2>();
    }
    public void OnRotate(InputAction.CallbackContext context)
    {
        _rotate = context.ReadValue<Vector2>();
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _jump = true;
        }
    }
    RaycastHit _hit;
    public void OnInteract(InputAction.CallbackContext context)
    {
        if(HudManager.Instance.IsTarget) return;
        if (_hit.collider !=null && _hit.collider.CompareTag("Puzzle") && _hit.collider.GetComponent<PuzzleManager>() != null)
        {
            HudManager.Instance.OpenPuzzle(_hit.collider.GetComponent<PuzzleManager>().puzzleType);
            _hit.collider.GetComponent<PuzzleManager>().Initialize();
        }
    }


    private float _currentXRotation;
    private float _currentYRotation;
    
    private float _xRotation = 0f;
    private float _yRotation = 0f;
    
    private float _gravity = -20f;

    private Vector3 velocity;
    
    void Update()
    {
        Move();
        Rotate();
        HandleGravity();
        HandleJump();
        HandleInteract();
    }
    

    private void Move()
    {
        if (HudManager.Instance.IsTarget) return;
        
        Vector3 move = transform.right * _movement.x + transform.forward * _movement.y;
        _controller.Move(move * (moveSpeed * Time.fixedDeltaTime));
    }

    private void Rotate()
    {
        if (!HudManager.Instance.IsTarget)
        {
            float mouseX = _rotate.x * rotateSpeed * Time.deltaTime;
            float mouseY = _rotate.y * rotateSpeed * Time.deltaTime;

            _yRotation += mouseX;
            
            _xRotation -= mouseY;

        }
        _xRotation = Mathf.Clamp(_xRotation, -85f, 85f);
        
        _currentYRotation = Mathf.Lerp(_currentYRotation, _yRotation, smoothRotate * Time.deltaTime);
        _currentXRotation = Mathf.Lerp(_currentXRotation, _xRotation, smoothRotate * Time.deltaTime);
        
        transform.rotation = Quaternion.Euler(0f, _currentYRotation, 0f);
        cameraPivot.localRotation = Quaternion.Euler(_currentXRotation, 0f, 0f);
    }
    
    private void HandleGravity()
    {
        if (_controller.isGrounded && velocity.y < 0) velocity.y = -2f;
        
        velocity.y += _gravity * Time.deltaTime;

        _controller.Move(velocity * Time.deltaTime);
    }
    
    private void HandleJump()
    {
        if (!_jump) return;

        if (_controller.isGrounded) velocity.y = Mathf.Sqrt(jumpForce * -2f * _gravity);
        
        _jump = false;
    }
    
    
    private void HandleInteract()
    {
        if(HudManager.Instance.IsTarget) return;

        HudManager.Instance.OnTarget(Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out _hit, 5f) && _hit.collider.CompareTag("Puzzle"));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(playerCamera.transform.position, playerCamera.transform.forward*5);
    }
}
