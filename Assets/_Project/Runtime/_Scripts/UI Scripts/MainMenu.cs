using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    void Start()
    {

    }

    void Update()
    {

    }

    public void Exit()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Sandbox"); // Replace later with what scene is the game scene
    }

    void ShowCredits()
    {

    }

    void HighScore()
    {

    }
}
