#region
using System;
using TMPro;
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
    public bool hasChargedBattery;
    public bool hasRepairedHull;
    public bool hasRefilledFuel;
    public bool hasBeenStunned;
    public bool hasEnteredTheDeep;
    public bool hasLightOut;
    public bool hasHadCriticalHull;
    public bool hasHadCriticalFuel;
    public bool hasHadCriticalBattery;
    public bool hasHadPressureEvent;
    public bool hasPlayedIntro;
    public bool hasPlayedFirstPlay;

    public Scanner ShipScanner;

    [SerializeField]
    TextMeshProUGUI highscoreText;

    public float highScore;
    public float currentDepth;
    private readonly float initialDepth = 20f;

    [SerializeField]
    GameObject ship;

    // Add more variables as needed

    public GameState state;

    public enum GameState
    {
        Play,
        Pause,
        GameOver,
        Menu,
    }

    // Events
    public event Action OnIntro;
    public event Action OnPlay;
    public event Action<GameObject> OnFirstKelp;
    public event Action OnLowFuel;
    public event Action<GameObject> OnFirstRock;
    public event Action OnCriticalHull;
    public event Action<GameObject> OnFirstJellyfish;
    public event Action OnLightOut;
    public event Action OnLowBattery;
    public event Action OnHullDamage;
    public event Action<string> OnGameOver;
    public event Action OnPressureEvent;
    public event Action OnPlayerStun;
    public event Action OnEnterTheDeep;
    public event Action OnHullRepair;
    public event Action OnFuelRefill;
    public event Action OnBatteryCharge;

    public bool isGoingToMainMenu;

    [SerializeField]
    GameAnimation gameAnimation;

    // Add more events as needed

    void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        // Find referecnes

        if (gameAnimation == null)
        {
            gameAnimation = GameObject
                .FindGameObjectsWithTag("GameAnimation")[0]
                .GetComponent<GameAnimation>();

            if (gameAnimation == null)
            {
                Debug.LogError("GameAnimation not found!");
            }
        }

        if (ship == null)
        {
            //ship = FindObjectByTag("Player");
            ship = GameObject.FindGameObjectWithTag("Ship");

            // Debug
            if (ship == null)
            {
                Debug.LogError("Ship not found!");
            }
        }

        if (ShipScanner == null)
        {
            ShipScanner = ship.GetComponentInChildren<Scanner>();
            if (ShipScanner == null)
            {
                Debug.LogError("ShipScanner not found!");
            }
        }
        currentDepth = 0;
        isGameOver = false;

        // Play the right animation
        if (hasPlayedIntro)
        {
            if (isGoingToMainMenu)
            {
                gameAnimation.MainMenu();
            }

            gameAnimation.Replay();
        }
        else
        {
            gameAnimation.Intro();
        }
    }

    public void TriggerPlayIntro()
    {
        if (!isIntroPlayed)
        {
            isIntroPlayed = true;
            OnIntro?.Invoke();
        }
    }

    // Update is called once per frame

    void Update()
    {
        highscoreText.text = $"High Score: {1 * -Mathf.RoundToInt(currentDepth)}m";

        CheckVisibilityForKelp();
        CheckVisibilityForRocks();
        CheckVisibilityForJellyfish();

        UpdateDepth();

        switch (state)
        {
            case GameState.Play:
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    GameStateChanger(GameState.Pause);
                    mainMenu = FindAnyObjectByType<MainMenu>();
                    mainMenu.Gradient.SetActive(true);
                    mainMenu.PausePanel.SetActive(true);
                    //mainMenu.SkipTutorialButton.SetActive(false);
                }
                break;
            case GameState.Pause:
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    GameStateChanger(GameState.Play);
                    mainMenu = FindAnyObjectByType<MainMenu>();
                    mainMenu.Gradient.SetActive(false);
                    mainMenu.PausePanel.SetActive(false);
                    //mainMenu.SkipTutorialButton.SetActive(true);
                }
                break;
            case GameState.GameOver:
                if (!isGameOver)
                {
                    isGameOver = true;
                    GameStateChanger(GameState.Pause);
                    mainMenu = FindAnyObjectByType<MainMenu>();
                    mainMenu.Gradient.SetActive(true);
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
                Time.timeScale = 0f;
                break;
        }
    }

    void UpdateDepth()
    {
        currentDepth = ship.transform.position.y * -1 - initialDepth;

        if (currentDepth > highScore)
        {
            highScore = currentDepth;
        }
    }

    void CheckVisibilityForKelp()
    {
        if (!hasSeenKelp)
        {
            GameObject seenObj = IsObjectInView("Kelp");
            if (seenObj != null)
            {
                hasSeenKelp = true;
                OnFirstKelp?.Invoke(seenObj);
                Debug.Log("First kelp seen! Player notified.");
            }
        }
    }

    void CheckVisibilityForRocks()
    {
        if (!hasSeenRock)
        {
            GameObject seenObj = IsObjectInView("Rock");
            if (seenObj != null)
            {
                hasSeenRock = true;
                OnFirstRock?.Invoke(seenObj);
                Debug.Log("First rock seen! Player notified.");
            }
        }
    }

    void CheckVisibilityForJellyfish()
    {
        if (!hasSeenJellyfish)
        {
            GameObject seenObj = IsObjectInView("Jellyfish");
            if (seenObj != null)
            {
                hasSeenJellyfish = true;
                OnFirstJellyfish?.Invoke(seenObj);
                Debug.Log("First jellyfish seen! Player notified.");
            }
        }
    }


    // First time hull damage
    public void TriggerHullDamage()
    {
        if (!hasTakenHullDamage)
        {
            OnHullDamage?.Invoke();
            hasTakenHullDamage = true;
        }
    }

    public void TriggerPlayerStun()
    {
        if (!hasBeenStunned)
        {
            OnPlayerStun?.Invoke();
            hasBeenStunned = true;
        }
    }

    // Hull on 1 hit left.
    public void TriggerCriticalHull()
    {
        if (!hasHadCriticalHull)
        {
            OnCriticalHull?.Invoke();
            hasHadCriticalHull = true;
        }
    }

    public void TriggerPressureEvent()
    {
        if (!hasHadPressureEvent)
        {
            OnPressureEvent?.Invoke();
            hasHadPressureEvent = true;
        }
    }

    // Fuel below 20%
    public void TriggerLowFuel()
    {
        if (!hasHadCriticalFuel)
        {
            OnLowFuel?.Invoke();
            hasHadCriticalFuel = true;
        }
    }

    // Battery below 20%
    public void TriggerLowBattery(float batteryPercentage)
    {
        if (!hasHadCriticalBattery)
        {
            OnLowBattery?.Invoke();
            hasHadCriticalBattery = true;
        }
    }

    // Light out
    public void TriggerLightOut()
    {
        if (!hasLightOut)
        {
            OnLightOut?.Invoke();
            hasLightOut = true;
        }
    }

    // Game Over
    public void TriggerGameOver(string reason)
    {
        if (!isGameOver)
        {
            Debug.Log($"Game Over triggered: {reason}");
            isGameOver = true;

            GameStateChanger(GameState.GameOver);

            if (mainMenu == null)
            {
                mainMenu = FindAnyObjectByType<MainMenu>();
            }

            if (mainMenu != null)
            {
                mainMenu.SetGameOverReason($"Game Over: {reason}");
                mainMenu.GameOverPanel.SetActive(true);
            }

            OnGameOver?.Invoke(reason);
        }
    }

    // Enter the deep
    public void TriggerEnterTheDeep()
    {
        if (!hasEnteredTheDeep)
        {
            OnEnterTheDeep?.Invoke();
            hasEnteredTheDeep = true;
        }
    }

    // Hull repair
    public void TriggerHullRepair()
    {
        if (!hasRepairedHull)
        {
            OnHullRepair?.Invoke();
            hasRepairedHull = true;
        }
    }

    // Fuel refill
    public void TriggerFuelRefill()
    {
        if (!hasRefilledFuel)
        {
            OnFuelRefill?.Invoke();
            hasRefilledFuel = true;
        }
    }

    // Battery charge
    public void TriggerBatteryCharge()
    {
        if (!hasChargedBattery)
        {
            OnBatteryCharge?.Invoke();
            hasChargedBattery = true;
        }
    }

    public void TriggerIntro()
    {
        if (!hasPlayedIntro)
        {
            OnIntro?.Invoke();
            hasPlayedIntro = true;
        }
    }

    public void TriggerPlay()
    {
        Debug.Log("playing the game.");
        OnPlay?.Invoke();
        hasPlayedFirstPlay = true;
        GameStateChanger(GameState.Play);
        gameAnimation.Play();
    }

    GameObject IsObjectInView(string tag)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
        Camera mainCamera = Camera.main;

        foreach (GameObject obj in objects)
        {
            Vector3 screenPoint = mainCamera.WorldToViewportPoint(obj.transform.position);
            bool onScreen =
                screenPoint.z > 0
                && screenPoint.x > 0
                && screenPoint.x < 1
                && screenPoint.y > 0
                && screenPoint.y < 1;

            if (onScreen)
                return obj;
        }

        return null;
    }
}

/*
List of Events:
1. OnIntro
2. OnPlay
3. OnFirstKelp
4. OnLowFuel
5. OnFirstRock
6. OnCriticalHull
7. OnFirstJellyfish
8. OnLightOut
9. OnLowBattery
10. OnHullDamage
11. OnGameOver
12. OnPressureEvent
13. OnPlayerStun
14. OnEnterTheDeep
15. OnHullRepair
16. OnFuelRefill
17. OnBatteryCharge
*/
