#region
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Custom.Attributes;
using UnityEngine.SceneManagement;
using static GameManager;
#endregion

[DisallowMultipleComponent]
public class GameManager : SingletonPersistent<GameManager>
{
    HighScoreManager highScoreManager;
    ScoreManager scoreManager;

    [Header("UI Elements")]
    public GameObject mainMenuPanel;
    public GameObject pausePanel;
    public GameObject gameOverPanel;

    public bool isGameOver = false;


    #region Singleton
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("GameManager instance not found!");
            }
            return _instance;
        }
    }

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            Debug.LogError("Singleton already exist");
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    public GameState state;

    public enum GameState
    {
        Play,
        Pause,
        GameOver
        // Add more game states if needed
    }

    void Start()
    {
        mainMenuPanel.SetActive(true);
        gameOverPanel.SetActive(false);
        pausePanel.SetActive(false);
    }

    void Update()
    {
        switch (state)
        {
            case GameState.Play:
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    GameStateChanger(GameState.Pause);
                    pausePanel.SetActive(true);
                }
                break;
            case GameState.Pause:
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    GameStateChanger(GameState.Play);
                    pausePanel.SetActive(false);
                }
                break;
            case GameState.GameOver:
                if (!isGameOver && Input.GetKeyDown(KeyCode.K))
                {
                    isGameOver = true;
                    GameStateChanger(GameState.Pause);
                    gameOverPanel.SetActive(true);
                    Debug.Log("Game Over");
                }
                break;
        }
        // Add more cases if needed

        GameStateChanger(state);
    }

    public void GameStateChanger(GameState currentState)
    {
        state = currentState;

        switch (state)
        {
            case GameState.Play:
                Time.timeScale = 1f;
                break;
            case GameState.Pause:
                Time.timeScale = 0f;
                break;
            case GameState.GameOver:
                Time.timeScale = 0f;
                break;
        }
    }

    public void StartGame()
    {
        GameStateChanger(GameState.Play);
    }

    public void PauseGame()
    {
        GameStateChanger(GameState.Pause);
    }

    public void GameOver()
    {
        highScoreManager.SaveHighScores();
        GameStateChanger(GameState.GameOver);
    }
}
