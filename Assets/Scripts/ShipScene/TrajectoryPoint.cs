using UnityEngine;

public class TrajectoryPoint : MonoBehaviour
{
    [Header("Visual Settings")]
    [SerializeField] private float fadeSpeed = 1.5f;
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private float pulseAmount = 0.2f;
    
    private float initialScale;
    private MeshRenderer meshRenderer;
    private float lifeTime = 0f;
    
    // private void Awake()
    // {
    //     meshRenderer = GetComponent<MeshRenderer>();
    //     if (meshRenderer == null)
    //     {
    //         meshRenderer = GetComponentInChildren<MeshRenderer>();
    //     }
        
    //     initialScale = transform.localScale.x;
    // }
    
    private void Update()
    {
        // // Subtle pulsing effect
        // float pulse = Mathf.Sin(lifeTime * pulseSpeed) * pulseAmount;
        // float currentScale = initialScale + pulse;
        // transform.localScale = new Vector3(currentScale, currentScale, currentScale);
        
        // // Fade out if we have a renderer
        // if (meshRenderer != null)
        // {
        //     Color currentColor = meshRenderer.material.color;
        //     currentColor.a = Mathf.Lerp(currentColor.a, 0.6f, Time.deltaTime * fadeSpeed);
        //     meshRenderer.material.color = currentColor;
        // }
        
        // lifeTime += Time.deltaTime;
    }
}