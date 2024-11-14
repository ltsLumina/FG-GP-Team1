
using System;
using System.Collections.Generic;
using DG.Tweening;
using Lumina.Essentials.Modules;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    HighScoreManager highScoreManager;

    [Header("UI Elements")]
    [SerializeField]
    GameObject mainMenuPanel;
    public GameObject MainMenuPanel => mainMenuPanel;

    [SerializeField]
    GameObject pausePanel;
    public GameObject PausePanel => pausePanel;

    [SerializeField]
    GameObject gameOverPanel;
    public TextMeshProUGUI GameOverReasonText => gameOverReasonText;

    [SerializeField]
    TextMeshProUGUI gameOverReasonText;
    public GameObject GameOverPanel => gameOverPanel;

    [SerializeField]
    GameObject skipTutorialButton;


    [SerializeField]
    GameObject gradient;
    public GameObject Gradient => gradient;

    [SerializeField]
    GameObject scoreUI;
    public GameObject ScoreUI => scoreUI;

    [Header("Pause UIs")]
    [SerializeField]
    List<GameObject> pauseUIs;
    public List<GameObject> PauseUIs => pauseUIs;


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

    public void OnBackButtonPressed()
    {
        switch (GameManager.Instance.state)
        {
            case GameManager.GameState.Menu:
                Debug.Log("Back button pressed in Menu state");
                mainMenuPanel.SetActive(true);
                pausePanel.SetActive(false);
                break;

            case GameManager.GameState.Pause:
                Debug.Log("Back button pressed in Play state");
                pausePanel.SetActive(true);
                mainMenuPanel.SetActive(false);
                GameManager.Instance.GameStateChanger(GameManager.GameState.Pause);
                break;

            default:
                Debug.LogWarning("Back button pressed in unexpected state: " + GameManager.Instance.state);
                break;
        }
    }

    public void StartTurtorialGame()
    {
        //Start Animation for tutorial
        Debug.Log("Starting the tutorial game...");
        GameManager.Instance.TriggerPlayIntro();
        mainMenuPanel.SetActive(false);
    }

    public void SetPauseUIsActive(bool isActive)
    {
        foreach (GameObject ui in pauseUIs)
        {
            if (ui != null)
            {
                ui.SetActive(isActive);
            }
        }
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
        Debug.Log("Retrying the game...");
        GameManager.Instance.hasPlayedIntro = true;
        GameManager.Instance.GameStateChanger(GameManager.GameState.Play);
        GameManager.Instance.isGoingToMainMenu = false;
        GameManager.Instance.isGameOver = false;
        gameOverPanel.SetActive(false);
        pausePanel.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Debug.Log("Restarting the game...");
    }

    public void BackToMain()
    {
        Debug.Log("Returning to the main menu...");
        GameManager.Instance.GameStateChanger(GameManager.GameState.Menu);
        GameManager.Instance.hasPlayedIntro = true;
        GameManager.Instance.isGoingToMainMenu = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        SetPauseUIsActive(false);
        gameOverPanel.SetActive(false);
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