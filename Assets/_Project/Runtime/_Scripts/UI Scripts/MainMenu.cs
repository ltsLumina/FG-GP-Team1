using System;
using DG.Tweening;
using Lumina.Essentials.Modules;
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
    public GameObject GameOverPanel => gameOverPanel;
    [SerializeField] GameObject skipTutorialButton;
    public GameObject Gradient => gradient;
    [SerializeField] GameObject gradient;

    public GameObject SkipTutorialButton => skipTutorialButton;

    void Start()
    {
        DOTween.KillAll();
    }

    public void StartTurtorialGame()
    {
        // Replace later with what scene is the game scene
        SceneManager.LoadScene("GoldBuild");
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void Retry()
    {
        GameManager.Instance.GameStateChanger(GameManager.GameState.Play);
        highScoreManager = Helpers.Find<HighScoreManager>();
        highScoreManager.SaveHighScores();
        scoreManager = Helpers.Find<ScoreManager>();
        scoreManager.ResetGame();
        SceneManager.LoadScene("GoldBuild");
    }

    public void BackToMain()
    {
        GameManager.Instance.GameStateChanger(GameManager.GameState.Play);
        SceneManager.LoadScene(0);
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
        highScoreManager.SaveHighScores();
        GameManager.Instance.GameStateChanger(GameManager.GameState.GameOver);
        gameOverPanel.SetActive(false);
        pausePanel.SetActive(false);
        mainMenuPanel.SetActive(false);
    }
}
