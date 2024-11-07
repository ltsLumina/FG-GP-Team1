#region
using UnityEngine;
#endregion

[DisallowMultipleComponent]
public class GameManager : SingletonPersistent<GameManager>
{
    MainMenu mainMenu;
    public bool isGameOver = false;

    public GameState state;

    public enum GameState
    {
        Play,
        Pause,
        GameOver
        // Add more game states if needed
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            GameStateChanger(GameState.GameOver);
        }

        switch (state)
        {

            case GameState.Play:
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
                break;
        }
    }
}
