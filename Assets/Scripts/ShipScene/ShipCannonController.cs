using UnityEngine;
using System.Collections;

public class ShipCannonController : MonoBehaviour
{
    [Header("Cannon Settings")]
    [SerializeField] private Transform[] leftCannons;
    [SerializeField] private Transform[] rightCannons;
    [SerializeField] private Transform frontCannon;
    [SerializeField] private GameObject cannonBallPrefab;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float cannonBallSpeed = 20f;

    [Header("References")]
    [SerializeField] private ThirdPersonOrbitCamera cameraController;

    private float nextFireTime;
    private bool isFiring;

    private void Update()
    {
         if (Input.GetKeyDown("space"))
        {
            if (Time.time >= nextFireTime)
            {
                FireCannons();
                nextFireTime = Time.time + fireRate;
            }
        }
    }

    private void FireCannons()
    {
        // Get camera's forward direction relative to ship
        Vector3 cameraForward = cameraController.CameraRotation * Vector3.forward;
        float angle = Vector3.SignedAngle(transform.forward, cameraForward, Vector3.up);

        // Determine which cannons to fire based on camera direction
        if (Mathf.Abs(angle) < 45f)
        {
            // Fire front cannon
            Debug.Log("fire front");
            FireCannon(frontCannon);
        }
        else if (angle > 0)
        {
            // Fire right cannons
            Debug.Log("fire right");

            foreach (Transform cannon in rightCannons)
            {
                FireCannon(cannon);
            }
        }
        else
        {
            // Fire left cannons
            Debug.Log("fire left");

            foreach (Transform cannon in leftCannons)
            {
                FireCannon(cannon);
            }
        }
    }

    private void FireCannon(Transform cannon)
    {
        if (cannon == null) return;

        GameObject cannonBall = Instantiate(cannonBallPrefab, cannon.position, cannon.rotation);
        Rigidbody rb = cannonBall.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = cannon.forward * cannonBallSpeed;
        }
    }
} 