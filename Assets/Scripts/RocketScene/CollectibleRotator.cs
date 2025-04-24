using UnityEngine;

public class CollectibleRotator : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 100f; // Y ekseninde dönüş hızı
    
    private void Update()
    {
        // Y ekseninde sürekli dönüş
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
} 