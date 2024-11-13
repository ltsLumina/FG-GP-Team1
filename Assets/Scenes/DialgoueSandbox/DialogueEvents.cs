using UnityEngine;

public class DialogueEvents : MonoBehaviour
{
    [SerializeField]
    private DialogueManager dialogueManager;

    // Dialogue SO for each sequence
    [SerializeField]
    private Dialogue introDialogue;

    [SerializeField]
    private Dialogue gameStartedDialogue;

    [SerializeField]
    private Dialogue firstPlayDialogue;

    [SerializeField]
    private Dialogue firstKelpDialogue;

    [SerializeField]
    private Dialogue firstRockDialogue;

    [SerializeField]
    private Dialogue rockHitsShipDialogue;

    [SerializeField]
    private Dialogue hullBreachPressureDialogue;

    [SerializeField]
    private Dialogue criticalHullDamageDialogue;

    [SerializeField]
    private Dialogue criticalFuelLevelDialogue;

    [SerializeField]
    private Dialogue criticalBatteryChargeLevelDialogue;

    [SerializeField]
    private Dialogue deathFromRunningOutOfFuelDialogue;

    [SerializeField]
    private Dialogue deathFromHullDestroyedDialogue;

    [SerializeField]
    private Dialogue lightsOutDialogue;

    [SerializeField]
    private Dialogue enterTheDeepDialogue;

    [SerializeField]
    private Dialogue hullRepairDialogue;

    [SerializeField]
    private Dialogue fuelRefillDialogue;

    [SerializeField]
    private Dialogue batteryChargeDialogue;

    [SerializeField]
    private Dialogue playerStunDialogue;

    [SerializeField]
    private Dialogue electricFishOnScreenDialogue;

    private void Awake()
    {
        if (dialogueManager == null)
        {
            dialogueManager = GetComponent<DialogueManager>();
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from GameManager events
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnIntro -= TriggerIntroDialogue;
            GameManager.Instance.OnPlay -= TriggerFirstPlayDialogue;
            GameManager.Instance.OnFirstKelp -= TriggerFirstKelpDialogue;
            GameManager.Instance.OnFirstRock -= TriggerFirstRockDialogue;
            GameManager.Instance.OnPressureEvent -= TriggerRockHitsShipDialogue;
            GameManager.Instance.OnCriticalHull -= TriggerCriticalHullDialogue;
            GameManager.Instance.OnLowFuel -= TriggerCriticalFuelDialogue;
            GameManager.Instance.OnLowBattery -= TriggerCriticalBatteryDialogue;
            GameManager.Instance.OnGameOver -= TriggerGameOverDialogue;
            GameManager.Instance.OnLightOut -= TriggerLightsOutDialogue;
            GameManager.Instance.OnEnterTheDeep -= TriggerEnterTheDeepDialogue;
            GameManager.Instance.OnHullRepair -= TriggerHullRepairDialogue;
            GameManager.Instance.OnFuelRefill -= TriggerFuelRefillDialogue;
            GameManager.Instance.OnBatteryCharge -= TriggerBatteryChargeDialogue;
            GameManager.Instance.OnPlayerStun -= TriggerPlayerStunDialogue;
            GameManager.Instance.OnFirstJellyfish -= TriggerElectricFishOnScreenDialogue;
        }
    }

    private void Start()
    {
        // Check for missing dialogues
        if (introDialogue == null) // shame on you turner
            Debug.LogWarning("Intro dialogue is missing.");
        if (gameStartedDialogue == null)
            Debug.LogWarning("Game started dialogue is missing.");
        if (firstPlayDialogue == null)
            Debug.LogWarning("First play dialogue is missing.");
        if (firstKelpDialogue == null)
            Debug.LogWarning("First kelp dialogue is missing.");
        if (firstRockDialogue == null)
            Debug.LogWarning("First rock dialogue is missing.");
        if (rockHitsShipDialogue == null)
            Debug.LogWarning("Rock hits ship dialogue is missing.");
        if (hullBreachPressureDialogue == null)
            Debug.LogWarning("Hull breach pressure dialogue is missing.");
        if (criticalHullDamageDialogue == null)
            Debug.LogWarning("Critical hull damage dialogue is missing.");
        if (criticalFuelLevelDialogue == null)
            Debug.LogWarning("Critical fuel level dialogue is missing.");
        if (criticalBatteryChargeLevelDialogue == null)
            Debug.LogWarning("Critical battery charge level dialogue is missing.");
        if (deathFromRunningOutOfFuelDialogue == null)
            Debug.LogWarning("Death from running out of fuel dialogue is missing.");
        if (deathFromHullDestroyedDialogue == null)
            Debug.LogWarning("Death from hull destroyed dialogue is missing.");
        if (lightsOutDialogue == null)
            Debug.LogWarning("Lights out dialogue is missing.");
        if (enterTheDeepDialogue == null)
            Debug.LogWarning("Enter the deep dialogue is missing.");
        if (hullRepairDialogue == null)
            Debug.LogWarning("Hull repair dialogue is missing.");
        if (fuelRefillDialogue == null)
            Debug.LogWarning("Fuel refill dialogue is missing.");
        if (batteryChargeDialogue == null)
            Debug.LogWarning("Battery charge dialogue is missing.");
        if (playerStunDialogue == null)
            Debug.LogWarning("Player stun dialogue is missing.");
        if (electricFishOnScreenDialogue == null)
            Debug.LogWarning("Electric fish on screen dialogue is missing.");

        // Subscribe to GameManager events
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnIntro += TriggerIntroDialogue;
            GameManager.Instance.OnPlay += TriggerFirstPlayDialogue;
            GameManager.Instance.OnFirstKelp += TriggerFirstKelpDialogue;
            GameManager.Instance.OnFirstRock += TriggerFirstRockDialogue;
            GameManager.Instance.OnPressureEvent += TriggerRockHitsShipDialogue;
            GameManager.Instance.OnCriticalHull += TriggerCriticalHullDialogue;
            GameManager.Instance.OnLowFuel += TriggerCriticalFuelDialogue;
            GameManager.Instance.OnLowBattery += TriggerCriticalBatteryDialogue;
            GameManager.Instance.OnGameOver += TriggerGameOverDialogue;
            GameManager.Instance.OnLightOut += TriggerLightsOutDialogue;
            GameManager.Instance.OnEnterTheDeep += TriggerEnterTheDeepDialogue;
            GameManager.Instance.OnHullRepair += TriggerHullRepairDialogue;
            GameManager.Instance.OnFuelRefill += TriggerFuelRefillDialogue;
            GameManager.Instance.OnBatteryCharge += TriggerBatteryChargeDialogue;
            GameManager.Instance.OnPlayerStun += TriggerPlayerStunDialogue;
            GameManager.Instance.OnFirstJellyfish += TriggerElectricFishOnScreenDialogue;
        }
    }

    private void TriggerIntroDialogue()
    {
        if (introDialogue != null)
        {
            dialogueManager.StartDialogue(introDialogue);
        }
    }

    private void TriggerFirstPlayDialogue()
    {
        if (firstPlayDialogue != null)
        {
            dialogueManager.StartDialogue(firstPlayDialogue);
        }
    }

    private void TriggerFirstKelpDialogue(GameObject kelp)
    {
        if (firstKelpDialogue != null)
        {
            dialogueManager.StartDialogue(firstKelpDialogue, kelp);
        }
    }

    private void TriggerFirstRockDialogue(GameObject rock)
    {
        if (firstRockDialogue != null)
        {
            dialogueManager.StartDialogue(firstRockDialogue, rock);
        }
    }

    private void TriggerRockHitsShipDialogue()
    {
        if (rockHitsShipDialogue != null)
        {
            dialogueManager.StartDialogue(rockHitsShipDialogue);
        }
    }

    private void TriggerCriticalHullDialogue()
    {
        if (criticalHullDamageDialogue != null)
        {
            dialogueManager.StartDialogue(criticalHullDamageDialogue);
        }
    }

    private void TriggerCriticalFuelDialogue()
    {
        if (criticalFuelLevelDialogue != null)
        {
            dialogueManager.StartDialogue(criticalFuelLevelDialogue);
        }
    }

    private void TriggerCriticalBatteryDialogue()
    {
        if (criticalBatteryChargeLevelDialogue != null)
        {
            dialogueManager.StartDialogue(criticalBatteryChargeLevelDialogue);
        }
    }

    private void TriggerGameOverDialogue(string reason)
    {
        if (reason.ToLower() == "fuel" && deathFromRunningOutOfFuelDialogue != null)
        {
            dialogueManager.StartDialogue(deathFromRunningOutOfFuelDialogue);
        }
        else if (reason.ToLower() == "hull" && deathFromHullDestroyedDialogue != null)
        {
            dialogueManager.StartDialogue(deathFromHullDestroyedDialogue);
        }
    }

    private void TriggerLightsOutDialogue()
    {
        if (lightsOutDialogue != null)
        {
            dialogueManager.StartDialogue(lightsOutDialogue);
        }
    }

    private void TriggerEnterTheDeepDialogue()
    {
        if (enterTheDeepDialogue != null)
        {
            dialogueManager.StartDialogue(enterTheDeepDialogue);
        }
    }

    private void TriggerHullRepairDialogue()
    {
        if (hullRepairDialogue != null)
        {
            dialogueManager.StartDialogue(hullRepairDialogue);
        }
    }

    private void TriggerFuelRefillDialogue()
    {
        if (fuelRefillDialogue != null)
        {
            dialogueManager.StartDialogue(fuelRefillDialogue);
        }
    }

    private void TriggerBatteryChargeDialogue()
    {
        if (batteryChargeDialogue != null)
        {
            dialogueManager.StartDialogue(batteryChargeDialogue);
        }
    }

    private void TriggerPlayerStunDialogue()
    {
        if (playerStunDialogue != null)
        {
            dialogueManager.StartDialogue(playerStunDialogue);
        }
    }

    private void TriggerElectricFishOnScreenDialogue(GameObject jelly)
    {
        if (electricFishOnScreenDialogue != null)
        {
            dialogueManager.StartDialogue(electricFishOnScreenDialogue, jelly);
        }
    }
}
