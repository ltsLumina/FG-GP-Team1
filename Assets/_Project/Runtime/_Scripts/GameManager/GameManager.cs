#region
using Lumina.Essentials.Attributes;
using UnityEngine;
#endregion

[DisallowMultipleComponent]
public class GameManager : SingletonPersistent<GameManager>
{
    public enum GameState
    {
        Play,
        Pause,
        GameOver
    }
    
    [SerializeField, ReadOnly] bool isGameOver;
    [SerializeField, ReadOnly] GameState state;

    MainMenu mainMenu;

    void Update()
    {
        switch (state)
        {
            case GameState.Play:
                isGameOver = false;
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    GameStateChanger(GameState.Pause);
                    mainMenu = FindAnyObjectByType<MainMenu>();
                    mainMenu.PausePanel.SetActive(true);
                }
                break;
            case GameState.Pause:
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    GameStateChanger(GameState.Play);
                    mainMenu = FindAnyObjectByType<MainMenu>();
                    mainMenu.PausePanel.SetActive(false);
                }
                break;
            case GameState.GameOver:
                if (!isGameOver)
                {
                    isGameOver = true;
                    GameStateChanger(GameState.Pause);
                    mainMenu = FindAnyObjectByType<MainMenu>();
                    mainMenu.GameOverPanel.SetActive(true);
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
                // mainMenu = FindAnyObjectByType<MainMenu>();
                // mainMenu.GameOverPanel.SetActive(true);
                break;
        }
    }
}
