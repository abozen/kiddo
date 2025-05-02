using UnityEngine;

public enum ShipState
{
    Movement,
    Firing
}
public enum FiringDirection
    {
        Left,
        Front,
        Right
    }

public class ShipStateManager : MonoBehaviour
{
    [System.Serializable]
    public class CameraView
    {
        public Vector3 offset;
        public Vector3 lookAtOffset;
    }

    [Header("State Settings")]
    [SerializeField] private KeyCode toggleFiringStateKey = KeyCode.E;
    [SerializeField] private float firingCooldown = 2f;
    [SerializeField] private float minAimDistance = 5f;
    [SerializeField] private float maxAimDistance = 50f;
    [SerializeField] private float minVerticalAngle = -30f;
    [SerializeField] private float maxVerticalAngle = 30f;
    [SerializeField] private float aimSensitivity = 1.0f;

    [Header("Camera Settings")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float cameraTransitionSpeed = 5f;
    [SerializeField] private CameraView leftCameraView = new CameraView 
    { 
        offset = new Vector3(-5f, 2f, 0f),
        lookAtOffset = new Vector3(0f, 0f, 5f)
    };
    [SerializeField] private CameraView frontCameraView = new CameraView 
    { 
        offset = new Vector3(0f, 2f, 5f),
        lookAtOffset = new Vector3(0f, 0f, 10f)
    };
    [SerializeField] private CameraView rightCameraView = new CameraView 
    { 
        offset = new Vector3(5f, 2f, 0f),
        lookAtOffset = new Vector3(0f, 0f, 5f)
    };

    private ShipState currentState = ShipState.Movement;
    private ShipController shipController;
    private ShipCannonSystem cannonSystem;
    private ThirdPersonOrbitCamera orbitCamera;
    private float lastFireTime;
    private Vector3 targetCameraPosition;
    private Quaternion targetCameraRotation;
    private Vector2 initialTouchPosition;
    private Vector2 previousTouchPosition;
    private bool isAiming;
    private float aimStartTime;
    private FiringDirection currentFiringDirection = FiringDirection.Front;
    private Vector2 aimDelta = Vector2.zero;

    private void Start()
    {
        shipController = GetComponent<ShipController>();
        cannonSystem = GetComponent<ShipCannonSystem>();
        
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        orbitCamera = mainCamera.GetComponent<ThirdPersonOrbitCamera>();
        targetCameraPosition = mainCamera.transform.position;
        targetCameraRotation = mainCamera.transform.rotation;
    }

    private void Update()
    {
        HandleStateTransition();
        
        switch (currentState)
        {
            case ShipState.Movement:
                HandleMovementState();
                break;
            case ShipState.Firing:
                HandleFiringState();
                break;
        }

        UpdateCamera();
    }

    private void HandleStateTransition()
    {
        if (Input.GetKeyDown(toggleFiringStateKey))
        {
            ToggleState();
        }
    }

    public void ToggleState()
    {
        currentState = currentState == ShipState.Movement ? ShipState.Firing : ShipState.Movement;
        if (currentState == ShipState.Firing)
        {
            // Initialize firing state
            isAiming = false;
            aimDelta = Vector2.zero;
            if (orbitCamera != null)
            {
                orbitCamera.enabled = false;
            }
        }
        else
        {
            if (cannonSystem != null)
            {
                cannonSystem.StopAiming();
            }
            if (orbitCamera != null)
            {
                orbitCamera.enabled = true;
            }
        }
    }

    private void HandleMovementState()
    {
        // Enable normal ship controls
        if (shipController != null)
        {
            shipController.enabled = true;
        }
    }

    private void HandleFiringState()
    {
        // Disable normal ship controls
        if (shipController != null)
        {
            shipController.enabled = false;
        }

        // Handle arrow key input for camera direction
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentFiringDirection = FiringDirection.Left;
            if (isAiming && cannonSystem != null)
            {
                cannonSystem.StartAiming(currentFiringDirection);
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentFiringDirection = FiringDirection.Right;
            if (isAiming && cannonSystem != null)
            {
                cannonSystem.StartAiming(currentFiringDirection);
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentFiringDirection = FiringDirection.Front;
            if (isAiming && cannonSystem != null)
            {
                cannonSystem.StartAiming(currentFiringDirection);
            }
        }

        // Handle touch input for mobile
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    HandleTouchStart(touch.position);
                    break;
                
                case TouchPhase.Moved:
                    HandleTouchMove(touch.position);
                    break;
                
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    HandleTouchEnd();
                    break;
            }
        }
        // Handle mouse input for testing in editor
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                HandleTouchStart(Input.mousePosition);
            }
            else if (Input.GetMouseButton(0) && isAiming)
            {
                HandleTouchMove(Input.mousePosition);
            }
            else if (Input.GetMouseButtonUp(0) && isAiming)
            {
                HandleTouchEnd();
            }
        }
    }

    private void HandleTouchStart(Vector2 position)
    {
        isAiming = true;
        initialTouchPosition = position;
        previousTouchPosition = position;
        aimStartTime = Time.time;
        aimDelta = Vector2.zero;
        
        if (cannonSystem != null)
        {
            cannonSystem.StartAiming(currentFiringDirection);
        }
    }

    private void HandleTouchMove(Vector2 position)
    {
        if (!isAiming) return;
        
        // Calculate delta from previous position for smooth control
        Vector2 delta = position - previousTouchPosition;
        previousTouchPosition = position;
        
        // Accumulate aim delta
        aimDelta += delta;
        
        // Update the aiming in the cannon system
        if (cannonSystem != null)
        {
            cannonSystem.UpdateAimTarget(aimDelta, aimSensitivity);
        }
    }

    private void HandleTouchEnd()
    {
        if (!isAiming) return;
        
        isAiming = false;
        if (Time.time - lastFireTime >= firingCooldown)
        {
            float holdTime = Time.time - aimStartTime;
            if (cannonSystem != null)
            {
                cannonSystem.FireCannons(currentFiringDirection, aimDelta, holdTime);
            }
            lastFireTime = Time.time;
        }
    }

    private void UpdateCamera()
    {
        if (currentState == ShipState.Firing)
        {
            // Get the appropriate camera view based on firing direction
            CameraView currentView = currentFiringDirection switch
            {
                FiringDirection.Left => leftCameraView,
                FiringDirection.Right => rightCameraView,
                _ => frontCameraView
            };

            // Calculate target position and look-at point
            targetCameraPosition = transform.position + transform.TransformDirection(currentView.offset);
            Vector3 lookAtPoint = transform.position + transform.TransformDirection(currentView.lookAtOffset);
            targetCameraRotation = Quaternion.LookRotation(lookAtPoint - targetCameraPosition);

            // Smoothly move camera
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetCameraPosition, Time.deltaTime * cameraTransitionSpeed);
            mainCamera.transform.rotation = Quaternion.Slerp(mainCamera.transform.rotation, targetCameraRotation, Time.deltaTime * cameraTransitionSpeed);
        }
        else
        {
            // Return to default camera position
            targetCameraPosition = transform.position + transform.TransformDirection(new Vector3(0f, 5f, -10f));
            targetCameraRotation = Quaternion.LookRotation(transform.position - targetCameraPosition);

            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetCameraPosition, Time.deltaTime * cameraTransitionSpeed);
            mainCamera.transform.rotation = Quaternion.Slerp(mainCamera.transform.rotation, targetCameraRotation, Time.deltaTime * cameraTransitionSpeed);
        }
    }
    
    // Public method to get current firing direction - used by ShipCannonSystem
    public FiringDirection GetCurrentFiringDirection()
    {
        return currentFiringDirection;
    }
}