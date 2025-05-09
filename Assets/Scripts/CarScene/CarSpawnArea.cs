using UnityEngine;

public class CarSpawnArea : MonoBehaviour
{
    private bool isOccupied = false;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TrafficCar") || other.CompareTag("Player"))
        {
            isOccupied = true;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("TrafficCar") || other.CompareTag("Player"))
        {
            isOccupied = false;
        }
    }
    
    public bool IsOccupied()
    {
        return isOccupied;
    }
} 