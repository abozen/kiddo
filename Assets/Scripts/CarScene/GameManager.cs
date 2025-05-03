using System;
using UnityEngine;

public class CarGameManager : MonoBehaviour
{
    public enum GameState
    {
        Start,
        Playing,
        GameOver
    }
    
    [Header("Game Settings")]
    [SerializeField] private float scoreMultiplier = 1f;
    [SerializeField] private float difficultyIncreaseInterval = 30f; // Seconds between difficulty increases
    [SerializeField] private float maxDifficulty = 3f;
    
    [Header("References")]
    [SerializeField] private UIController uiController;
    [SerializeField] private PlayerCarController playerController;
    [SerializeField] private TrafficCarSpawner trafficSpawner;
    
    private GameState currentState = GameState.Playing;
    private float score = 0f;
    private float gameTimer = 0f;
    private float difficultyLevel = 1f;
    private float startTime;
    
    public static event Action<GameState> OnGameStateChanged;
    
    private void Start()
    {
        // Find references if not assigned
        if (uiController == null)
        {
            uiController = FindObjectOfType<UIController>();
        }
        
        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerCarController>();
        }
        
        if (trafficSpawner == null)
        {
            trafficSpawner = FindObjectOfType<TrafficCarSpawner>();
        }
        
        // Set initial game state
        ChangeGameState(GameState.Start);
    }
    
    private void Update()
    {
        if (currentState == GameState.Playing)
        {
            // Update score based on time or distance
            UpdateScore();
            
            // Update game timer
            gameTimer += Time.deltaTime;
            
            // Check if we should increase difficulty
            if (gameTimer >= difficultyIncreaseInterval)
            {
                IncreaseDifficulty();
                gameTimer = 0f;
            }
        }
    }
    
    private void UpdateScore()
    {
        // Calculate score based on time played and speed
        if (playerController != null)
        {
            float speedFactor = playerController.GetCurrentSpeed() / playerController.GetMaxSpeed();
            score += Time.deltaTime * scoreMultiplier * difficultyLevel * (1f + speedFactor);
            
            // Update UI
            if (uiController != null)
            {
                uiController.UpdateScore(Mathf.FloorToInt(score));
            }
        }
    }
    
    private void IncreaseDifficulty()
    {
        // Increase difficulty up to maximum
        difficultyLevel = Mathf.Min(difficultyLevel + 0.2f, maxDifficulty);
        
        // Adjust game parameters based on difficulty
        // This could be handled by events or direct references
    }
    
    public void StartGame()
    {
        if (currentState == GameState.Start || currentState == GameState.GameOver)
        {
            // Reset score and timer
            score = 0f;
            gameTimer = 0f;
            difficultyLevel = 1f;
            startTime = Time.time;
            
            // Change state to playing
            ChangeGameState(GameState.Playing);
        }
    }
    
    public void GameOver()
    {
        if (currentState == GameState.Playing)
        {
            ChangeGameState(GameState.GameOver);
            
            // Update UI with final score
            if (uiController != null)
            {
                uiController.ShowGameOver(Mathf.FloorToInt(score));
            }
        }
    }
    
    public void RestartGame()
    {
        // Reload the current scene or reset game state
        StartGame();
    }
    
    public void ChangeGameState(GameState newState)
    {
        currentState = newState;
        OnGameStateChanged?.Invoke(newState);
    }
    
    public bool IsGameActive()
    {
        return currentState == GameState.Playing;
    }
    
    public GameState GetCurrentState()
    {
        return currentState;
    }
    
    public int GetCurrentScore()
    {
        return Mathf.FloorToInt(score);
    }
    
    public float GetGameTime()
    {
        return Time.time - startTime;
    }
} 