using UnityEngine;

public class ObstacleRotator : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private float minRotationSpeed = 20f;
    [SerializeField] private float maxRotationSpeed = 40f;
    [SerializeField] private Vector3 minRotationAxis = new Vector3(0.5f, 0.5f, 0.5f);
    [SerializeField] private Vector3 maxRotationAxis = new Vector3(1.5f, 1.5f, 1.5f);
    
    [Header("Floating Settings")]
    [SerializeField] private float minFloatAmplitude = 0.3f;
    [SerializeField] private float maxFloatAmplitude = 0.7f;
    [SerializeField] private float minFloatFrequency = 0.7f;
    [SerializeField] private float maxFloatFrequency = 1.3f;
    
    private Vector3 startPosition;
    private float timeOffset;
    private float rotationSpeed;
    private Vector3 rotationAxis;
    private float floatAmplitude;
    private float floatFrequency;
    
    private void Start()
    {
        startPosition = transform.position;
        timeOffset = Random.Range(0f, 2f * Mathf.PI); // Rastgele başlangıç fazı
        
        // Rastgele değerler atama
        rotationSpeed = Random.Range(minRotationSpeed, maxRotationSpeed);
        rotationAxis = new Vector3(
            Random.Range(minRotationAxis.x, maxRotationAxis.x),
            Random.Range(minRotationAxis.y, maxRotationAxis.y),
            Random.Range(minRotationAxis.z, maxRotationAxis.z)
        );
        floatAmplitude = Random.Range(minFloatAmplitude, maxFloatAmplitude);
        floatFrequency = Random.Range(minFloatFrequency, maxFloatFrequency);
    }
    
    private void Update()
    {
        // Sürekli dönüş
        transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime);
        
        // Hafif yüzen hareket
        float newY = startPosition.y + Mathf.Sin((Time.time + timeOffset) * floatFrequency) * floatAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
} 