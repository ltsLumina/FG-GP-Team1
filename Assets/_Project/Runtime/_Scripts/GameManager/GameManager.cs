#region
using System;
using UnityEngine;
#endregion

[DisallowMultipleComponent]
public class GameManager : SingletonPersistent<GameManager>
{
    MainMenu mainMenu;

    public bool isGameOver;
    public bool isIntroPlayed;
    public bool hasSeenKelp;
    public bool hasSeenRock;
    public bool hasSeenJellyfish;
    public bool hasLowFuelWarning;
    public bool hasTakenHullDamage;
    public bool hasLowBatteryWarning;
    // Add more variables as needed

    public GameState state;

    public enum GameState
    {
        Play,
        Pause,
        GameOver
        // Add more game states if needed
    }

    // Events
    public event Action OnIntro;
    public event Action OnFirstKelp;
    public event Action OnLowFuel;
    public event Action OnFirstRock;
    public event Action OnCriticalHull;
    public event Action OnFirstJellyfish;
    public event Action OnLightOut;
    public event Action OnLowBattery;
    public event Action OnPlayerDeath;
    public event Action OnFirstHullDamage;
    // Add more events as needed

    void Start()
    {
        if (!isIntroPlayed)
        {
            isIntroPlayed = true;
            OnIntro?.Invoke();
        }
    }

    void Update()
    {
        CheckVisibilityForKelp();
        CheckVisibilityForRocks();
        CheckVisibilityForJellyfish();

        switch (state)
        {
            case GameState.Play:
                isGameOver = false;
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    GameStateChanger(GameState.Pause);
                    mainMenu = FindAnyObjectByType<MainMenu>();
                    mainMenu.PausePanel.SetActive(true);
                    mainMenu.SkipTutorialButton.SetActive(false);
                }
                break;
            case GameState.Pause:
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    GameStateChanger(GameState.Play);
                    mainMenu = FindAnyObjectByType<MainMenu>();
                    mainMenu.PausePanel.SetActive(false);
                    mainMenu.SkipTutorialButton.SetActive(true);
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

    // First time seeing kelp
    public void TriggerFirstKelp()
    {
        if (!hasSeenKelp)
        {
            hasSeenKelp = true;
            OnFirstKelp?.Invoke();
        }
    }

    void CheckVisibilityForKelp()
    {
        if (!hasSeenKelp && IsObjectInView("Kelp"))
        {
            hasSeenKelp = true;
            OnFirstKelp?.Invoke();
            Debug.Log("First kelp seen! Player notified.");
        }
    }

    // First time seeing rock
    public void TriggerFirstRock()
    {
        if (!hasSeenRock)
        {
            hasSeenRock = true;
            OnFirstRock?.Invoke();
        }
    }

    void CheckVisibilityForRocks()
    {
        if (!hasSeenRock && IsObjectInView("Rock"))
        {
            hasSeenRock = true;
            OnFirstRock?.Invoke();
            Debug.Log("First rock seen! Player notified.");
        }
    }

    // First time seeing jellyfish
    public void TriggerFirstJellyfish()
    {
        if (!hasSeenJellyfish)
        {
            hasSeenJellyfish = true;
            OnFirstJellyfish?.Invoke();
        }
    }

    void CheckVisibilityForJellyfish()
    {
        if (!hasSeenJellyfish && IsObjectInView("Jellyfish"))
        {
            hasSeenJellyfish = true;
            OnFirstJellyfish?.Invoke();
            Debug.Log("First jellyfish seen! Player notified.");
        }
    }

    // First time hull damage
    public void TriggerFirstHullDamage()
    {
        // TODO: Has to be called everytime hull damage is taken
        if (!hasTakenHullDamage)
        {
            hasTakenHullDamage = true;
            OnFirstHullDamage?.Invoke();
        }
    }

    // Hull on 1 HP
    public void TriggerCriticalHull(int hullHP)
    {
        if (hullHP == 1)
        {
            OnCriticalHull?.Invoke();
        }
    }

    // Fuel below 20%
    public void TriggerLowFuel(float fuelPercentage)
    {
        if (fuelPercentage < 20 && !hasLowFuelWarning)
        {
            hasLowFuelWarning = true;
            OnLowFuel?.Invoke();
            Debug.Log("Low fuel event triggered.");
        }
    }

    // Battery below 20%
    public void TriggerLowBattery(float batteryPercentage)
    {
        if (batteryPercentage < 20 && !hasLowBatteryWarning)
        {
            hasLowBatteryWarning = true;
            OnLowBattery?.Invoke();
        }
    }

    // Light out
    public void TriggerLightOut()
    {
        OnLightOut?.Invoke();
    }

    // Player died
    public void TriggerPlayerDeath(string reason)
    {
        Debug.Log($"Player died due to: {reason}");
        OnPlayerDeath?.Invoke();
    }

    bool IsObjectInView(string tag)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
        Camera mainCamera = Camera.main;

        foreach (GameObject obj in objects)
        {
            Vector3 screenPoint = mainCamera.WorldToViewportPoint(obj.transform.position);
            bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;

            if (onScreen) return true;
        }

        return false;
    }
}
