using System;
using DG.Tweening;
using Lumina.Essentials.Modules;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    HighScoreManager highScoreManager;
    ScoreManager scoreManager;


    [Header("UI Elements")]
    [SerializeField] GameObject mainMenuPanel;
    public GameObject MainMenuPanel => mainMenuPanel;
    [SerializeField] GameObject pausePanel;
    public GameObject PausePanel => pausePanel;
    [SerializeField] GameObject gameOverPanel;
    public TextMeshProUGUI GameOverReasonText => gameOverReasonText;
    [SerializeField] TextMeshProUGUI gameOverReasonText;
    public GameObject GameOverPanel => gameOverPanel;
    [SerializeField] GameObject skipTutorialButton;
    public GameObject Gradient => gradient;
    [SerializeField] GameObject gradient;

    public GameObject SkipTutorialButton => skipTutorialButton;

    void Start()
    {
        DOTween.KillAll();
    }

    public void SetGameOverReason(string reason)
    {
        if (gameOverReasonText != null)
        {
            gameOverReasonText.text = reason;
        }
        else
        {
            Debug.LogWarning("GameOverReasonText is not assigned in the MainMenu!");
        }
    }

    public void StartTurtorialGame()
    {
        //Start Animation for tutorial
        Debug.Log("Starting the tutorial game...");
        GameManager.Instance.TriggerPlayIntro();
        mainMenuPanel.SetActive(false);
    }

    public void Exit()
    {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }

    public void SkipTutorial()
    {
        //Skips the tutorial button
        Debug.Log("Skipping the tutorial...");
        GameManager.Instance.hasPlayedIntro = true;
        StartGame();
    }

    public void Retry()
    {
        GameManager.Instance.hasPlayedIntro = true;
        GameManager.Instance.GameStateChanger(GameManager.GameState.Play);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        mainMenuPanel.SetActive(true);
        gameOverPanel.SetActive(false);
        pausePanel.SetActive(false);
        Debug.Log("Restarting the game...");
    }

    public void BackToMain()
    {
        Debug.Log("Returning to the main menu...");
        GameManager.Instance.GameStateChanger(GameManager.GameState.Menu);

        mainMenuPanel.SetActive(true);
        pausePanel.SetActive(false);
        gameOverPanel.SetActive(false);

        if (scoreManager == null)
            scoreManager = Helpers.Find<ScoreManager>();
        scoreManager.ResetGame();
    }

    public void StartGame()
    {
        GameManager.Instance.GameStateChanger(GameManager.GameState.Play);
        mainMenuPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        pausePanel.SetActive(false);
    }

    public void UnPauseGame()
    {
        GameManager.Instance.GameStateChanger(GameManager.GameState.Play);
        pausePanel.SetActive(false);
        gameOverPanel.SetActive(false);
        mainMenuPanel.SetActive(false);
    }

    public void GameOver()
    {
        if (highScoreManager == null)
            highScoreManager = Helpers.Find<HighScoreManager>();
        highScoreManager.SaveHighScores();

        GameManager.Instance.GameStateChanger(GameManager.GameState.GameOver);

        gameOverPanel.SetActive(true);
        pausePanel.SetActive(false);
        mainMenuPanel.SetActive(false);
    }
}
