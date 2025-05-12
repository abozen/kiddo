using UnityEngine;

public class CarColorChanger : MonoBehaviour
{
    [Header("Color Settings")]
    [SerializeField] private Color[] availableColors = new Color[5] {
        Color.red,
        Color.blue,
        Color.green,
        Color.yellow,
        Color.black
    };

    [SerializeField] private int selectedColorIndex = 0;

    [Header("References")]
    [SerializeField] private MeshRenderer[] rendererParts;

    private void OnValidate()
    {
        // Ensure the color index stays within valid range
        selectedColorIndex = Mathf.Clamp(selectedColorIndex, 0, availableColors.Length - 1);
        
        // Update color when changed in the inspector
        UpdateCarColor();
    }

    private void Awake()
    {
        // If renderers aren't manually assigned, find all mesh renderers on the car
        if (rendererParts == null || rendererParts.Length == 0)
        {
            rendererParts = GetComponentsInChildren<MeshRenderer>();
        }
        
        // Apply the selected color at startup
        UpdateCarColor();
    }

    public void UpdateCarColor()
    {
        if (rendererParts == null || rendererParts.Length == 0)
            return;

        Color selectedColor = availableColors[selectedColorIndex];

        // Apply the color to all renderer parts
        foreach (MeshRenderer renderer in rendererParts)
        {
            if (renderer != null)
            {
                // Use sharedMaterials to avoid leaking materials in edit mode
                Material[] sharedMats = renderer.sharedMaterials;
                for (int i = 0; i < sharedMats.Length; i++)
                {
                    // Create new material instance only at runtime
                    if (Application.isPlaying)
                    {
                        renderer.materials[i].color = selectedColor;
                    }
                    else
                    {
                        // In edit mode, only modify if the material is already an instance
                        if (sharedMats[i] != null && !sharedMats[i].name.Contains("(Instance)"))
                        {
                            // Create a temporary copy for preview purposes
                            Material tempMaterial = new Material(sharedMats[i]);
                            tempMaterial.color = selectedColor;
                            
                            // Replace in the shared materials array
                            sharedMats[i] = tempMaterial;
                        }
                        else if (sharedMats[i] != null)
                        {
                            // If it's already an instance, we can modify it directly
                            sharedMats[i].color = selectedColor;
                        }
                    }
                }
                
                // Apply the modified materials array back if in edit mode
                if (!Application.isPlaying)
                {
                    renderer.sharedMaterials = sharedMats;
                }
            }
        }
    }

    // Method to change color at runtime (can be called from other scripts)
    public void SetCarColor(int colorIndex)
    {
        if (colorIndex >= 0 && colorIndex < availableColors.Length)
        {
            selectedColorIndex = colorIndex;
            UpdateCarColor();
        }
    }

    // Method to set a custom color not in the predefined list
    public void SetCustomColor(Color newColor)
    {
        foreach (MeshRenderer renderer in rendererParts)
        {
            if (renderer != null)
            {
                if (Application.isPlaying)
                {
                    // At runtime, we can modify materials directly
                    foreach (Material mat in renderer.materials)
                    {
                        mat.color = newColor;
                    }
                }
                else
                {
                    // In edit mode, use sharedMaterials to avoid leaks
                    Material[] sharedMats = renderer.sharedMaterials;
                    for (int i = 0; i < sharedMats.Length; i++)
                    {
                        if (sharedMats[i] != null)
                        {
                            if (!sharedMats[i].name.Contains("(Instance)"))
                            {
                                Material tempMaterial = new Material(sharedMats[i]);
                                tempMaterial.color = newColor;
                                sharedMats[i] = tempMaterial;
                            }
                            else
                            {
                                sharedMats[i].color = newColor;
                            }
                        }
                    }
                    renderer.sharedMaterials = sharedMats;
                }
            }
        }
    }
}