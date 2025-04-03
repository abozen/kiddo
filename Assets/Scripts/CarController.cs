using System;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public WheelCollider wheelFL, wheelFR, wheelRL, wheelRR;
    public Transform wheelFLTransform, wheelFRTransform, wheelRLTransform, wheelRRTransform;
    public Transform steeringWheel;

    public float maxMotorTorque = 1500f;
    public float maxSteeringAngle = 30f;
    public float brakeForce = 3000f;

    private Rigidbody rb;
    private float xAxis, yAxis; // Joystick girişleri

    public float topSpeed = 5f; // Maksimum hız (km/h)
    public float accelerationSmoothness = 5f; // Hızlanmayı yumuşat
    public float reversePower = 0.5f; // Geri viteste torku düşür (%50)
    float currentSpeed;
    [SerializeField] private SimpleInputNamespace.Joystick joystick; // Joystick referansı eklendi



    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.5f, 0); // Aracı daha dengeli yapmak için
    }

    void Update()
    {
        // 🎮 Joystick girişlerini al
        xAxis = joystick.xAxis.value; //Input.GetAxis("Horizontal");  // Joystick X ekseni (sol-sağ)
        yAxis = joystick.yAxis.value; // Input.GetAxis("Vertical");    // Joystick Y ekseni (gaz-fren)

        ApplySteering(xAxis);
        ApplyAcceleration(yAxis);
        ApplyBraking();
        UpdateWheelTransforms();
        HandleSteeringWheel(xAxis);
    }

    private void HandleSteeringWheel(float steeringInput)
    {
        steeringWheel.localRotation = Quaternion.Euler(steeringWheel.localRotation.eulerAngles.x, steeringWheel.localRotation.eulerAngles.y, -steeringInput * maxSteeringAngle);
    }

    void ApplySteering(float steeringInput)
    {
        float steering = steeringInput * maxSteeringAngle;
        wheelFL.steerAngle = steering;
        wheelFR.steerAngle = steering;
    }

    void ApplyAcceleration(float accelerationInput)
    {
        currentSpeed = Vector3.Dot(rb.velocity, transform.forward) * 3.6f; // m/s → km/h çevir
        float motorTorque = accelerationInput * maxMotorTorque;

        // Max hız sınırı
        if (Mathf.Abs(currentSpeed) > topSpeed)
        {
            motorTorque = 0; // Hız sınırını geçmeyi engelle
        }

        // Geri gitme (yAxis negatifse, motorTorque negatif olacak)
        if (accelerationInput < 0)
        {
            motorTorque *= reversePower; // Geri giderken tork düşürülebilir (örneğin 0.5f)
        }

        wheelRL.motorTorque = motorTorque;
        wheelRR.motorTorque = motorTorque;

        Debug.Log($"Hız: {currentSpeed} km/h, Motor Torku: {motorTorque}");
    }


    void ApplyBraking()
    {
        if (currentSpeed > 0.1f && yAxis < -0.1f)  // Fren tuşu veya joystick geri hareket
        {
            wheelRL.brakeTorque = brakeForce;
            wheelRR.brakeTorque = brakeForce;
        }
        else
        {
            wheelRL.brakeTorque = 0;
            wheelRR.brakeTorque = 0;
        }
    }

    void UpdateWheelTransforms()
    {
        UpdateWheelTransform(wheelFL, wheelFLTransform);
        UpdateWheelTransform(wheelFR, wheelFRTransform);
        UpdateWheelTransform(wheelRL, wheelRLTransform);
        UpdateWheelTransform(wheelRR, wheelRRTransform);
    }

    void UpdateWheelTransform(WheelCollider collider, Transform transform)
    {
        Vector3 pos;
        Quaternion rot;
        collider.GetWorldPose(out pos, out rot);
        transform.position = pos;
        transform.rotation = rot;
    }
}
