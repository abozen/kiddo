using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSceneController : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private CarGameManager gameManager;
    [SerializeField] private PlayerCarController playerCarController;
    [SerializeField] private TrafficCarSpawner trafficCarSpawner;
    [SerializeField] private RoadManager roadManager;
    [SerializeField] private UIController uiController;
    [SerializeField] private CameraFollow cameraFollow;
    
    // Start is called before the first frame update
    void Start()
    {
        // Verify that all required components are present
        VerifyComponents();
        
        // Additional initialization if needed
        Debug.Log("Car Scene Initialized");
    }
    
    private void VerifyComponents()
    {
        // Check if components are assigned, and find them if not
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<CarGameManager>();
            if (gameManager == null)
            {
                Debug.LogError("CarGameManager not found in the scene. Please add a CarGameManager component.");
            }
        }
        
        if (playerCarController == null)
        {
            playerCarController = FindObjectOfType<PlayerCarController>();
            if (playerCarController == null)
            {
                Debug.LogError("PlayerCarController not found in the scene. Please add a PlayerCarController component.");
            }
        }
        
        if (trafficCarSpawner == null)
        {
            trafficCarSpawner = FindObjectOfType<TrafficCarSpawner>();
            if (trafficCarSpawner == null)
            {
                Debug.LogError("TrafficCarSpawner not found in the scene. Please add a TrafficCarSpawner component.");
            }
        }
        
        if (roadManager == null)
        {
            roadManager = FindObjectOfType<RoadManager>();
            if (roadManager == null)
            {
                Debug.LogError("RoadManager not found in the scene. Please add a RoadManager component.");
            }
        }
        
        if (uiController == null)
        {
            uiController = FindObjectOfType<UIController>();
            if (uiController == null)
            {
                Debug.LogError("UIController not found in the scene. Please add a UIController component.");
            }
        }
        
        if (cameraFollow == null)
        {
            cameraFollow = FindObjectOfType<CameraFollow>();
            if (cameraFollow == null)
            {
                Debug.LogWarning("CameraFollow not found in the scene. Player car will not be followed by the camera.");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
