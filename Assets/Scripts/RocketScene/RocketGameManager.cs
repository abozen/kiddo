using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI gemsText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private TextMeshProUGUI finalGemsText;
    [SerializeField] private TextMeshProUGUI distanceText;
    [SerializeField] private Transform player;
    [SerializeField] private Image[] bulletImages; // UI'daki bullet görselleri
    [SerializeField] private Color bulletActiveColor = Color.white;
    [SerializeField] private Color bulletInactiveColor = new Color(1, 1, 1, 0.3f);
    
    private bool gameOver = false;
    private int gems = 0;
    private int bulletCount = 3; // Başlangıç bullet sayısı
    private float initialPlayerZ;
    private float distanceTraveled;
    
    private void Start()
    {
        // Initialize game
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
            
        gems = 0;
        bulletCount = 3;
        gameOver = false;
        
        // Save initial player position
        if (player != null)
            initialPlayerZ = player.position.z;
            
        // Update UI
        UpdateGemsUI();
        UpdateHighScoreUI();
        UpdateBulletUI();
    }
    
    private void Update()
    {
        if (gameOver)
            return;
            
        // Update distance traveled
        if (player != null)
        {
            distanceTraveled = player.position.z - initialPlayerZ;
            
            // Update distance UI
            if (distanceText != null)
            {
                distanceText.text = $"Distance: {distanceTraveled:0}m";
            }
        }
    }
    
    public void AddGem()
    {
        gems++;
        UpdateGemsUI();
    }
    
    public void AddBullet()
    {
        if (bulletCount < 3)
        {
            bulletCount++;
            UpdateBulletUI();
        }
    }
    
    public bool UseBullet()
    {
        if (bulletCount > 0)
        {
            bulletCount--;
            UpdateBulletUI();
            return true;
        }
        return false;
    }
    
    public int GetBulletCount()
    {
        return bulletCount;
    }
    
    public bool IsGameOver()
    {
        return gameOver;
    }
    
    public void GameOver()
    {
        if (gameOver)
            return;
            
        gameOver = true;
        
        // Show game over panel
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            
            // Update final gems display
            if (finalGemsText != null)
            {
                finalGemsText.text = $"Gems Collected: {gems}";
            }
        }
        
        // Update high score if needed
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        if (gems > highScore)
        {
            PlayerPrefs.SetInt("HighScore", gems);
        }
        
        // Update total gems
        int currentTotalGems = PlayerPrefs.GetInt("TotalGems", 0);
        PlayerPrefs.SetInt("TotalGems", currentTotalGems + gems);
        PlayerPrefs.Save();
        
        UpdateHighScoreUI();
    }
    
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    private void UpdateGemsUI()
    {
        if (gemsText != null)
        {
            gemsText.text = $"Gems: {gems}";
        }
    }
    
    private void UpdateHighScoreUI()
    {
        if (highScoreText != null)
        {
            int highScore = PlayerPrefs.GetInt("HighScore", 0);
            highScoreText.text = $"High Score: {highScore}";
        }
    }
    
    private void UpdateBulletUI()
    {
        if (bulletImages != null)
        {
            for (int i = 0; i < bulletImages.Length; i++)
            {
                if (bulletImages[i] != null)
                {
                    bulletImages[i].color = i < bulletCount ? bulletActiveColor : bulletInactiveColor;
                }
            }
        }
    }
}