using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
    [SerializeField] private PlayerCollisioonHandler collisionHandler;
    
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
        
        if (collisionHandler == null)
        {
            collisionHandler = FindObjectOfType<PlayerCollisioonHandler>();
        }
        
        // Set up button listeners
        SetupButtons();
        
        // Subscribe to game state changes
        CarGameManager.OnGameStateChanged += HandleGameStateChanged;
        
        // Show initial UI
        HandleGameStateChanged(gameManager != null ? gameManager.GetCurrentState() : CarGameManager.GameState.Start);
        
        // Initialize UI
        UpdateScore(0);
    }
    
    private void Update()
    {
        // Update speed display
        if (gameManager != null && gameManager.IsGameActive() && playerController != null)
        {
            UpdateSpeedDisplay();
        }
        
        if (collisionHandler != null && scoreText != null)
        {
            int gemsCount = collisionHandler.GetGemsCount();
            scoreText.text = $"Gems: {gemsCount}";
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
        
        // Setup accelerate button
        if (accelerateButton != null)
        {
            // Add event trigger component if it doesn't exist
            EventTrigger accTrigger = accelerateButton.gameObject.GetComponent<EventTrigger>() ?? 
                                      accelerateButton.gameObject.AddComponent<EventTrigger>();
            
            // Clear any existing triggers
            accTrigger.triggers.Clear();
                                      
            // Add pointer down event
            EventTrigger.Entry accDownEntry = new EventTrigger.Entry();
            accDownEntry.eventID = EventTriggerType.PointerDown;
            accDownEntry.callback.AddListener((data) => { OnAccelerateButtonDown(); });
            accTrigger.triggers.Add(accDownEntry);
            
            // Add pointer up event
            EventTrigger.Entry accUpEntry = new EventTrigger.Entry();
            accUpEntry.eventID = EventTriggerType.PointerUp;
            accUpEntry.callback.AddListener((data) => { OnAccelerateButtonUp(); });
            accTrigger.triggers.Add(accUpEntry);
            
            // Add pointer exit event (if finger moves off button while pressing)
            EventTrigger.Entry accExitEntry = new EventTrigger.Entry();
            accExitEntry.eventID = EventTriggerType.PointerExit;
            accExitEntry.callback.AddListener((data) => { OnAccelerateButtonUp(); });
            accTrigger.triggers.Add(accExitEntry);
            
            // Remove any existing click listeners to avoid conflicts
            accelerateButton.onClick.RemoveAllListeners();
        }
        
        // Setup brake button
        if (brakeButton != null)
        {
            EventTrigger brakeTrigger = brakeButton.gameObject.GetComponent<EventTrigger>() ?? 
                                       brakeButton.gameObject.AddComponent<EventTrigger>();
            
            // Clear any existing triggers
            brakeTrigger.triggers.Clear();
                                       
            // Add pointer down event
            EventTrigger.Entry brakeDownEntry = new EventTrigger.Entry();
            brakeDownEntry.eventID = EventTriggerType.PointerDown;
            brakeDownEntry.callback.AddListener((data) => { OnBrakeButtonDown(); });
            brakeTrigger.triggers.Add(brakeDownEntry);
            
            // Add pointer up event
            EventTrigger.Entry brakeUpEntry = new EventTrigger.Entry();
            brakeUpEntry.eventID = EventTriggerType.PointerUp;
            brakeUpEntry.callback.AddListener((data) => { OnBrakeButtonUp(); });
            brakeTrigger.triggers.Add(brakeUpEntry);
            
            // Add pointer exit event
            EventTrigger.Entry brakeExitEntry = new EventTrigger.Entry();
            brakeExitEntry.eventID = EventTriggerType.PointerExit;
            brakeExitEntry.callback.AddListener((data) => { OnBrakeButtonUp(); });
            brakeTrigger.triggers.Add(brakeExitEntry);
            
            brakeButton.onClick.RemoveAllListeners();
        }
        
        // Setup left button
        if (leftButton != null)
        {
            EventTrigger leftTrigger = leftButton.gameObject.GetComponent<EventTrigger>() ?? 
                                      leftButton.gameObject.AddComponent<EventTrigger>();
            
            // Clear any existing triggers
            leftTrigger.triggers.Clear();
                                      
            // Add pointer down event
            EventTrigger.Entry leftDownEntry = new EventTrigger.Entry();
            leftDownEntry.eventID = EventTriggerType.PointerDown;
            leftDownEntry.callback.AddListener((data) => { OnLeftButtonDown(); });
            leftTrigger.triggers.Add(leftDownEntry);
            
            // Add pointer up event
            EventTrigger.Entry leftUpEntry = new EventTrigger.Entry();
            leftUpEntry.eventID = EventTriggerType.PointerUp;
            leftUpEntry.callback.AddListener((data) => { OnLeftButtonUp(); });
            leftTrigger.triggers.Add(leftUpEntry);
            
            // Add pointer exit event
            EventTrigger.Entry leftExitEntry = new EventTrigger.Entry();
            leftExitEntry.eventID = EventTriggerType.PointerExit;
            leftExitEntry.callback.AddListener((data) => { OnLeftButtonUp(); });
            leftTrigger.triggers.Add(leftExitEntry);
            
            leftButton.onClick.RemoveAllListeners();
        }
        
        // Setup right button
        if (rightButton != null)
        {
            EventTrigger rightTrigger = rightButton.gameObject.GetComponent<EventTrigger>() ?? 
                                       rightButton.gameObject.AddComponent<EventTrigger>();
            
            // Clear any existing triggers
            rightTrigger.triggers.Clear();
                                       
            // Add pointer down event
            EventTrigger.Entry rightDownEntry = new EventTrigger.Entry();
            rightDownEntry.eventID = EventTriggerType.PointerDown;
            rightDownEntry.callback.AddListener((data) => { OnRightButtonDown(); });
            rightTrigger.triggers.Add(rightDownEntry);
            
            // Add pointer up event
            EventTrigger.Entry rightUpEntry = new EventTrigger.Entry();
            rightUpEntry.eventID = EventTriggerType.PointerUp;
            rightUpEntry.callback.AddListener((data) => { OnRightButtonUp(); });
            rightTrigger.triggers.Add(rightUpEntry);
            
            // Add pointer exit event
            EventTrigger.Entry rightExitEntry = new EventTrigger.Entry();
            rightExitEntry.eventID = EventTriggerType.PointerExit;
            rightExitEntry.callback.AddListener((data) => { OnRightButtonUp(); });
            rightTrigger.triggers.Add(rightExitEntry);
            
            rightButton.onClick.RemoveAllListeners();
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
            scoreText.text = $"Gems: {score}";
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
    
    private void OnAccelerateButtonDown()
    {
        if (playerController != null)
        {
            playerController.SetAccelerate(true);
        }
    }
    
    private void OnAccelerateButtonUp()
    {
        if (playerController != null)
        {
            playerController.SetAccelerate(false);
        }
    }
    
    private void OnBrakeButtonDown()
    {
        if (playerController != null)
        {
            playerController.SetBrake(true);
        }
    }
    
    private void OnBrakeButtonUp()
    {
        if (playerController != null)
        {
            playerController.SetBrake(false);
        }
    }
    
    private void OnLeftButtonDown()
    {
        if (playerController != null)
        {
            playerController.SetMoveLeft(true);
        }
    }
    
    private void OnLeftButtonUp()
    {
        if (playerController != null)
        {
            playerController.SetMoveLeft(false);
        }
    }
    
    private void OnRightButtonDown()
    {
        if (playerController != null)
        {
            playerController.SetMoveRight(true);
        }
    }
    
    private void OnRightButtonUp()
    {
        if (playerController != null)
        {
            playerController.SetMoveRight(false);
        }
    }
} 