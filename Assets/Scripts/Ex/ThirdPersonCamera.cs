using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Position Settings")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 2f, -4f);
    [SerializeField] private float smoothSpeed = 5f;

    [Header("Look Settings")]
    [SerializeField] private bool lookAtTarget = true;
    [SerializeField] private Vector3 lookOffset = new Vector3(0f, 1f, 0f);

    [Header("Collision Settings")]
    [SerializeField] private bool handleCollisions = true;
    [SerializeField] private float collisionRadius = 0.2f;
    [SerializeField] private LayerMask collisionLayers;
    [SerializeField] private float minDistance = 1f;

    private Vector3 desiredPosition;
    private Vector3 smoothedPosition;
    private float originalDistance;

    private void Start()
    {
        
        // If no target is set, try to find the player by tag
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                target = player.transform;
            else
                Debug.LogError("Third Person Camera: No target assigned and no player found with 'Player' tag!");
        }

        // Calculate the original distance from the target
        originalDistance = offset.magnitude;
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        // Calculate the desired position
        desiredPosition = target.position + target.TransformDirection(offset);

        // Handle collisions if enabled
        if (handleCollisions)
        {
            HandleCameraCollision();
        }

        // Smoothly move the camera
        smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        // Make the camera look at the target if enabled
        if (lookAtTarget)
        {
            transform.LookAt(target.position + lookOffset);
        }
    }

    private void HandleCameraCollision()
    {
        Vector3 directionToTarget = target.position - (target.position + target.TransformDirection(offset));
        float targetDistance = directionToTarget.magnitude;

        // Cast a ray from the target position to the desired camera position
        Ray ray = new Ray(target.position + lookOffset, -directionToTarget.normalized);
        RaycastHit hit;

        if (Physics.SphereCast(ray, collisionRadius, out hit, originalDistance, collisionLayers))
        {
            // If we hit something, adjust the camera position
            float distanceToObstacle = hit.distance;
            float adjustedDistance = Mathf.Max(distanceToObstacle, minDistance);

            // Adjust the desired position
            desiredPosition = target.position - directionToTarget.normalized * adjustedDistance;
        }
    }

    // Draw gizmos in the editor for visualization
    private void OnDrawGizmosSelected()
    {
        if (target != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(target.position + lookOffset, 0.2f);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(target.position + lookOffset, target.position + target.TransformDirection(offset));

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(target.position + target.TransformDirection(offset), collisionRadius);
        }
    }
}