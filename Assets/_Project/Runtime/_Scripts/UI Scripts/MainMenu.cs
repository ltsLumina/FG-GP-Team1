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

    public void StartTurtorialGame()
    {
        // Replace later with what scene is the game scene
        SceneManager.LoadScene("Turtorial");
    }

    public void Exit()
    {
        UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }

    public void SkipTutorial()
    {
        SceneManager.LoadScene("Birkan Sandbox");
    }

    public void Retry()
    {
        GameManager.Instance.GameStateChanger(GameManager.GameState.Play);
        SceneManager.LoadScene("Birkan Sandbox");
        highScoreManager.SaveHighScores();
        scoreManager.ResetGame();
    }

    public void BackToMain()
    {
        SceneManager.LoadScene("Main Menu");
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
