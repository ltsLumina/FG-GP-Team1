#region
using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Lumina.Essentials.Modules;
using UnityEditor;
using UnityEngine;
using UnityEngine.Custom.Attributes;
using UnityEngine.Events;
#if UNITY_EDITOR
using VHierarchy.Libs;
#endif
using VInspector;
#endregion

[Author("Alex"), DisallowMultipleComponent]
public class Train : MonoBehaviour
{
#pragma warning disable 0414

    public enum Task 
    {
        Clean,
        Refuel,
        Repair,
        Recharge,
    }

#if UNITY_EDITOR
    [UsedImplicitly] // This is used by the VInspector. Don't remove it and don't remove 'public'. 
    public VInspectorData vInspectorData;
#endif

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

    [Header("Kelp")]
    [Range(1, 50)]
    [SerializeField] int kelpRestoreAmount = 25;
    
    [Header("Algae")]
    [Tooltip("The amount of dirtiness restored by the algae.")]
    [Range(1, 50)]
    [SerializeField] int algaeRestoreAmount = 10;
    
    [Header("Dirtiness")]
    [SerializeField] List<DirtinessStage> dirtinessStages = new (5);
    [Tooltip("The current dirtiness stage of the train.")]
    [Range(1, 5)]
    [SerializeField] int dirtinessStage = 1;
    [Tooltip("The amount of dirtiness the train has.")]
    [RangeResettable(0, 100)]
    [SerializeField] float dirtiness;
    [Tooltip("The rate at which dirtiness increases. One unit per second.")]
    [Range(0f, 5f)]
    [SerializeField] float dirtinessRate = 1;
    
    [Header("Cleaning")]
    [RangeResettable(0, 100)]
    [SerializeField] float dirtinessCleanAmount = 25;

    [Tab("Hull")]
    [Tooltip("The hull integrity of the train.")]
    [RangeResettable(0, 3)] 
    [SerializeField] int hullIntegrity = 3; // Takes 3 hits before it breaks and you lose.
    [Tooltip("The rate at which the train is repaired. One unit per second.")]
    [Range(1f, 3f)]
    [SerializeField] int repairAmount = 1;
    [Tooltip("The amount of hull breaches the train has.")]
    [Range(0,3)]
    [SerializeField] int hullBreaches;

    [Tab("Power")]
    [Tooltip("The amount of power/electricity the train has. A low power level will dim the lights.")]
    [RangeResettable(0, 100)]
    [SerializeField] float power = 100; // Also known as "Light".
    [Tooltip("The rate at which power depletes. One unit per second.")]
    [Range(0f, 5f)]
    [SerializeField] float powerDepletionRate = 1;
    [Tooltip("The rate at which power regenerates upon gathering a jellyfish. One unit per second.")]
    [Range(0f, 5f)]
    [SerializeField] float powerRechargeAmount = 1;

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
    [SerializeField] int jellyfishRestoreAmount = 25;
    [Tooltip("The duration of the stun when the player bumps into a jellyfish.")]
    [Range(1, 5)]
    [SerializeField] float jellyfishStunDuration = 1;

    [Tab("Rocks")]
    [SerializeField] string inDevelopment = "Yes.";

    [Tab("Events")]
    [Foldout("Fuel")]
    [SerializeField] UnityEvent onFuelDepleted;
    [SerializeField] UnityEvent onFuelRestored;
    [SerializeField] UnityEvent<int> onDirtinessStageChanged;
    [Foldout("Hull")]
    [SerializeField] UnityEvent<int> onHullBreach;
    [SerializeField] UnityEvent<int> onHullIntegrityChanged;
    [SerializeField] UnityEvent onDeath;
    [Foldout("Power")]
    [SerializeField] UnityEvent onPowerDepleted;
    [SerializeField] UnityEvent onPowerRestored;
    [SerializeField] UnityEvent OnLightDim;
    [Foldout("Rocks")]
    [SerializeField] UnityEvent<Rock> onRockCollision;

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
    ManagementCollider[] managementColliders;
    
    // <- Properties ->

    [MaxValue(100)]
    public float Fuel
    {
        get => fuel;
        set
        {
            fuel = Mathf.Clamp(value, 0, 100);
            if (fuel <= 0) onFuelDepleted.Invoke();
        }
    }
    
    [MaxValue(100)]
    public float Dirtiness
    {
        get => dirtiness;
        set => dirtiness = value;
    }

    [MaxValue(3)]
    public int HullIntegrity
    {
        get
        {
            if (hullIntegrity <= 0) onDeath.Invoke();
            return hullIntegrity;
        }
        set
        {
            if (invincible) return;
            hullIntegrity = Mathf.Clamp(value, 0, 3);
            if (hullIntegrity <= 0) onDeath.Invoke();
        }
    }

    [MaxValue(100)]
    public float Power
    {
        get => power;
        set => power = value;
    }

#if UNITY_EDITOR
    void OnGUI()
    {
        if (!showDebugInfo) return;

        var style = new GUIStyle(GUI.skin.box) { fontSize = 40 };
        using (new GUILayout.VerticalScope("box"))
        {
            GUILayout.Label($"Speed: {speed}", style);
            GUILayout.Label($"Fuel: {Fuel.Round()}", style);
            GUILayout.Label($"Hull Integrity: {HullIntegrity}", style);
            GUILayout.Label($"Dirtiness: {dirtiness.Round()} (Stage {dirtinessStage})", style);
            GUILayout.Label($"Power: {Power.Round()}", style);
            GUILayout.Label($"Jellyfish Spawn Rate: {jellyfishSpawnRate}", style);
            GUILayout.Label($"Jellyfish Spawn Interval: {jellyfishSpawnInterval}", style);
        }
    }
#endif

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
                DebugDirtiness(false);
                onDirtinessStageChanged.Invoke(1);
                break;

            case 2: // Low dirtiness. (20-39)
                Fuel -= fuelDepletionRate * fuelDepletionRateMultipliers[1].multiplier * Time.deltaTime;
                DebugDirtiness(false);
                onDirtinessStageChanged.Invoke(2);
                break;

            case 3: // Medium dirtiness. (40-59)
                Fuel -= fuelDepletionRate * fuelDepletionRateMultipliers[2].multiplier * Time.deltaTime;
                DebugDirtiness(false);
                onDirtinessStageChanged.Invoke(3);
                break;

            case 4: // High dirtiness. (60-79)
                Fuel -= fuelDepletionRate * fuelDepletionRateMultipliers[3].multiplier * Time.deltaTime;
                DebugDirtiness(false);
                onDirtinessStageChanged.Invoke(4);
                break;

            case 5: // Very high dirtiness. (80-100)
                Fuel -= fuelDepletionRate * fuelDepletionRateMultipliers[4].multiplier * Time.deltaTime;
                DebugDirtiness(false);
                onDirtinessStageChanged.Invoke(5);
                break;
        }
    }

    #region Utility
    void DebugDirtiness(bool enable)
    {
        if (!enable) return;
        Debug.Log($"Dirtiness: {dirtiness} (Stage {dirtinessStage})");
    }
    #endregion
    
    void Dive()
    {
        Vector3 dir   = Vector3.down;
        float   speed = Mathf.Clamp(this.speed, 0, maxSpeed);
        transform.position += dir * (speed * Time.deltaTime);
    }

    #region Tasks
    public void SetTaskStatus(Task task)
    {
        switch (task)
        {
            case Task.Clean:
                Dirtiness -= 25f; // TODO: Change this to a variable.
                break;

            case Task.Refuel:
                Fuel += algaeRestoreAmount;
                Player.HoldingResource(out Resource heldItem);
                Destroy(heldItem.gameObject);
                break;

            case Task.Repair:
                hullBreaches--;
                HullIntegrity += repairAmount;
                onHullIntegrityChanged.Invoke(HullIntegrity);
                break;

            case Task.Recharge:
                Power += powerRechargeAmount;
                break;
        }
    }

    public bool CanPerformTask(Task task) => task switch
    { Task.Clean    => Dirtiness > 0,
      Task.Refuel   => Fuel          < 100 && Player.HoldingResource(out Resource _),
      Task.Repair   => HullIntegrity < 3   && hullBreaches > 0,
      Task.Recharge => Power < 100,
      _             => false };
    
    public bool IsTaskComplete(Task task) => task switch
    { Task.Clean    => Dirtiness     <= 20,
      Task.Refuel   => Fuel          >= 100,
      Task.Repair   => HullIntegrity >= 3,
      Task.Recharge => Power         >= 100,
      _             => false };
    #endregion

    #region Collision/Trigger
    void OnCollision(GameObject collision)
    {
        switch (collision.tag)
        {
            case "Jellyfish":
                hullBreaches++;
                onHullBreach.Invoke(hullBreaches);
                break;
            
            case "Rock":
                hullBreaches++;
                HullIntegrity = Mathf.Max(0, 3 - hullBreaches);
                onHullBreach.Invoke(hullBreaches);
                onHullIntegrityChanged.Invoke(HullIntegrity);
                onRockCollision.Invoke(collision.GetComponent<Rock>());
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
    #endregion

    [EndTab]
    
    [Button, UsedImplicitly, ShowIf(nameof(debugMode))]
    void c_Refuel() => Fuel = 100;

    [Button, UsedImplicitly, ShowIf(nameof(debugMode))]
    void c_Clean() => dirtiness = 0;

    [Button, UsedImplicitly, ShowIf(nameof(debugMode))]
    void c_Repair() => HullIntegrity = 100;

    [Button, UsedImplicitly, ShowIf(nameof(debugMode))]
    void c_Damage() => HullIntegrity -= 1;

    [Button, UsedImplicitly, ShowIf(nameof(debugMode))]
    void c_Recharge() => Power = 100;

#if UNITY_EDITOR
    [Button, UsedImplicitly, ShowIf(nameof(debugMode))]
    void c_ShowManagementColliders() => TrainEditorWindow.Open();
#endif

    [Serializable]
    struct DirtinessStage
    {
        [HideInInspector, UsedImplicitly]
        public string name;
        public int threshold;

        public DirtinessStage(int threshold = 0)
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
  
    #region Validation/Editor
    void OnValidate()
    {
        if (inDevelopment.Contains("No", StringComparison.InvariantCultureIgnoreCase)) inDevelopment = "You think you're so funny don't you.";
        else if (!string.Equals(inDevelopment, "Yes.", StringComparison.Ordinal)) inDevelopment      = "Yes.";

        ValidateDirtiness();
        ValidateFuelDepletionRateMultipliers();
        
        HullIntegrity = Mathf.Max(0, 3 - hullBreaches);
    }

    void Reset()
    {
        ValidateDirtiness(); 
        ValidateFuelDepletionRateMultipliers();
    }

    #region Utility
    void ValidateDirtiness()
    {
        if (dirtinessStages == null) return;
        
        // Ensure dirtinessStages always has 5 elements
        if (dirtinessStages.Count != 5)
        {
            dirtinessStages.Clear();
             
            List<DirtinessStage> defaults = new () { new (20), new (40), new (60), new (80), new (100) };
            dirtinessStages = defaults;
        }

        // Set the name of each dirtiness stage
        for (int i = 0; i < dirtinessStages.Count; i++)
        {
            DirtinessStage stage = dirtinessStages[i];
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
    #endregion
    #endregion
}

#if UNITY_EDITOR
public class TrainEditorWindow : EditorWindow
{
    public static void Open()
    {
        TrainEditorWindow window = GetWindow<TrainEditorWindow>();
        window.titleContent = new ("Train Editor");
        window.Show();
    }

    Vector2 scrollPos;

    void OnGUI()
    {
        scrollPos = GUILayout.BeginScrollView(scrollPos);

        foreach (var managementCollider in Helpers.FindMultiple<ManagementCollider>())
        {
            GUILayout.Space(25);
            GUILayout.Label("", GUI.skin.horizontalSlider);
            GUILayout.Space(25);
            
            // draw editor for each management collider
            var editor = Editor.CreateEditor(managementCollider);
            editor.OnInspectorGUI();
        }

        GUILayout.EndScrollView();
    }
}
#endif
