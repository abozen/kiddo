using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }
    
    [Header("Transition Settings")]
    [SerializeField] private Image[] cloudImagePrefabs;
    [SerializeField] private int cloudCount = 40; // Bulut sayısını artırdık
    [SerializeField] private float transitionDuration = 1.5f;
    [SerializeField] private float cloudSpreadRadius = 1000f;
    [SerializeField] private Canvas transitionCanvas;
    
    private Image[] clouds;
    private Vector2[] targetPositions;
    private bool isTransitioning = false;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeClouds();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void InitializeClouds()
    {
        clouds = new Image[cloudCount];
        targetPositions = new Vector2[cloudCount];
        
        // Ekranı grid olarak böl
        int gridSizeX = 8; // Yatayda 8 hücre
        int gridSizeY = 5; // Dikeyde 5 hücre
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        float cellWidth = screenWidth / gridSizeX;
        float cellHeight = screenHeight / gridSizeY;
        
        for (int i = 0; i < cloudCount; i++)
        {
            // Rastgele bulut prefabı seç
            Image randomCloudPrefab = cloudImagePrefabs[Random.Range(0, cloudImagePrefabs.Length)];
            clouds[i] = Instantiate(randomCloudPrefab, transitionCanvas.transform);
            clouds[i].gameObject.SetActive(false);
            
            // Grid pozisyonunu hesapla
            int row = i / gridSizeX;
            int col = i % gridSizeX;
            
            // Grid hücresinin merkez noktasını hesapla
            float x = (col * cellWidth) - (screenWidth / 2) + (cellWidth / 2);
            float y = (row * cellHeight) - (screenHeight / 2) + (cellHeight / 2);
            
            // Rastgele offset ekle (daha küçük offset)
            x += Random.Range(-cellWidth * 0.2f, cellWidth * 0.2f);
            y += Random.Range(-cellHeight * 0.2f, cellHeight * 0.2f);
            
            // Üst kısım için ekstra bulutlar
            if (row == 0)
            {
                // Üst sıraya ekstra bulutlar ekle
                if (i < cloudCount - 4) // Son 4 bulutu sakla
                {
                    targetPositions[i] = new Vector2(x, y);
                    continue;
                }
                
                // Son 4 bulutu üst kısma yerleştir
                float extraY = screenHeight / 2 + cellHeight / 2;
                float extraX = (i - (cloudCount - 4)) * (screenWidth / 4) - (screenWidth / 2) + (screenWidth / 8);
                targetPositions[i] = new Vector2(extraX, extraY);
            }
            else
            {
                targetPositions[i] = new Vector2(x, y);
            }
        }
    }
    
    public IEnumerator TransitionToScene(string sceneName)
    {
        Debug.Log("" + sceneName);
        if (isTransitioning) yield break;
        isTransitioning = true;
        Debug.Log("" + sceneName);
        
        // Activate and position clouds at screen edges
        foreach (Image cloud in clouds)
        {
            cloud.gameObject.SetActive(true);
            
            // Rastgele kenar seç (0: sol, 1: üst, 2: sağ, 3: alt)
            int edge = Random.Range(0, 4);
            Vector2 startPos = GetEdgePosition(edge);
            
            cloud.rectTransform.anchoredPosition = startPos;
            cloud.rectTransform.localScale = Vector3.one;
        }
        Debug.Log("" + sceneName);
        
        // Animate clouds coming in
        for (int i = 0; i < clouds.Length; i++)
        {
            clouds[i].rectTransform.DOAnchorPos(targetPositions[i], transitionDuration)
                .SetEase(Ease.InOutQuad);
        }
        
        yield return new WaitForSeconds(transitionDuration);
        
        // Load new scene
        SceneManager.LoadScene(sceneName);
        Debug.Log("" + sceneName);
        
        // Animate clouds going out
        foreach (Image cloud in clouds)
        {
            // Rastgele kenara doğru hareket
            int edge = Random.Range(0, 4);
            Vector2 endPos = GetEdgePosition(edge);
            
            cloud.rectTransform.DOAnchorPos(endPos, transitionDuration)
                .SetEase(Ease.InOutQuad);
        }
        Debug.Log("td" + transitionDuration);
        // yield return new WaitForSecondsRealtime(transitionDuration);
        
        
        // // Deactivate clouds
        // foreach (Image cloud in clouds)
        // {
        //     cloud.gameObject.SetActive(false);
        // }
        
        isTransitioning = false;
    }
    
    private Vector2 GetEdgePosition(int edge)
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        
        switch (edge)
        {
            case 0: // Sol
                return new Vector2(-cloudSpreadRadius, Random.Range(-screenHeight/2, screenHeight/2));
            case 1: // Üst
                return new Vector2(Random.Range(-screenWidth/2, screenWidth/2), screenHeight/2 + cloudSpreadRadius);
            case 2: // Sağ
                return new Vector2(screenWidth + cloudSpreadRadius, Random.Range(-screenHeight/2, screenHeight/2));
            case 3: // Alt
                return new Vector2(Random.Range(-screenWidth/2, screenWidth/2), -screenHeight/2 - cloudSpreadRadius);
            default:
                return Vector2.zero;
        }
    }
} 