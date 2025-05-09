using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target;        // Target to follow (car)
    public Vector3 offset = new Vector3(0, 2, -5); // Default offset between camera and car
    public float smoothSpeed = 5f; // Smoothness speed for following target
    
    [Header("Camera Effects")]
    public float gasEffectDistance = 1.5f; // How far back camera moves when accelerating
    public float brakeEffectDistance = 1.0f; // How far forward camera moves when braking
    public float effectSpeed = 5f; // Speed of camera movement effects
    
    [Header("Camera Positions")]
    public Vector3[] cameraPoses; // Different camera positions
    private int currentCamPos = 0;
    
    private Vector3 originalOffset; // Original offset
    private Vector3 targetOffset; // Target offset with effects applied
    
    [Header("References")]
    [SerializeField] private PlayerCarController carController;
    bool isAccelerating = false;
    bool isDeclariting = false;
    
    private void Start()
    {
        // Initialize camera position
        if (cameraPoses != null && cameraPoses.Length > 0)
        {
            offset = cameraPoses[currentCamPos];
        }
        
        // Store original offset
        originalOffset = offset;
        targetOffset = offset;
        
        // Try to find car controller if not assigned
        if (carController == null && target != null)
        {
            carController = target.GetComponent<PlayerCarController>();
        }
    }
    
    void LateUpdate()
    {
        Vector3 desiredPosition = target.position + offset; // Hedef pozisyon
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed); // Yumuşak geçiş
        transform.position = desiredPosition; // Kameranın pozisyonunu güncelle

        transform.LookAt(target); // Kamerayı arabanın yönüne çevir

        
        HandleCameraEffect();
    }
    
    private void HandleCameraEffect()
    {
        bool isAccelerating = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
        bool isBraking = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
        
        // When accelerating, camera moves back
        if (isAccelerating)
        {
            targetOffset = originalOffset - new Vector3(0, 0, gasEffectDistance);
        }
        // When braking, camera moves forward
        else if (isBraking)
        {
            targetOffset = originalOffset + new Vector3(0, 0, brakeEffectDistance);
        }
        // Neither gas nor brake, camera returns to original position
        else
        {
            targetOffset = originalOffset;
        }
        
        // Smoothly move offset to target position
        offset = Vector3.Lerp(offset, targetOffset, effectSpeed * Time.deltaTime);
    }
    
    public void ChangeCameraPos()
    {
        // Change to next camera position
        currentCamPos++;
        if (cameraPoses != null && cameraPoses.Length > 0)
        {
            currentCamPos %= cameraPoses.Length;
            originalOffset = cameraPoses[currentCamPos];
            targetOffset = originalOffset;
        }
    }

    
} 