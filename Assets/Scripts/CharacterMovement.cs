using UnityEngine;
using SimpleInputNamespace;

public class CharacterMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float runSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float acceleration = 5f;
    [SerializeField] private float deceleration = 5f;
    [SerializeField] private Joystick joystick;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float runningJumpThreshold = 0.7f; // Hangi hızda running jump tetikleneceği (0-1 arası)

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string speedParameter = "speed";

    // Components
    private CharacterController characterController;

    // Movement variables
    private Vector3 moveDirection;
    private float currentSpeed;
    private float verticalVelocity;
    private bool isJumping;

    public ThirdPersonOrbitCamera thirdPersonOrbitCamera;
    private Vector3 lastMoveDirection = Vector3.zero;
    private bool isGrounded;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();

        // If animator wasn't assigned in the inspector, try to get it from this GameObject
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    private void Update()
    {
        HandleMovement();
        HandleJump();
        UpdateAnimation();
    }

    private void HandleJump()
    {
        //Debug.Log("mine: " + IsGrounded() + " -- char: " + characterController.isGrounded);

        if (IsGrounded())
        {
            //verticalVelocity = -2f; // Ground check için küçük bir negatif değer
            isJumping = false;
        }

        // Apply gravity
        verticalVelocity += Physics.gravity.y * Time.deltaTime;
        Vector3 verticalMovement = Vector3.up * verticalVelocity * Time.deltaTime;
        characterController.Move(verticalMovement);
    }

    public void JumpOnClick()
    {
        Debug.Log("mine: " + IsGrounded() + " -- char: " + characterController.isGrounded);
        Debug.Log("isJumping: " + isJumping);
        if (IsGrounded() && !isJumping)
        {
            verticalVelocity = jumpForce;
            isJumping = true;

            // Hıza göre farklı zıplama animasyonları
            float normalizedSpeed = currentSpeed / runSpeed;
            if (normalizedSpeed >= runningJumpThreshold)
            {
                animator.SetTrigger("RunningJump");
            }
            else
            {
                animator.SetTrigger("Jump");
            }
        }
    }

    private void HandleMovement()
    {
        float horizontalInput = joystick.xAxis.value;
        float verticalInput = joystick.yAxis.value;

        float inputMagnitude = Mathf.Clamp01(new Vector2(horizontalInput, verticalInput).magnitude);

        // Eğer giriş varsa, hareket yönünü güncelle
        if (inputMagnitude > 0.1f)
        {
            moveDirection = new Vector3(horizontalInput, 0f, verticalInput);
            moveDirection = thirdPersonOrbitCamera.CameraRotation * moveDirection;
            moveDirection.y = 0f; // Y ekseninde hareketi iptal et
            lastMoveDirection = moveDirection.normalized; // Son hareket yönünü güncelle
        }
        else
        {
            moveDirection = lastMoveDirection; // Joystick bırakıldığında son yönü koru
        }

        if (inputMagnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            float targetSpeed = Mathf.Lerp(walkSpeed, runSpeed, inputMagnitude);
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, deceleration * Time.deltaTime);
        }

        // Y ekseni hareketini verticalVelocity ile birleştir
        Vector3 horizontalMovement = moveDirection * currentSpeed * Time.deltaTime;
        characterController.Move(horizontalMovement);
    }

    private void UpdateAnimation()
    {
        if (animator != null)
        {
            // Set the speed parameter for the blend tree (0-1)
            float normalizedSpeed = currentSpeed / runSpeed;
            animator.SetFloat(speedParameter, normalizedSpeed);
        }
    }
    bool IsGrounded()
{
    float distanceToGround = 0.2f; // Küçük bir mesafe
    return Physics.Raycast(transform.position, Vector3.down, distanceToGround);
}

    private void OnTriggerEnter(Collider other) {
        isGrounded = true;
    }
    private void OnTriggerExit(Collider other) {
        isGrounded = false;
    }
}
