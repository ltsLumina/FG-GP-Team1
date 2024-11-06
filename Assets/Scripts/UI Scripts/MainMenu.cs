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

    public void StartGame()
    {
        // Replace later with what scene is the game scene
        SceneManager.LoadScene("Birkan Sandbox");
    }

    public void Exit()
    {
        Application.Quit();
    }

    void Death()
    {

    }

    void ShowCredits()
    {

    }

    void HighScore()
    {

    }
}
