using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    public enum CollectableType
    {
        Bronze = 1,
        Silver = 5,
        Gold = 10
    }

    public CollectableType type = CollectableType.Bronze;

    // Visual indicator properties
    [SerializeField] private Color bronzeColor = new Color(0.8f, 0.5f, 0.2f);
    [SerializeField] private Color silverColor = new Color(0.75f, 0.75f, 0.75f);
    [SerializeField] private Color goldColor = new Color(1.0f, 0.84f, 0.0f);

    [SerializeField] private float rotationSpeed = 90f;

    private void Start()
    {
        Color collectableColor;
        
        // Set the color based on type
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            switch (type)
            {
                case CollectableType.Bronze:
                    renderer.material.color = bronzeColor;
                    collectableColor = bronzeColor;
                    break;
                case CollectableType.Silver:
                    renderer.material.color = silverColor;
                    collectableColor = silverColor;
                    break;
                case CollectableType.Gold:
                    renderer.material.color = goldColor;
                    collectableColor = goldColor;
                    break;
                default:
                    collectableColor = bronzeColor;
                    break;
            }
            
            // Find and update particle system color
            Transform shineTransform = transform.Find("shine");
            if (shineTransform != null)
            {
                Transform shinyTransform = shineTransform.Find("shiny");
                if (shinyTransform != null)
                {
                    Transform circlesTransform = shinyTransform.Find("Circles");
                    if (circlesTransform != null)
                    {
                        ParticleSystem particleSystem = circlesTransform.GetComponent<ParticleSystem>();
                        if (particleSystem != null)
                        {
                            var mainModule = particleSystem.main;
                            mainModule.startColor = collectableColor;
                        }
                    }
                }
            }
        }
    }

    private void Update()
    {
        // Rotate the collectable
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    public int GetValue()
    {
        return (int)type;
    }
} 