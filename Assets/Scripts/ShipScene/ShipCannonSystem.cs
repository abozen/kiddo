using UnityEngine;
using System.Collections.Generic;

public class ShipCannonSystem : MonoBehaviour
{
    [System.Serializable]
    public class CannonGroup
    {
        public Transform[] leftCannons;
        public Transform[] frontCannons;
        public Transform[] rightCannons;
    }

    [Header("Cannon Settings")]
    [SerializeField] private CannonGroup cannonGroups;
    [SerializeField] private GameObject cannonBallPrefab;
    [SerializeField] private GameObject projectileTrajectoryPrefab;
    [SerializeField] private float projectileSpeed = 20f;
    [SerializeField] private float projectileLifetime = 5f;

    [Header("Aiming Settings")]
    [SerializeField] private float minHorizontalAim = -45f;
    [SerializeField] private float maxHorizontalAim = 45f;
    [SerializeField] private float minVerticalAim = -10f;
    [SerializeField] private float maxVerticalAim = 30f;
    [SerializeField] private float aimSmoothness = 5f;

    [Header("Trajectory Visualization")]
    [SerializeField] private int trajectoryPoints = 30;
    [SerializeField] private float trajectoryTimeStep = 0.1f;

    private Dictionary<FiringDirection, Transform[]> cannonMap;
    private Vector2 currentAimPosition;
    private Vector2 targetAimPosition;
    private List<GameObject> trajectoryObjects = new List<GameObject>();
    private bool isDrawingTrajectory = false;
    private Vector2 screenCenter;
    private ShipStateManager stateManager;

    private void Awake()
    {
        InitializeCannonMap();
        screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        stateManager = GetComponent<ShipStateManager>();
    }

    private void Update()
    {
        if (isDrawingTrajectory)
        {
            UpdateAimPosition();
            DrawTrajectory();
        }
    }

    private void InitializeCannonMap()
    {
        cannonMap = new Dictionary<FiringDirection, Transform[]>
        {
            { FiringDirection.Left, cannonGroups.leftCannons },
            { FiringDirection.Front, cannonGroups.frontCannons },
            { FiringDirection.Right, cannonGroups.rightCannons }
        };
    }

    public void StartAiming(FiringDirection direction)
    {
        isDrawingTrajectory = true;
        ClearTrajectory();
        targetAimPosition = Vector2.zero;
        currentAimPosition = Vector2.zero;
    }

    public void UpdateAimTarget(Vector2 aimDelta, float sensitivity = 1.0f)
    {
        // Convert aim delta to normalized aim coordinates
        float horizontalAim = Mathf.Clamp(aimDelta.x * sensitivity / (Screen.width * 0.5f), -1f, 1f);
        float verticalAim = Mathf.Clamp(aimDelta.y * sensitivity / (Screen.height * 0.5f), -1f, 1f);

        // Apply min/max limits by mapping -1 to 1 range to our min/max angles
        horizontalAim = Mathf.Lerp(minHorizontalAim, maxHorizontalAim, (horizontalAim + 1f) * 0.5f) / 45f; // Normalize to -1 to 1
        verticalAim = Mathf.Lerp(minVerticalAim, maxVerticalAim, (verticalAim + 1f) * 0.5f) / 30f; // Normalize to -1 to 1

        targetAimPosition = new Vector2(horizontalAim, verticalAim);
    }

    private void UpdateAimPosition()
    {
        // Smoothly interpolate current aim position toward target
        currentAimPosition = Vector2.Lerp(currentAimPosition, targetAimPosition, Time.deltaTime * aimSmoothness);
    }

    public void StopAiming()
    {
        isDrawingTrajectory = false;
        ClearTrajectory();
    }

    public Transform[] GetCannonPositions(FiringDirection direction)
    {
        return cannonMap.TryGetValue(direction, out Transform[] cannons) ? cannons : new Transform[0];
    }

    public void FireCannons(FiringDirection direction, Vector2 aimPosition, float holdTime = 0f)
    {
        Transform[] cannons = GetCannonPositions(direction);
        if (cannons == null || cannons.Length == 0) return;

        StopAiming();

        foreach (Transform cannon in cannons)
        {
            // Calculate firing direction based on aim position
            Vector3 firingDirection = CalculateFiringDirection(cannon, currentAimPosition);
            FireProjectile(cannon.position, firingDirection);
        }
    }

    private Vector3 CalculateFiringDirection(Transform cannon, Vector2 aimPosition)
    {
        // Get base direction from cannon
        Vector3 baseDirection = cannon.forward + new Vector3(0, 0.5f, 0);

        // Calculate rotation based on aim
        float horizontalAngle = Mathf.Lerp(minHorizontalAim, maxHorizontalAim, (aimPosition.x + 1f) * 0.5f);
        float verticalAngle = Mathf.Lerp(minVerticalAim, maxVerticalAim, (aimPosition.y + 1f) * 0.5f);

        // Create rotation based on aim angles
        Quaternion horizontalRotation = Quaternion.Euler(0, horizontalAngle, 0);
        Quaternion verticalRotation = Quaternion.Euler(verticalAngle, 0, 0);

        // Apply rotations to base direction
        Vector3 firingDirection = verticalRotation * horizontalRotation * baseDirection;
        //Vector3 firingDirection = new Vector3(verticalAngle,horizontalAngle,0) + baseDirection;

        return firingDirection.normalized;
    }

    private void FireProjectile(Vector3 position, Vector3 direction)
    {
        if (cannonBallPrefab == null) return;

        GameObject projectile = Instantiate(cannonBallPrefab, position, Quaternion.identity);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        if (rb != null)
        {
            // Add physical properties to make it look realistic
            rb.velocity = direction * projectileSpeed;

            // Optional: Add slight random torque for more realistic rotation
            //rb.AddTorque(Random.insideUnitSphere * 2f, ForceMode.Impulse);
        }

        // Destroy projectile after lifetime
        Destroy(projectile, projectileLifetime);
    }

    private void DrawTrajectory()
    {
        // Clear previous trajectory visualization
        ClearTrajectory();

        // Get active cannons based on current direction
        FiringDirection currentDirection = stateManager != null ? stateManager.GetCurrentFiringDirection() : FiringDirection.Front;
        Transform[] cannons = GetCannonPositions(currentDirection);
        if (cannons == null || cannons.Length == 0) return;

        // Draw trajectory for each cannon
        foreach (Transform cannon in cannons)
        {
            Vector3 startPosition = cannon.position;
            Vector3 startVelocity = CalculateFiringDirection(cannon, currentAimPosition) * projectileSpeed;

            // Create trajectory points
            for (int i = 0; i < trajectoryPoints; i++)
            {
                float time = i * trajectoryTimeStep;
                Vector3 point = CalculateTrajectoryPoint(startPosition, startVelocity, time);

                // Instantiate trajectory visualization object
                GameObject trajectoryObj = Instantiate(projectileTrajectoryPrefab, point, Quaternion.identity);

                // Scale down each consecutive point to create a narrowing effect
                float scale = Mathf.Lerp(1f, 0.4f, (float)i / trajectoryPoints);
                trajectoryObj.transform.localScale = new Vector3(scale, scale, scale);

                trajectoryObjects.Add(trajectoryObj);
            }
        }
    }

    private Vector3 CalculateTrajectoryPoint(Vector3 startPos, Vector3 startVelocity, float time)
    {
        // Physics formula for projectile motion
        return startPos + startVelocity * time + 0.5f * Physics.gravity * time * time;
    }

    private void ClearTrajectory()
    {
        foreach (GameObject obj in trajectoryObjects)
        {
            Destroy(obj);
        }
        trajectoryObjects.Clear();
    }
}