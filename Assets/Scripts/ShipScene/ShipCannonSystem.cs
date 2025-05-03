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
    [SerializeField] private Material trajectoryLineMaterial;
    [SerializeField] private float trajectoryLineWidth = 0.1f;
    [SerializeField] private Gradient trajectoryGradient;
    [SerializeField] private Material trajectoryAreaMaterial; // Material for the area between trajectories
    [SerializeField] private Color trajectoryAreaColor = new Color(0.5f, 0.5f, 1f, 0.3f); // Default transparent blue
    [SerializeField] private float zOffset = 0.05f; // Small Z-offset to prevent Z-fighting

    private Dictionary<FiringDirection, Transform[]> cannonMap;
    private Vector2 currentAimPosition;
    private Vector2 targetAimPosition;
    private List<LineRenderer> trajectoryLines = new List<LineRenderer>();
    private List<GameObject> trajectoryAreaMeshes = new List<GameObject>();
    private bool isDrawingTrajectory = false;
    private Vector2 screenCenter;
    private ShipStateManager stateManager;

    private void Awake()
    {
        InitializeCannonMap();
        screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        stateManager = GetComponent<ShipStateManager>();
        
        // Initialize default trajectory gradient for white lines
        trajectoryGradient = new Gradient();
        GradientColorKey[] colorKeys = new GradientColorKey[2];
        colorKeys[0] = new GradientColorKey(Color.white, 0.0f);
        colorKeys[1] = new GradientColorKey(Color.white, 1.0f);
        
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
        alphaKeys[0] = new GradientAlphaKey(1.0f, 0.0f);
        alphaKeys[1] = new GradientAlphaKey(0.5f, 1.0f); // Fade out at the end
        
        trajectoryGradient.SetKeys(colorKeys, alphaKeys);
        
        // Create default area material if needed
        if (trajectoryAreaMaterial == null)
        {
            // Use a shader that handles transparency better
            Shader shader = Shader.Find("Transparent/Diffuse");
            if (shader == null) shader = Shader.Find("Legacy Shaders/Transparent/Diffuse");
            
            trajectoryAreaMaterial = new Material(shader);
            trajectoryAreaMaterial.color = trajectoryAreaColor;
            // Disable culling to make the material double-sided
            trajectoryAreaMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Set rendering mode
            trajectoryAreaMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            trajectoryAreaMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            trajectoryAreaMaterial.SetInt("_ZWrite", 0); // Don't write to depth buffer
            trajectoryAreaMaterial.renderQueue = 3000; // Transparent queue
        }
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
        // Aim açılarını derecelere dönüştür
        float horizontalAngle = Mathf.Lerp(minHorizontalAim, maxHorizontalAim, (aimPosition.x + 1f) * 0.5f);
        float verticalAngle = Mathf.Lerp(minVerticalAim, maxVerticalAim, (aimPosition.y + 1f) * 0.5f);

        // Temel yön: topun baktığı yön
        Vector3 baseDirection = cannon.forward;

        // İlk olarak local eksenlerde rotasyonları oluştur
        Quaternion verticalRot = Quaternion.AngleAxis(-verticalAngle, cannon.right);   // Yukarı/Aşağı: sağ ekseni etrafında
        Quaternion horizontalRot = Quaternion.AngleAxis(horizontalAngle, cannon.up);   // Sol/Sağ: yukarı ekseni etrafında

        // Önce dikey, sonra yatay döndürme uygula
        Vector3 finalDirection = horizontalRot * verticalRot * baseDirection;

        return finalDirection.normalized;
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

        // Calculate all trajectory points first to use later for the area mesh
        List<Vector3[]> allTrajectoryPoints = new List<Vector3[]>();
        
        // Draw trajectory for each cannon
        foreach (Transform cannon in cannons)
        {
            // Create a new GameObject for the line renderer
            GameObject trajectoryLineObj = new GameObject("TrajectoryLine");
            LineRenderer lineRenderer = trajectoryLineObj.AddComponent<LineRenderer>();
            
            // Configure the line renderer
            lineRenderer.positionCount = trajectoryPoints;
            lineRenderer.startWidth = trajectoryLineWidth;
            lineRenderer.endWidth = trajectoryLineWidth * 0.4f; // Narrower at the end
            lineRenderer.useWorldSpace = true;
            
            // Set material
            if (trajectoryLineMaterial != null)
                lineRenderer.material = trajectoryLineMaterial;
            else
            {
                // Default material for lines if none provided
                lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            }
            
            // Make sure lines render on top of the area
            lineRenderer.material.renderQueue = 3100; // Higher than transparent
            lineRenderer.sortingOrder = 10;
            
            // Set the gradient - white line
            lineRenderer.colorGradient = trajectoryGradient;
            
            Vector3 startPosition = cannon.position;
            Vector3 startVelocity = CalculateFiringDirection(cannon, currentAimPosition) * projectileSpeed;

            // Calculate and set all trajectory points
            Vector3[] points = new Vector3[trajectoryPoints];
            for (int i = 0; i < trajectoryPoints; i++)
            {
                float time = i * trajectoryTimeStep;
                points[i] = CalculateTrajectoryPoint(startPosition, startVelocity, time);
            }
            
            // Store the points for area mesh creation
            allTrajectoryPoints.Add(points);
            
            // Apply all positions at once for better performance
            lineRenderer.SetPositions(points);
            
            trajectoryLines.Add(lineRenderer);
        }
        
        // Create area meshes between adjacent trajectories if we have at least 2 trajectories
        if (allTrajectoryPoints.Count >= 2)
        {
            for (int i = 0; i < allTrajectoryPoints.Count - 1; i++)
            {
                // Each successive area should be rendered behind the previous ones to avoid flickering
                float currentZOffset = zOffset * (i + 1);
                CreateAreaMeshBetweenTrajectories(allTrajectoryPoints[i], allTrajectoryPoints[i + 1], currentZOffset);
            }
        }
    }

   private void CreateAreaMeshBetweenTrajectories(Vector3[] trajectory1, Vector3[] trajectory2, float zOffsetValue = 0)
{
    if (trajectory1.Length != trajectory2.Length || trajectory1.Length < 2) return;
    
    // Create a new GameObject for the mesh
    GameObject areaMeshObj = new GameObject("TrajectoryAreaMesh");
    MeshFilter meshFilter = areaMeshObj.AddComponent<MeshFilter>();
    MeshRenderer meshRenderer = areaMeshObj.AddComponent<MeshRenderer>();
    
    // Set material with double-sided rendering
    Material materialInstance = new Material(trajectoryAreaMaterial != null ? trajectoryAreaMaterial : CreateDefaultAreaMaterial());
    materialInstance.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
    materialInstance.SetInt("_ZWrite", 0);
    materialInstance.renderQueue = 3000;
    meshRenderer.material = materialInstance;
    
    // Set renderer sorting
    meshRenderer.sortingOrder = 5;
    
    Mesh mesh = new Mesh();
    
    // Daha fazla noktayı koru ama çok karmaşık olmasın
    int pointCount = Mathf.Min(trajectory1.Length, 40); 
    int skipFactor = trajectory1.Length / pointCount;
    if (skipFactor < 1) skipFactor = 1;
    
    pointCount = Mathf.CeilToInt((float)trajectory1.Length / skipFactor);
    
    // Create vertices
    Vector3[] vertices = new Vector3[pointCount * 2];
    
    // Kamera yönünde sabit bir offset vektörü belirle
    Vector3 offsetDir = Camera.main != null ? Camera.main.transform.forward.normalized : Vector3.forward;
    
    // Tutarlı ve daha büyük bir Z-offset kullan
    float consistentZOffset = zOffsetValue * 0.1f; // Daha küçük bir değer kullan
    
    for (int i = 0; i < pointCount; i++)
    {
        int sourceIndex = Mathf.Min(i * skipFactor, trajectory1.Length - 1);
        
        // Noktaları değiştirmeden direkt ata
        vertices[i] = trajectory1[sourceIndex];
        vertices[i + pointCount] = trajectory2[sourceIndex];
    }
    
    // Tek taraflı mesh oluştur - sadece ön yüz
    int[] triangles = new int[(pointCount - 1) * 6]; // 6 indices per quad
    
    for (int i = 0; i < pointCount - 1; i++)
    {
        int indexBase = i * 6; // 6 indices per quad
        
        // Sadece ön yüz üçgenleri
        triangles[indexBase] = i;
        triangles[indexBase + 1] = i + pointCount;
        triangles[indexBase + 2] = i + 1;
        
        triangles[indexBase + 3] = i + 1;
        triangles[indexBase + 4] = i + pointCount;
        triangles[indexBase + 5] = i + pointCount + 1;
    }
    
    // Create UVs (simple mapping)
    Vector2[] uvs = new Vector2[vertices.Length];
    for (int i = 0; i < pointCount; i++)
    {
        float uvX = (float)i / (pointCount - 1);
        uvs[i] = new Vector2(uvX, 0);
        uvs[i + pointCount] = new Vector2(uvX, 1);
    }
    
    // Assign to mesh
    mesh.vertices = vertices;
    mesh.triangles = triangles;
    mesh.uv = uvs;
    
    // Normals'ı doğru hesapla ve mesh'e ata
    mesh.RecalculateNormals();
    
    // Assign mesh to mesh filter
    meshFilter.mesh = mesh;
    
    // Add to list for cleanup
    trajectoryAreaMeshes.Add(areaMeshObj);
    
    // Arka yüz için ayrı bir mesh oluştur
    GameObject backMeshObj = new GameObject("TrajectoryAreaMesh_Back");
    MeshFilter backMeshFilter = backMeshObj.AddComponent<MeshFilter>();
    MeshRenderer backMeshRenderer = backMeshObj.AddComponent<MeshRenderer>();
    
    // Aynı materyali kullan
    backMeshRenderer.material = materialInstance;
    backMeshRenderer.sortingOrder = 4; // Ön yüzden daha geride
    
    Mesh backMesh = new Mesh();
    
    // Arka yüz için mesh vertices'leri kopyala ama küçük bir offset uygula
    Vector3[] backVertices = new Vector3[vertices.Length];
    for (int i = 0; i < vertices.Length; i++) {
        backVertices[i] = vertices[i] + offsetDir * consistentZOffset;
    }
    
    // Arka yüz için üçgenleri ters sırayla oluştur (winding order is reversed)
    int[] backTriangles = new int[(pointCount - 1) * 6];
    
    for (int i = 0; i < pointCount - 1; i++)
    {
        int indexBase = i * 6;
        
        // Ters sırada üçgenler
        backTriangles[indexBase] = i;
        backTriangles[indexBase + 1] = i + 1;
        backTriangles[indexBase + 2] = i + pointCount;
        
        backTriangles[indexBase + 3] = i + 1;
        backTriangles[indexBase + 4] = i + pointCount + 1;
        backTriangles[indexBase + 5] = i + pointCount;
    }
    
    // Arka mesh için verileri ata
    backMesh.vertices = backVertices;
    backMesh.triangles = backTriangles;
    backMesh.uv = uvs; // Aynı UV'leri kullanabiliriz
    
    backMesh.RecalculateNormals();
    
    backMeshFilter.mesh = backMesh;
    trajectoryAreaMeshes.Add(backMeshObj);
}

// Yardımcı metod - varsayılan materyal oluşturma
private Material CreateDefaultAreaMaterial()
{
    Shader shader = Shader.Find("Transparent/Diffuse");
    if (shader == null) shader = Shader.Find("Legacy Shaders/Transparent/Diffuse");
    
    Material mat = new Material(shader);
    mat.color = trajectoryAreaColor;
    mat.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
    mat.SetInt("_ZWrite", 0);
    mat.renderQueue = 3000;
    return mat;
}

    private Vector3 CalculateTrajectoryPoint(Vector3 startPos, Vector3 startVelocity, float time)
    {
        // Physics formula for projectile motion
        return startPos + startVelocity * time + 0.5f * Physics.gravity * time * time;
    }

    private void ClearTrajectory()
    {
        foreach (LineRenderer line in trajectoryLines)
        {
            if (line != null)
                Destroy(line.gameObject);
        }
        trajectoryLines.Clear();
        
        foreach (GameObject meshObj in trajectoryAreaMeshes)
        {
            if (meshObj != null)
                Destroy(meshObj);
        }
        trajectoryAreaMeshes.Clear();
    }
}