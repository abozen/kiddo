using UnityEngine;

public class TrafficCarSpawner : MonoBehaviour
{
    [Header("Note")]
    [SerializeField] [TextArea(3, 5)] private string deprecationNotice = 
        "This component is deprecated. Traffic spawning is now handled by RoadTrafficController components attached to each road tile.";
    
    private void Start()
    {
        Debug.LogWarning("TrafficCarSpawner is deprecated. Traffic spawning is now handled by RoadTrafficController components attached to each road tile.");
    }
} 