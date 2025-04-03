using UnityEngine;
using UnityEngine.EventSystems;

public class ThirdPersonOrbitCamera : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 2f, -4f);

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 2f;
    [SerializeField] private float touchRotationSpeed = 3f; // Dokunmatik için ek hız ayarı
    [SerializeField] private float minVerticalAngle = -10f;
    [SerializeField] private float maxVerticalAngle = 60f;
    [SerializeField] private float smoothTime = 0.1f;

    [Header("Input Settings")]
    [SerializeField] private SimpleInputNamespace.Joystick joystick; // Joystick referansı
    [SerializeField] private float screenDividerRatio = 0.5f; // Ekranı bölen oran (0.5 = orta)

    private float yaw;
    private float pitch;
    private float smoothYaw;
    private float smoothPitch;
    private float yawVelocity;
    private float pitchVelocity;

    // Multitouch için değişkenler
    private int cameraFingerID = -1;
    private bool isCameraDragging = false;

    public Quaternion CameraRotation { get; private set; }

    private void Start()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
        Input.multiTouchEnabled = true; // Multitouch'ı aktif et

        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                target = player.transform;
            else
                Debug.LogError("Third Person Camera: No target assigned and no player found with 'Player' tag!");
        }

        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        HandleTouchInput();
        UpdateCameraPosition();
    }

    private void HandleTouchInput()
    {
        // Editor için mouse kontrolü
        #if UNITY_EDITOR
        if (Input.GetMouseButton(0) && !IsPointerOverUI())
        {
            // Fare pozisyonu ekranın sağ tarafında mı kontrol et
            if (Input.mousePosition.x > Screen.width * screenDividerRatio)
            {
                yaw += Input.GetAxis("Mouse X") * rotationSpeed;
                pitch -= Input.GetAxis("Mouse Y") * rotationSpeed;
                pitch = Mathf.Clamp(pitch, minVerticalAngle, maxVerticalAngle);
            }
        }
        #endif

        // Mobil için dokunmatik kontrol
        if (Input.touchCount > 0)
        {
            foreach (Touch touch in Input.touches)
            {
                // SADECE sağ tarafta dokunma varsa kamera kontrolü yap
                if (touch.position.x > Screen.width * screenDividerRatio)
                {
                    // UI elemanı kontrolü
                    if (IsPointerOverUIAtPosition(touch.position))
                        continue;

                    if (touch.phase == TouchPhase.Began)
                    {
                        // Yeni kamera dokunuşu başladı
                        if (cameraFingerID == -1)
                        {
                            cameraFingerID = touch.fingerId;
                            isCameraDragging = true;
                        }
                    }
                    else if (touch.fingerId == cameraFingerID)
                    {
                        if (touch.phase == TouchPhase.Moved && isCameraDragging)
                        {
                            // Kamera hareketini güncelle
                            yaw += touch.deltaPosition.x * touchRotationSpeed * Time.deltaTime;
                            pitch -= touch.deltaPosition.y * touchRotationSpeed * Time.deltaTime;
                            pitch = Mathf.Clamp(pitch, minVerticalAngle, maxVerticalAngle);
                        }
                        else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                        {
                            // Kamera dokunuşu sonlandı
                            cameraFingerID = -1;
                            isCameraDragging = false;
                        }
                    }
                }
                // Eğer parmak sol taraftan sağa geçerse, kamera kontrolünü durdur
                else if (touch.fingerId == cameraFingerID)
                {
                    cameraFingerID = -1;
                    isCameraDragging = false;
                }
            }
        }

        // Kamera dönüşünü yumuşat
        smoothYaw = Mathf.SmoothDamp(smoothYaw, yaw, ref yawVelocity, smoothTime);
        smoothPitch = Mathf.SmoothDamp(smoothPitch, pitch, ref pitchVelocity, smoothTime);
        CameraRotation = Quaternion.Euler(0, smoothYaw, 0);
    }

    private void UpdateCameraPosition()
    {
        Quaternion rotation = Quaternion.Euler(smoothPitch, smoothYaw, 0);
        Vector3 desiredPosition = target.position + rotation * offset;

        transform.position = desiredPosition;
        transform.LookAt(target.position);
    }

    private bool IsPointerOverUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }

    private bool IsPointerOverUIAtPosition(Vector2 position)
    {
        if (EventSystem.current == null)
            return false;
            
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = position;
        System.Collections.Generic.List<RaycastResult> results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }
}