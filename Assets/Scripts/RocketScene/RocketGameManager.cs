using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI distanceText;
    [SerializeField] private Transform player;
    
    private bool gameOver = false;
    private int score = 0;
    private float initialPlayerZ;
    private float distanceTraveled;
    
    private void Start()
    {
        // Initialize game
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
            
        score = 0;
        gameOver = false;
        
        // Save initial player position
        if (player != null)
            initialPlayerZ = player.position.z;
            
        // Update UI
        UpdateScoreUI();
        UpdateHighScoreUI();
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
            
            // Add score based on distance (every 50 meters)
            if (distanceTraveled > 0 && distanceTraveled % 50 < 0.1f)
            {
                AddScore(5);
            }
        }
    }
    
    public void AddScore(int points)
    {
        score += points;
        UpdateScoreUI();
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
            
            // Update final score display
            if (finalScoreText != null)
            {
                finalScoreText.text = $"Final Score: {score}";
            }
        }
        
        // Update high score if needed
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        if (score > highScore)
        {
            PlayerPrefs.SetInt("HighScore", score);
            PlayerPrefs.Save();
            UpdateHighScoreUI();
        }
    }
    
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
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
}