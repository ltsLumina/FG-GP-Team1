using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    HighScoreManager highScoreManager;
    ScoreManager scoreManager;

    public void StartGame()
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
        highScoreManager.SaveHighScores();
        SceneManager.LoadScene("Birkan Sandbox");
    }
}
