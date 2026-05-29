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
    [SerializeField] private InputActionReference crouch;
    
    [Header("Movement")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float moveSpeed = 100f;
    [SerializeField] private float rotateSpeed = 20f;
    [SerializeField] private float smoothRotate = 5f;
    
    [Header("Visual")]
    [SerializeField] private Transform eyes;
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Animator animator;
    
    
    private Vector2 _movement;
    private Vector2 _rotate;
    private bool _jump;
    private bool _canJump;
    
    private float actualSpeed;
    private float verticalSpeed;
    
    private CharacterController _controller;
    
    private bool isCrouching = false;
    
    
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
        crouch.action.Enable();
        
        movement.action.performed += OnMove;
        rotate.action.performed += OnRotate;
        jump.action.performed += OnJump;
        interact.action.performed += OnInteract;
        crouch.action.performed += OnCrouch;
        
        movement.action.canceled += OnMove;
        rotate.action.canceled += OnRotate;
        jump.action.canceled += OnJump;
        interact.action.canceled += OnInteract;
        crouch.action.canceled += OnCrouch;
    }
    
    private void OnDisable()
    {
        movement.action.performed -= OnMove;
        rotate.action.performed -= OnRotate;
        jump.action.performed -= OnJump;
        interact.action.performed -= OnInteract;
        crouch.action.performed -= OnCrouch;
        
        movement.action.canceled -= OnMove;
        rotate.action.canceled -= OnRotate;
        jump.action.canceled -= OnJump;
        interact.action.canceled -= OnInteract;
        crouch.action.canceled -= OnCrouch;
        
        movement.action.Disable();
        rotate.action.Disable();
        jump.action.Disable();
        interact.action.Disable();
        crouch.action.Disable();
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
        
        animator.SetTrigger("Interact");
        
        if (_hit.collider !=null && _hit.collider.CompareTag("Puzzle") && _hit.collider.GetComponent<PuzzleManager>() != null)
        {
            if (_hit.collider.GetComponent<PuzzleManager>().solved)
            {
                print("solved");
                return;
            }
            
            HudManager.Instance.OpenPuzzle(_hit.collider.GetComponent<PuzzleManager>().puzzleType);
            _hit.collider.GetComponent<PuzzleManager>().Initialize();
        }
    }

    [Header("Crouch Settings")]
    [SerializeField] private float normalHeight = 2f;
    [SerializeField] private float crouchHeight = 1f;
    [SerializeField] private Vector3 normalCenter = new Vector3(0, 1f, 0);
    [SerializeField] private Vector3 crouchCenter = new Vector3(0, 0.5f, 0);
    public void OnCrouch(InputAction.CallbackContext context)
    {
        isCrouching = !isCrouching;
        animator.SetBool("isCrouch", isCrouching);
        _controller.center = isCrouching ? crouchCenter : normalCenter;
        _controller.height = isCrouching ? crouchHeight : normalHeight;
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
        
        animator.SetBool("isGround", _controller.isGrounded);
    }
    

    private float _animSpeed;

    private void Move()
    {
        if (HudManager.Instance.IsTarget) return;

        Vector3 move = transform.right * _movement.x / 2 + transform.forward * _movement.y;
        move *= isCrouching ? 0.2f : 0.8f;

        _controller.Move(move * (moveSpeed * Time.fixedDeltaTime));

        float targetSpeed = move.magnitude > 0.1f ? 1f : 0f;
        _animSpeed = Mathf.Lerp(_animSpeed, targetSpeed, 10f * Time.deltaTime);
        animator.SetFloat("HSpeed", _animSpeed);
    }

    private void Rotate()
    {
        cameraPivot.position = eyes.position;
        if (!HudManager.Instance.IsTarget)
        {
            float mouseX = _rotate.x * rotateSpeed * Time.deltaTime;
            float mouseY = _rotate.y * rotateSpeed * Time.deltaTime;

            _yRotation += mouseX;
            
            _xRotation -= mouseY;

        }
        _xRotation = Mathf.Clamp(_xRotation, -60f, 60f);
        
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
        animator.SetFloat("VSpeed", _controller.isGrounded ? 0f : velocity.y);
        
    }
    
    private void HandleJump()
    {
        if (!_jump) return;

        if (_controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * _gravity);
            animator.SetTrigger("Jump");
        }
        
        
        
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
