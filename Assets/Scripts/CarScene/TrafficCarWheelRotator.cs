using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficCarWheelRotator : MonoBehaviour
{
    [SerializeField] private Transform frontLeftWheel;
    [SerializeField] private Transform frontRightWheel;
    [SerializeField] private Transform rearLeftWheel;
    [SerializeField] private Transform rearRightWheel;
    [SerializeField] private float rotationSpeed = 200f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (frontLeftWheel) frontLeftWheel.Rotate(rotationSpeed * Time.deltaTime, 0, 0);
        if (frontRightWheel) frontRightWheel.Rotate(rotationSpeed * Time.deltaTime, 0, 0);
        if (rearLeftWheel) rearLeftWheel.Rotate(rotationSpeed * Time.deltaTime, 0, 0);
        if (rearRightWheel) rearRightWheel.Rotate(rotationSpeed * Time.deltaTime, 0, 0);
    }
}
