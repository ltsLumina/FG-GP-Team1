#region
using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Custom.Attributes;
using UnityEngine.Events;
using VHierarchy.Libs;
using VInspector;
#endregion

[Author("Alex"), DisallowMultipleComponent]
public class Train : MonoBehaviour
{
#pragma warning disable 0414

    [UsedImplicitly] // This is used by the VInspector. Don't remove it and don't remove 'public'. 
    public VInspectorData vInspectorData;

    [Header("Train Settings")]
    [SerializeField] float speed = 5;
    [SerializeField] float maxSpeed = 10;

    [Tab("Fuel")]
    [Tooltip("The amount of fuel the train has.")]
    [RangeResettable(0, 100)]
    [SerializeField] float fuel = 100;
    [Tooltip("The rate at which fuel depletes. One unit per second.")]
    [Range(0.1f, 5f)]
    [SerializeField] float fuelDepletionRate = 1;
    [Tooltip("The multipliers for the fuel depletion rate per dirtiness level.")]
    [SerializeField] List<FuelDepletionRateMultiplier> fuelDepletionRateMultipliers;
    
    [Header("Dirtiness")]
    [SerializeField] List<Dirtiness> dirtinessStages = new (5);
    [Tooltip("The current dirtiness stage of the train.")]
    [Range(1, 5)]
    [SerializeField] int dirtinessStage = 1;
    [Tooltip("The amount of dirtiness the train has.")]
    [RangeResettable(0, 100)]
    [SerializeField] float dirtiness;
    [Tooltip("The rate at which dirtiness increases. One unit per second.")]
    [Range(0f, 5f)]
    [SerializeField] float dirtinessRate = 1;
    
    [Header("Algae")]
    [Tooltip("The amount of algae restored by the algae.")]
    [Range(1, 50)]
    [SerializeField] int algaeRestoreAmount = 10;

    [Tab("Hull")]
    [Tooltip("The hull integrity of the train.")]
    [RangeResettable(0, 100)]
    [SerializeField] float hullIntegrity = 100; // "Repair" task.
    [Tooltip("The rate at which the train is repaired. One unit per second.")]
    [Range(0.1f, 5f)]
    [SerializeField] float repairRate = 1;
    [Tooltip("The amount of hull breaches the train has.")]
    [Range(0,10)]
    [SerializeField] int hullBreaches;
    [Tooltip("The rate at which the hull integrity depletes. One unit per second.")]
    [SerializeField] List<HullIntegrityDepletionRate> hullIntegrityDepletionRate;

    [Tab("Power")]
    [Tooltip("The amount of power/electricity the train has. A low power level will dim the lights.")]
    [RangeResettable(0, 100)]
    [SerializeField] float power = 100; // Also known as "Light".
    [Tooltip("The rate at which power depletes. One unit per second.")]
    [Range(0f, 5f)]
    [SerializeField] float powerDepletionRate = 1;
    [Tooltip("The rate at which power regenerates upon gathering a jellyfish. One unit per second.")]
    [Range(0f, 5f)]
    [SerializeField] float powerRegenerationRate = 1;

    [Header("Lighting")]
    [Tooltip("The intensity of the light.")]
    [Range(0.1f, 5f)]
    [SerializeField] float lightIntensity = 1;
    [Tooltip("The radius of the light.")]
    [Range(0.1f, 10f)]
    [SerializeField] float lightRadius = 5;
    [Tooltip("The threshold for the power level when the light starts to dim.")]
    [Range(0, 100)]
    [SerializeField] int powerDimThreshold = 25;
    [Tooltip("A multiplier for the intensity and radius of the light when the power is low.")]
    [Range(0.1f, 1f)]
    [SerializeField] float lightDimMultiplier = 0.5f;

    [Header("Jellyfish")]
    [Tooltip("Whether to have a spawn rate or an interval.")]
    [SerializeField] bool spawnInterval;

    [Tooltip("The rate at which jellyfish spawn. One spawn per minute.")]
    [Range(1f, 60f), HideIf(nameof(spawnInterval))]
    [SerializeField] float jellyfishSpawnRate = 1; // TODO: Might move this to a separate script. 
    [Tooltip("Alternatively, the interval between jellyfish spawns.")]
    [Range(1f, 60f), ShowIf(nameof(spawnInterval))]
    [SerializeField] float jellyfishSpawnInterval = 10;
    [EndIf]
    [Tooltip("The speed of the jellyfish.")]
    [Range(1, 10)]
    [SerializeField] float jellyfishSpeed = 5;
    [Tooltip("The amount of power restored by a single jellyfish.")]
    [Range(1, 50)]
    [SerializeField] int jellyfishRestoreAmount = 10;
    [Tooltip("The duration of the stun when the player bumps into a jellyfish.")]
    [Range(1, 5)]
    [SerializeField] float jellyfishStunDuration = 1;
    [Tooltip("The amount of hull breaches caused by a single jellyfish hit.")]
    [Range(1, 5)]
    [SerializeField] int jellyfishHullBreachesPerHit = 1;

    [Tab("Rocks")]
    [SerializeField] string inDevelopment = "Yes.";

    [Tab("Events")]
    [Foldout("Fuel")]
    [SerializeField] UnityEvent onFuelDepleted;
    [SerializeField] UnityEvent onFuelRestored;
    [SerializeField] UnityEvent<int> onDirtinessStageChanged;
    [Foldout("Hull")]
    [SerializeField] UnityEvent onDeath;
    [Foldout("Power")]
    [SerializeField] UnityEvent onPowerDepleted;
    [SerializeField] UnityEvent onPowerRestored;
    [SerializeField] UnityEvent OnLightDim;
    [SerializeField] UnityEvent<int> onHullBreach;
    [Foldout("Rocks")]
    [SerializeField] UnityEvent<GameObject> onRockCollision;

    [Tab("Settings")]
    [Header("Settings")]
    [Space(10)]
    [Tooltip("Allows you to use the debug buttons in the inspector.")]
    [SerializeField] bool debugMode;
    [ShowIf(nameof(debugMode))]
    [Tooltip("The train wont take damage. Might break the game.")]
    [SerializeField] bool invincible;
    [SerializeField] bool showDebugInfo;
    [EndIf]

    [SerializeField] List<float> fuelDepletionDefaults = new () { 1, 1.5f, 2, 2.5f, 3 };
    [SerializeField] List<float> hullIntegrityDepletionDefaults = new () { 1, 5f, 7.5f, 10f, 15f };

    // <- Cached references. ->
    
    // <- Properties ->

    float Fuel
    {
        get => fuel;
        set
        {
            fuel = Mathf.Clamp(value, 0, 100);
            if (fuel <= 0) onFuelDepleted.Invoke();
        }
    }
    
    float HullIntegrity
    {
        get => hullIntegrity;
        set
        {
            if (invincible) return;
            hullIntegrity = Mathf.Clamp(value, 0, 100);
            if (hullIntegrity <= 0) onDeath.Invoke();
        }
    }

    void OnGUI()
    {
        if (!showDebugInfo) return;

        var style = new GUIStyle(GUI.skin.box) { fontSize = 40 };
        using (new GUILayout.VerticalScope("box"))
        {
            GUILayout.Label($"Speed: {speed}", style);
            GUILayout.Label($"Fuel: {fuel.Round()}", style);
            GUILayout.Label($"Hull Integrity: {hullIntegrity.Round()}", style);
            GUILayout.Label($"Dirtiness: {dirtiness.Round()} (Stage {dirtinessStage})", style);
            GUILayout.Label($"Power: {power.Round()}", style);
            GUILayout.Label($"Jellyfish Spawn Rate: {jellyfishSpawnRate}", style);
            GUILayout.Label($"Jellyfish Spawn Interval: {jellyfishSpawnInterval}", style);
        }
    }

    void Start()
    {
        Init();

        return;
        void Init()
        {
            onDeath.AddListener(() => Debug.Log("The train has died."));
        }
    }

    void Update()
    {
        Dive();

        dirtiness      = Mathf.Clamp(dirtiness + dirtinessRate * Time.deltaTime, 0, 100);
        dirtinessStage = Mathf.Clamp(dirtinessStages.FindIndex(d => dirtiness < d.threshold) + 1, 1, fuelDepletionRateMultipliers.Count);

        switch (dirtinessStage)
        {
            case 1: // No dirtiness. (0-19)
                Fuel -= fuelDepletionRate * fuelDepletionRateMultipliers[0].multiplier * Time.deltaTime;
                Debug.Log("No dirtiness.");
                onDirtinessStageChanged.Invoke(1);
                break;

            case 2: // Low dirtiness. (20-39)
                Fuel -= fuelDepletionRate * fuelDepletionRateMultipliers[1].multiplier * Time.deltaTime;
                Debug.Log("Low dirtiness.");
                onDirtinessStageChanged.Invoke(2);
                break;

            case 3: // Medium dirtiness. (40-59)
                Fuel -= fuelDepletionRate * fuelDepletionRateMultipliers[2].multiplier * Time.deltaTime;
                Debug.Log("Medium dirtiness.");
                onDirtinessStageChanged.Invoke(3);
                break;

            case 4: // High dirtiness. (60-79)
                Fuel -= fuelDepletionRate * fuelDepletionRateMultipliers[3].multiplier * Time.deltaTime;
                Debug.Log("High dirtiness.");
                onDirtinessStageChanged.Invoke(4);
                break;

            case 5: // Very high dirtiness. (80-100)
                Fuel -= fuelDepletionRate * fuelDepletionRateMultipliers[4].multiplier * Time.deltaTime;
                Debug.Log("Very high dirtiness.");
                onDirtinessStageChanged.Invoke(5);
                break;
        }
        
        HullIntegrity -= hullBreaches * hullIntegrityDepletionRate[hullBreaches].value * Time.deltaTime;
    }
    
    void Dive()
    {
        Vector3 dir   = Vector3.down;
        float   speed = Mathf.Clamp(this.speed, 0, maxSpeed);
        transform.position += dir * (speed * Time.deltaTime);
    }

    void Clean()
    {
        // TODO: Clean engine by standing behind it.
        // dirtiness -= example * Time.deltaTime;
        //
        // // Check if the current dirtiness stage will change
        // for (int i = 0; i < dirtinessStages.Count; i++)
        // {
        //     if (dirtiness < dirtinessStages[i].threshold)
        //     {
        //         onDirtinessStageChanged.Invoke(i + 1);
        //         break;
        //     }
        // }
    }
    
    void OnCollision(GameObject collision)
    {
        onRockCollision.Invoke(collision);
        
        switch (collision.tag)
        {
            case "Jellyfish":
                hullBreaches  += jellyfishHullBreachesPerHit;
                break;
            
            case "Rock":
                hullBreaches++;
                break;
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "Jellyfish":
                // TODO: Separate jellyfish return logic and jellyfish collision logic.
                    // This is the jellyfish return logic. 
                power += jellyfishRestoreAmount;
                onPowerRestored.Invoke();
                Destroy(other.gameObject);
                
                // This is the jellyfish collision logic.
                // StartCoroutine(OnCollision("Jellyfish"));
                break;

            case "Algae":
                Fuel += 25f;
                onFuelRestored.Invoke();
                Destroy(other.gameObject);
                break;
        }
    }

    void OnCollisionEnter(Collision other)
    {
        switch (other.gameObject.tag)
        {
            case "Rock":
                OnCollision(other.gameObject);
            break;
        }
    }

    [EndTab]
    
    [Button, UsedImplicitly, ShowIf(nameof(debugMode))]
    void c_Refuel() => fuel = 100;

    [Button, UsedImplicitly, ShowIf(nameof(debugMode))]
    void c_Clean() => dirtiness = 0;

    [Button, UsedImplicitly, ShowIf(nameof(debugMode))]
    void c_Repair() => hullIntegrity = 100;

    [Button, UsedImplicitly, ShowIf(nameof(debugMode))]
    void c_Damage() => hullIntegrity -= 25f;

    [Button, UsedImplicitly, ShowIf(nameof(debugMode))]
    void c_Recharge() => power = 100;

    [Serializable]
    struct Dirtiness
    {
        [HideInInspector, UsedImplicitly]
        public string name;
        public int threshold;

        public Dirtiness(int threshold = 0)
        {
            name           = "Stage";
            this.threshold = threshold;
        }
    }

    [Serializable]
    struct FuelDepletionRateMultiplier
    {
        [HideInInspector, UsedImplicitly]
        public string name;
        public float multiplier;

        public FuelDepletionRateMultiplier(float multiplier = 1)
        {
            name           = "Stage";
            this.multiplier = multiplier;
        }
    }
    
    [Serializable]
    struct HullIntegrityDepletionRate
    {
        [HideInInspector, UsedImplicitly]
        public string name;
        public float value;

        public HullIntegrityDepletionRate(float value = 1)
        {
            name           = "Stage";
            this.value = value;
        }
    }
  
    #region Validation/Editor
    void OnValidate()
    {
        if (inDevelopment.Contains("No", StringComparison.InvariantCultureIgnoreCase)) inDevelopment = "You think you're so funny don't you.";
        else if (!string.Equals(inDevelopment, "Yes.", StringComparison.Ordinal)) inDevelopment      = "Yes.";

        ValidateDirtiness();
        ValidateFuelDepletionRateMultipliers();
        ValidateHullIntegrityDepletionRate();

        // this is a test
    }

    void Reset()
    {
        ValidateDirtiness(); 
        ValidateFuelDepletionRateMultipliers();
        ValidateHullIntegrityDepletionRate();
    }

    #region Utility
    void ValidateDirtiness()
    {
        if (dirtinessStages == null) return;
        
        // Ensure dirtinessStages always has 5 elements
        if (dirtinessStages.Count != 5)
        {
            dirtinessStages.Clear();
             
            List<Dirtiness> defaults = new () { new (20), new (40), new (60), new (80), new (100) };
            dirtinessStages = defaults;
        }

        // Set the name of each dirtiness stage
        for (int i = 0; i < dirtinessStages.Count; i++)
        {
            Dirtiness stage = dirtinessStages[i];
            stage.name     = $"Stage {i + 1}";
            dirtinessStages[i] = stage;
        }

        // Set the dirtiness stage depending on the dirtiness rate
        for (int i = 0; i < dirtinessStages.Count; i++)
        {
            if (dirtiness < dirtinessStages[i].threshold)
            {
                dirtinessStage = i + 1;
                break;
            }
        }
    }

    void ValidateFuelDepletionRateMultipliers()
    {
        if (fuelDepletionRateMultipliers == null) return;
        
        // Ensure fuelDepletionRateMultipliers always has 5 elements
        if (fuelDepletionRateMultipliers.Count != 5)
        {
            fuelDepletionRateMultipliers.Clear();
            
            fuelDepletionRateMultipliers = fuelDepletionDefaults.ConvertAll(m => new FuelDepletionRateMultiplier(m));
        }

        // Set the name of each fuel depletion rate multiplier
        for (int i = 0; i < fuelDepletionRateMultipliers.Count; i++)
        {
            FuelDepletionRateMultiplier multiplier = fuelDepletionRateMultipliers[i];
            multiplier.name                 = $"Stage {i + 1}";
            fuelDepletionRateMultipliers[i] = multiplier;
        }
    }

    void ValidateHullIntegrityDepletionRate()
    {
        if (hullIntegrityDepletionRate == null) return;
        
        // Ensure hullIntegrityDepletionRate always has 5 elements
        if (hullIntegrityDepletionRate.Count != 5)
        {
            hullIntegrityDepletionRate.Clear();
            
            hullIntegrityDepletionRate = hullIntegrityDepletionDefaults.ConvertAll(m => new HullIntegrityDepletionRate(m));
        }

        // Set the name of each hull integrity depletion rate
        for (int i = 0; i < hullIntegrityDepletionRate.Count; i++)
        {
            HullIntegrityDepletionRate rate = hullIntegrityDepletionRate[i];
            rate.name                 = $"Stage {i + 1}";
            hullIntegrityDepletionRate[i] = rate;
        }
    }
    #endregion
    #endregion
}
