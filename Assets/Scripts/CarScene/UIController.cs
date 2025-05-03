using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject gameplayPanel;
    [SerializeField] private GameObject gameOverPanel;
    
    [Header("UI Elements - Gameplay")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private Slider speedometerSlider;
    [SerializeField] private Button accelerateButton;
    [SerializeField] private Button brakeButton;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    
    [Header("UI Elements - Game Over")]
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;
    
    [Header("References")]
    [SerializeField] private CarGameManager gameManager;
    [SerializeField] private PlayerCarController playerController;
    
    private void Start()
    {
        // Find references if not assigned
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<CarGameManager>();
        }
        
        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerCarController>();
        }
        
        // Set up button listeners
        SetupButtons();
        
        // Subscribe to game state changes
        CarGameManager.OnGameStateChanged += HandleGameStateChanged;
        
        // Show initial UI
        HandleGameStateChanged(gameManager != null ? gameManager.GetCurrentState() : CarGameManager.GameState.Start);
    }
    
    private void Update()
    {
        // Update speed display
        if (gameManager != null && gameManager.IsGameActive() && playerController != null)
        {
            UpdateSpeedDisplay();
        }
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from events
        CarGameManager.OnGameStateChanged -= HandleGameStateChanged;
    }
    
    private void SetupButtons()
    {
        // Start panel
        if (startPanel != null)
        {
            Button startButton = startPanel.GetComponentInChildren<Button>();
            if (startButton != null)
            {
                startButton.onClick.AddListener(OnStartButtonClicked);
            }
        }
        
        // Game Over panel
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(OnRestartButtonClicked);
        }
        
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
        }
        
        // Gameplay controls
        if (accelerateButton != null)
        {
            accelerateButton.onClick.AddListener(playerController.Accelerate);
        }
        
        if (brakeButton != null)
        {
            brakeButton.onClick.AddListener(playerController.Brake);
        }
        
        if (leftButton != null)
        {
            leftButton.onClick.AddListener(playerController.MoveLeft);
        }
        
        if (rightButton != null)
        {
            rightButton.onClick.AddListener(playerController.MoveRight);
        }
    }
    
    private void HandleGameStateChanged(CarGameManager.GameState newState)
    {
        // Show/hide panels based on game state
        if (startPanel != null) startPanel.SetActive(newState == CarGameManager.GameState.Start);
        if (gameplayPanel != null) gameplayPanel.SetActive(newState == CarGameManager.GameState.Playing);
        if (gameOverPanel != null) gameOverPanel.SetActive(newState == CarGameManager.GameState.GameOver);
    }
    
    public void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }
    }
    
    private void UpdateSpeedDisplay()
    {
        float currentSpeed = playerController.GetCurrentSpeed();
        float maxSpeed = playerController.GetMaxSpeed();
        
        // Update speed text
        if (speedText != null)
        {
            int speedKmh = Mathf.RoundToInt(currentSpeed * 3.6f); // Convert to km/h
            speedText.text = $"{speedKmh} km/h";
        }
        
        // Update speedometer slider
        if (speedometerSlider != null)
        {
            speedometerSlider.value = currentSpeed / maxSpeed;
        }
    }
    
    public void ShowGameOver(int finalScore)
    {
        if (finalScoreText != null)
        {
            finalScoreText.text = $"Final Score: {finalScore}";
        }
    }
    
    private void OnStartButtonClicked()
    {
        if (gameManager != null)
        {
            gameManager.StartGame();
        }
    }
    
    private void OnRestartButtonClicked()
    {
        if (gameManager != null)
        {
            gameManager.RestartGame();
        }
    }
    
    private void OnMainMenuButtonClicked()
    {
        if (gameManager != null)
        {
            // Return to main menu - alternatively could reload the scene
            gameManager.ChangeGameState(CarGameManager.GameState.Start);
        }
    }
} 