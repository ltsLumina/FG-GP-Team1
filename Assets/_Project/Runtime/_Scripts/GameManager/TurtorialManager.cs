using UnityEngine;

public class TurtorialManager : MonoBehaviour
{
    public GameObject pausePanel;

    public GameState state;

    public enum GameState
    {
        Play,
        Pause,
        Movement,
        Pickup,
        Fuel,
        Repair,
        Recharge,
        TurtorialDone,


        // Add more game states if needed
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
            case GameState.Movement:
                // Player try movement
                break;
            case GameState.Pickup:
                // Player try pickup
                break;
            case GameState.Fuel:
                // Player try fuel
                break;
            case GameState.Repair:
                // Player try repair
                break;
            case GameState.Recharge:
                // Player try recharge
                break;
            case GameState.TurtorialDone:
                // Change scene
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
            case GameState.Movement: 
                break;
            case GameState.Pickup:
                break;
            case GameState.Fuel:
                break;
            case GameState.Repair:
                break;
            case GameState.Recharge:
                break;
            case GameState.TurtorialDone:
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

    public void TurtorialMovement()
    {
        GameStateChanger(GameState.Movement);
    }

    public void TurtorialPickup()
    {
        GameStateChanger(GameState.Pickup);
    }

    public void TurtorialFuel()
    {
        GameStateChanger(GameState.Fuel);
    }

    public void TurtorialRepair()
    {
        GameStateChanger(GameState.Repair);
    }

    public void TurtorialRecharge()
    {
        GameStateChanger(GameState.Recharge);
    }

    public void TurtorialDone()
    {
        GameStateChanger(GameState.TurtorialDone);
    }
}