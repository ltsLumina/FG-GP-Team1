#region
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using JetBrains.Annotations;
using Lumina.Essentials.Modules;
using UnityEditor;
using UnityEngine;
using UnityEngine.Custom.Attributes;
using UnityEngine.Events;
using UnityEngine.Rendering.HighDefinition;
#if UNITY_EDITOR
using VHierarchy.Libs;
#endif
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
    [Tooltip("Whether the headlights are active, and which ones are active.")]
    [SerializeField] SerializedDictionary<GameObject, bool> lights;

    [Tooltip("The thresholds for the power level when the lights start to dim.")]
    [SerializeField] SerializedDictionary<GameObject, float> lightSwitchThresholds;
        
    [Tooltip("The amount of power/electricity the train has. A low power level will dim the lights.")]
    [RangeResettable(0, 100)]
    [SerializeField] float power = 100;
    [Tooltip("The rate at which power depletes. One unit per second.")]
    [Range(0f, 5f)]
    [SerializeField] float powerDepletionRate = 1;

    [Header("Battery")]
    [Range(1, 50)]
    [SerializeField] float batteryChargeRate = 25;

    [Header("Lighting")]
    [Tooltip("The intensity of the light.")]
    [Range(0, 100000)]
    [SerializeField] float lightIntensity = 20000;
    [Tooltip("The radius of the light.")]
    [Range(0, 100)]
    [SerializeField] float lightRadius = 50;

    // [Header("Jellyfish")]
    // [Tooltip("Whether to have a spawn rate or an interval.")]
    // [SerializeField] bool spawnInterval;
    //
    // [Tooltip("The rate at which jellyfish spawn. One spawn per minute.")]
    // [Range(1f, 60f), HideIf(nameof(spawnInterval))]
    // [SerializeField] float jellyfishSpawnRate = 1; // TODO: Might move this to a separate script. 
    // [Tooltip("Alternatively, the interval between jellyfish spawns.")]
    // [Range(1f, 60f), ShowIf(nameof(spawnInterval))]
    // [SerializeField] float jellyfishSpawnInterval = 10;
    // [EndIf]
    // [Tooltip("The speed of the jellyfish.")]
    // [Range(1, 10)]
    // [SerializeField] float jellyfishSpeed = 5;
    // [Tooltip("The amount of power restored by a single jellyfish.")]
    // [Range(1, 50)]
    // [SerializeField] int jellyfishRestoreAmount = 25;
    // [Tooltip("The duration of the stun when the player bumps into a jellyfish.")]
    // [Range(1, 5)]
    // [SerializeField] float jellyfishStunDuration = 1;

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
    [SerializeField] UnityEvent<Light> OnLightDim;
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
    [SerializeField] bool partyMode;
    [EndIf]

    [SerializeField] List<float> fuelDepletionDefaults = new () { 1, 1.5f, 2, 2.5f, 3 };
    [SerializeField] List<DirtinessStage> dirtinessStagesDefaults = new () { new (20), new (40), new (60), new (80), new (100) };
    
    // <- Cached references. ->
    
    // <- Properties ->

    #region Depth
    public float Depth => transform.position.y;
    public string DepthString => $"{Depth.Round()}m";
    public string DepthStringFormatted => Depth.Round() < 0 ? $"{Mathf.Abs(Depth.Round())}m below sea level" : $"{Depth.Round()}m above sea level";
    #endregion

    [MaxValue(100)]
    public float Fuel
    {
        get => fuel;
        private set
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
        set
        {
            power = Mathf.Clamp(value, 0, 100);
            if (power <= 0) onPowerDepleted.Invoke();
        }
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
        }
    }
#endif

    void Start()
    {
        // TODO: for alpha
        GameManager.Instance.GameStateChanger(GameManager.GameState.Play);

        Init();

        return;
        void Init()
        {
            onFuelDepleted.AddListener(() => onDeath.Invoke());
            onDeath.AddListener
            (() =>
            {
                GameManager.Instance.GameStateChanger(GameManager.GameState.GameOver);
                Debug.Log("Died");
            });

            DOTween.SetTweensCapacity(1000, 5);
            
            OnLightDim.AddListener
            (light =>
            {
                var hdLightData = light.GetComponent<HDAdditionalLightData>();
                DOVirtual.Float(hdLightData.volumetricDimmer, 0, 1, x => hdLightData.volumetricDimmer = x).OnComplete(() => { lights[light.gameObject] = false; });
            });

            onPowerRestored.AddListener
            (() =>
            {
                foreach (var light in lights)
                {
                    var hdLightData = light.Key.GetComponent<HDAdditionalLightData>();
                    DOVirtual.Float(hdLightData.volumetricDimmer, 1, 1, x => hdLightData.volumetricDimmer = x).OnComplete(() => { lights[light.Key] = true; });
                }
            });
            
            if (partyMode)
            {
                StartCoroutine(RGBLights());
            }
        }
    }
    
    IEnumerator RGBLights()
    {
        while (true)
        {
            foreach (var light in lights)
            {
                light.Key.GetComponent<Light>().color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
            }
            yield return new WaitForSeconds(0.25f);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            StartCoroutine(RGBLights());
        }
        
        Dive();
        FuelDirtinessCalculation();
        ToggleLightsAtThreshold();

    }

    void Dive()
    {
        Vector3 dir = Vector3.down;
        float speed = Mathf.Clamp(this.speed, 0, maxSpeed);
        transform.position += dir * (speed * Time.deltaTime);
    }
    
    void FuelDirtinessCalculation()
    {
        dirtiness = Mathf.Clamp(dirtiness + dirtinessRate * Time.deltaTime, 0, 100);
        dirtinessStage = Mathf.Clamp(dirtinessStages.FindIndex(d => dirtiness < d.threshold) + 1, 1, fuelDepletionRateMultipliers.Count);

        Fuel -= fuelDepletionRate * fuelDepletionRateMultipliers[dirtinessStage].multiplier * Time.deltaTime;
        onDirtinessStageChanged.Invoke(dirtinessStage);
    }

    void ToggleLightsAtThreshold()
    {
        foreach (var light in lights)
        {
            Power -= powerDepletionRate * Time.deltaTime;
            if (Power <= lightSwitchThresholds[light.Key])
            {
                OnLightDim.Invoke(light.Key.GetComponent<Light>());
            }
            else // Power has been restored above the threshold
            {
                onPowerRestored.Invoke();
            }
        }

        foreach (var light in lights)
        {
            light.Key.SetActive(light.Value);
        }
    }

    #region Tasks
    public void SetTaskStatus(Tasks tasks)
    {
        switch (tasks)
        {
            case Tasks.Clean:
                Dirtiness -= algaeRestoreAmount;
                break;

            case Tasks.Refuel:
                Fuel += kelpRestoreAmount;
                break;

            case Tasks.Repair:
                hullBreaches--;
                HullIntegrity += repairAmount;
                onHullIntegrityChanged.Invoke(HullIntegrity);
                break;

            case Tasks.Recharge:
                // Handled by Battery.cs
                break;
        }
    }

    public bool CanPerformTask(Tasks tasks) => tasks switch
    { Tasks.Clean    => Dirtiness > 0,
      Tasks.Refuel   => Fuel < 100,
      Tasks.Repair   => HullIntegrity < 3 && hullBreaches > 0 && !Player.HoldingResource(out Resource _),
      Tasks.Recharge => Power < 100 && Player.HoldingResource(out Resource battery) && battery.Item == IGrabbable.Items.Battery,
      _              => false };

    public bool IsTaskComplete(Tasks tasks) => tasks switch
    { Tasks.Clean    => Dirtiness     <= 20,
      Tasks.Refuel   => Fuel          >= 100,
      Tasks.Repair   => HullIntegrity >= 3,
      Tasks.Recharge => Power         >= 100,
      _                   => false };
    #endregion

    #region Collision
    void OnCollisionEnter(Collision other)
    {
        switch (other.gameObject.tag)
        {
            case "Rock":
                OnCollision(other.gameObject);
                break;
        }
    }

    void OnCollision(GameObject collision)
    {
        if (collision.CompareTag("Rock"))
        {
            hullBreaches++;
            HullIntegrity = Mathf.Max(0, 3 - hullBreaches);
            onHullBreach.Invoke(hullBreaches);
            onHullIntegrityChanged.Invoke(HullIntegrity);
            onRockCollision.Invoke(collision.GetComponent<Rock>());
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
    void c_ShowTasks() => TrainEditorWindow.Open();
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
            
            dirtinessStages = dirtinessStagesDefaults.ConvertAll(t => new DirtinessStage(t.threshold));
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

        foreach (var managementCollider in Helpers.FindMultiple<Task>())
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

internal static class TrainExtensions
{
    public static float Round(this float value) => Mathf.Round(value);
}
