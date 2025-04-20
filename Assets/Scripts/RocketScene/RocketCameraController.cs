using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketCameraController : MonoBehaviour
{
   [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0, 3, -8);
    [SerializeField] private float smoothSpeed = 0.125f;
    
    private void FixedUpdate()
    {
        if (target == null)
            return;
            
        // Calculate desired position
        Vector3 desiredPosition = target.position + offset;
        
        // Smoothly move camera
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
        
        // Make camera look at target
        transform.LookAt(target);
    }
}
