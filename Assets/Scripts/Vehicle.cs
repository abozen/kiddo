using System;
using UnityEngine;
using UnityEngine.UI;
public class Vehicle : MonoBehaviour
{
    [Header("Vehicle Settings")]
    public string vehicleName = "Car";
    public Transform driverSeat;
    public Transform exitPoint;

    [Header("Vehicle Components")]
    public MonoBehaviour vehicleController;
    public Collider meshCollider;
    public Collider playerCollider;

    private void Start()
    {
        // Disable vehicle control at start
        if (vehicleController != null)
            vehicleController.enabled = false;
    }

    public Transform GetDriverSeat()
    {
        return driverSeat;
    }

    public Transform GetExitPoint()
    {
        return exitPoint;
    }

    public string GetVehicleInfo()
    {
        return vehicleName;
    }

    public void EnableControl(bool enable)
    {
        if (vehicleController != null)
            vehicleController.enabled = enable;
    }

    public void SetCollider(bool ignore)
    {
        Physics.IgnoreCollision(meshCollider, playerCollider, ignore);
    }
}