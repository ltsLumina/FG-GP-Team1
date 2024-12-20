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
using VInspector;
using Random = UnityEngine.Random;
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

    [Header("Audio")]
    [SerializeField] FMODUnity.EventReference shipBonk;
    [SerializeField] FMODUnity.EventReference refuel;
    private FMOD.Studio.EventInstance gameMusic;

    [Tab("Fuel")]
    [Tooltip("The amount of fuel the train has.")]
    [RangeResettable(0, 100)]
    [SerializeField] float fuel = 100;

    [Tooltip("The rate at which fuel depletes. One unit per second.")]
    [Range(0.1f, 5f)]
    [SerializeField] float fuelDepletionRate = 1;

    [Header("Kelp")]
    [Range(1, 50)]
    [SerializeField] int kelpRestoreAmount = 25;

    [Tab("Hull")]
    [Tooltip("The hull integrity of the train.")]
    [RangeResettable(0, 3)]
    [SerializeField] int hullIntegrity = 3; // Takes 3 hits before it breaks and you lose.

    [Tooltip("The rate at which the train is repaired. One unit per second.")]
    [Range(1f, 3f)]
    [SerializeField] int repairAmount = 1;

    [Tooltip("The amount of hull breaches the train has.")]
    [Range(0, 3)]
    [SerializeField]int hullBreaches;

    [Tab("Power")]
    [Tooltip("Whether the headlights are active, and which ones are active.")]
    [SerializeField] List<GameObject> lights;

    [Tooltip("The thresholds for the power level when the lights start to dim.")]
    [SerializeField] SerializedDictionary<GameObject, float> lightSwitchThresholds;

    [Tooltip("The amount of power/electricity the train has. A low power level will dim the lights.")]
    [RangeResettable(0, 100)]
    [SerializeField]
    float power = 100;

    [Tooltip("The rate at which power depletes. One unit per second.")]
    [Range(0f, 5f)]
    [SerializeField]
    float powerDepletionRate = 1;

    [Header("Battery")]
    [Range(1, 50)]
    [SerializeField]
    float batteryChargeRate = 25;

    [Header("Lighting")]
    [Tooltip("The intensity of the light.")]
    [Range(0, 100000)]
    [SerializeField]
    float lightIntensity = 20000;

    [Tooltip("The radius of the light.")]
    [Range(0, 100)]
    [SerializeField]
    float lightRadius = 50;

    [Tab("Events")]
    [Foldout("Fuel")]
    [SerializeField]
    UnityEvent onFuelDepleted;

    [SerializeField]
    UnityEvent onFuelRestored;

    [SerializeField]
    UnityEvent<int> onDirtinessStageChanged;

    [Foldout("Hull")]
    [SerializeField]
    UnityEvent<int> onHullBreach;

    public UnityEvent<int> onHullIntegrityChanged;

    [SerializeField]
    UnityEvent onDeath;

    [Foldout("Power")]
    [SerializeField]
    UnityEvent onPowerDepleted;

    [SerializeField]
    UnityEvent onPowerRestored;

    [SerializeField]
    UnityEvent<Light> OnLightDim;

    [Foldout("Rocks")]
    [SerializeField]
    UnityEvent<Rock> onRockCollision;

    [Tab("Settings")]
    [Header("Settings")]
    [Space(10)]
    [Tooltip("Allows you to use the debug buttons in the inspector.")]
    [SerializeField]
    bool debugMode;

    [ShowIf(nameof(debugMode))]
    [Tooltip("The train wont take damage. Might break the game.")]
    [SerializeField]
    bool invincible;

    [SerializeField]
    bool showDebugInfo;

    [SerializeField]
    bool partyMode;

    [EndIf]



    // <- Cached references. ->

    // <- Properties ->

    #region Depth
    public float Depth => transform.position.y;
    public string DepthString => $"{Depth.Round()}m";
    public string DepthStringFormatted =>
        Depth.Round() < 0
            ? $"{Mathf.Abs(Depth.Round())}m below sea level"
            : $"{Depth.Round()}m above sea level";
    #endregion

    [MaxValue(100)]
    public float Fuel
    {
        get => fuel;
        private set
        {
            fuel = Mathf.Clamp(value, 0, 100);
            gameMusic.setParameterByName("Stress", (100-fuel)/100); 

            if (fuel <= 0)
                onFuelDepleted.Invoke();
        }
    }

    [MaxValue(3)]
    public int HullIntegrity
    {
        get
        {
            if (hullIntegrity <= 0)
            {
                HandleHullIntegrityDepletion();
                onDeath.Invoke();
            }
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
            if (power <= 0)
                onPowerDepleted.Invoke();
        }
    }

    // ReSharper disable once ConvertToAutoPropertyWhenPossible
    public float Speed
    {
        get => speed;
        set => speed = value;
    }

    public static Train Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager not found!");
            return;
        }

        Init();

        gameMusic = GameManager.Instance.GameMusicInstance;

        return;
        void Init()
        {
            onFuelDepleted.AddListener(HandleFuelDepletion);

            onDeath.AddListener(HandleHullIntegrityDepletion);

            onFuelDepleted.AddListener(() => onDeath.Invoke());
            onDeath.AddListener(() =>
            {
                Helpers.Find<Player>().Animator.SetTrigger("GameOver");
                Debug.Log("Died");
            });

            OnLightDim.AddListener(light =>
            {
                float volumetricDimmer = light.GetComponent<HDAdditionalLightData>().volumetricDimmer;
                light.GetComponent<HDAdditionalLightData>().volumetricDimmer = Mathf.Lerp(volumetricDimmer, 0, 1);
            });

            onPowerRestored.AddListener(() =>
            {
                foreach (var light in lights)
                {
                    var hdLight = light.GetComponent<HDAdditionalLightData>();
                    hdLight.volumetricDimmer = Mathf.Lerp(hdLight.volumetricDimmer, 2, 1);
                }
            });
            
            // on start, set activestate false
            foreach (var light in lights)
            {
                light.SetActive(false);
            }

            if (partyMode)
            {
                StartCoroutine(RGBLights());
            }

            DOTween.Kill(this);
        }
    }
    
    IEnumerator RGBLights()
    {
        while (true)
        {
            foreach (var light in lights)
            {
                light.GetComponent<Light>().color = new
                (Random.value, Random.value, Random.value
                );
            }
            yield return new WaitForSeconds(0.25f);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) StartCoroutine(RGBLights());

        if (transform.position.y < -500)
        {
            GameManager.Instance.TriggerEnterTheDeep();
            Debug.Log("deep");

            Power -= powerDepletionRate * Time.deltaTime;
            ToggleLightsAtThreshold();
        }

        Dive();
        FuelCalculation();
    }


    void HandleFuelDepletion()
    {
        Debug.Log("Fuel depleted!");
        GameManager.Instance.TriggerGameOver("Fuel ran out");
    }

    void HandleHullIntegrityDepletion()
    {
        Debug.Log("Hull integrity reached zero!");
        GameManager.Instance.TriggerGameOver("Hull destroyed");
    }

    void Dive()
    {
        Vector3 dir = Vector3.down;
        float speed = Mathf.Clamp(this.speed, 0, maxSpeed);
        transform.position += dir * (speed * Time.deltaTime);
    }

    void FuelCalculation()
    {
        Fuel -= fuelDepletionRate * Time.deltaTime;
        Helpers.Find<Player>().Animator.SetBool("FuelCritical", Fuel < 20);
    }

    void ToggleLightsAtThreshold()
    {
        Helpers.Find<Player>().Animator.SetBool("LightsCritical", Power < 20);
        
        // If the power is below the threshold, dim the lights
        foreach (var light in lights)
        {
            var battery = Helpers.Find<Battery>().GetComponent<Resource>();
            light.SetActive(!battery.Grabbed);
            
            if (Power <= lightSwitchThresholds[light])
            {
                OnLightDim.Invoke(light.GetComponent<Light>());
            }
            else // Power has been restored above the threshold
            {
                onPowerRestored.Invoke();
            }
        }
        
        // if no lights are on
        if (Power <= 0)
        {
            Helpers.Find<Player>().Animator.SetTrigger("LightsOut");
        }
    }

    #region Tasks
    public void SetTaskStatus(Tasks tasks)
    {
        switch (tasks)
        {
            case Tasks.Refuel:
                Fuel += kelpRestoreAmount;
                var fuel = FMODUnity.RuntimeManager.CreateInstance(refuel);
                FMODUnity.RuntimeManager.AttachInstanceToGameObject(fuel, transform);
                fuel.start();
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

    public bool CanPerformTask(Tasks tasks) =>
        tasks switch
        {
            Tasks.Refuel => Fuel < 100,
            Tasks.Repair => HullIntegrity < 3
                && hullBreaches > 0
                && !Player.HoldingResource(out Resource _),
            Tasks.Recharge => Power < 100
                && Player.HoldingResource(out Resource battery)
                && battery.Item == IGrabbable.Items.Battery,
            _ => false,
        };

    public bool IsTaskComplete(Tasks tasks) =>
        tasks switch
        {
            Tasks.Refuel => Fuel >= 100,
            Tasks.Repair => HullIntegrity >= 3,
            Tasks.Recharge => Power >= 100,
            _ => false,
        };
    #endregion

    #region Collision
    void OnCollisionEnter(Collision other)
    {
        switch (other.gameObject.tag)
        {
            case "Rock":
                Debug.Log("hit", other.gameObject);
                OnCollision(other.gameObject);
                break;
        }
    }

    bool firstHit;
    
    void OnCollision(GameObject collision)
    {
        if (invincible) return;
        
        if (collision.CompareTag("Rock"))
        {
            if (!firstHit)
            {
                firstHit = true;
            }

            var bonk = FMODUnity.RuntimeManager.CreateInstance(shipBonk);
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(bonk, transform);
            bonk.start();

            hullBreaches++;
            HullIntegrity = Mathf.Max(0, 3 - hullBreaches);
            onHullBreach.Invoke(hullBreaches);
            onHullIntegrityChanged.Invoke(HullIntegrity);
            onRockCollision.Invoke(collision.GetComponent<Rock>());

            Helpers.Find<Player>().Animator.SetBool("HullCritical", HullIntegrity == 1);
        }
    }
    #endregion

    [EndTab]
    [Button, UsedImplicitly, ShowIf(nameof(debugMode))]
    void c_Refuel() => Fuel = 100;

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
}

#if UNITY_EDITOR
public class TrainEditorWindow : EditorWindow
{
    public static void Open()
    {
        TrainEditorWindow window = GetWindow<TrainEditorWindow>();
        window.titleContent = new("Train Editor");
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
