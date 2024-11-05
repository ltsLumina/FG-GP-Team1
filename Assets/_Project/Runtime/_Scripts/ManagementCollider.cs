using System;
using DG.Tweening;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using VInspector;

[DisallowMultipleComponent, RequireComponent(typeof(BoxCollider))]
public class ManagementCollider : MonoBehaviour
{
    #pragma warning disable 0414
    
    [HideInInspector, Tooltip("DO NOT REMOVE! This is used by VInspector")]
    public VInspectorData vInspectorData;
    
    [SerializeField] Train.Task task = Train.Task.Refuel;
    
    [Tab("Collider")]
    [Space]
    [Header("Size")]
    [RangeResettable(1, 20)]
    [SerializeField] float width = 10f;
    [RangeResettable(1, 20)]
    [SerializeField] float height = 3f;
    [RangeResettable(1, 20)]
    [SerializeField] float depth = 10f;
    
    [Header("Offset")]
    [RangeResettable(-10, 10)]
    [SerializeField] float offsetX;
    [RangeResettable(-10, 10)]
    [SerializeField] float offsetY;

    [Tab("Task")]
    [Header("Timers")]
    [Tooltip("This is just a spacer variable because of how VInspector works")]
    //[SerializeField, ReadOnly, TextArea] string spacer;
    [ShowIf(nameof(task), Train.Task.Clean)]
    [RangeResettable(1, 10)]
    [SerializeField] float cleanTime = 5f;
    [ShowIf(nameof(task), Train.Task.Refuel)]
    [RangeResettable(1, 10)]
    [SerializeField] float refuelTime = 5f;
    [ShowIf(nameof(task), Train.Task.Repair)]
    [RangeResettable(1, 10)]
    [SerializeField] float repairTime = 5f;
    [ShowIf(nameof(task), Train.Task.Recharge)]
    [RangeResettable(1, 10)]
    [SerializeField] float rechargeTime = 5f;
    [EndIf]
    
    [Tooltip("The amount of time to wait after the task is complete before starting the task again")]
    [RangeResettable(0.1f, 5f)]
    [SerializeField] float waitOnTaskCompletion = 1f;
    [Space(15)]
    [SerializeField] UnityEvent onTaskPerformed;
    [SerializeField] UnityEvent<Train.Task> onTaskComplete;
    
    [Space(10)]
    [Tab("UI")]
    [SerializeField] Image chargeCircle;
    [ColorUsage(false, true)]
    [SerializeField] Color completedColor = Color.green;
    [ColorUsage(false, true)]
    [SerializeField] Color baseColor = Color.grey;

    // <- Cached Components ->
    
    Train train;
    Sequence onTaskCompleteSequence;
    
    // <- Properties ->
    
    public float TimeToCompleteTask => task switch
    { Train.Task.Clean    => cleanTime,
      Train.Task.Refuel   => refuelTime,
      Train.Task.Repair   => repairTime,
      Train.Task.Recharge => rechargeTime,
      _                   => 0 
    };
    
    #pragma warning restore 0414

    void OnValidate()
    {
        name = task.ToString();
        string[] validNames = Enum.GetNames(typeof(Train.Task));
        if (name != validNames[(int) task])
        {
            Logger.LogError($"Name must be the same as the task: {string.Join(", ", validNames)}");
            name = task.ToString();
        }
        
        // move component to top
#if UNITY_EDITOR
        if (Application.isPlaying) return;
        ComponentUtility.MoveComponentUp(this);
#endif
        
        var col = GetComponent<BoxCollider>();
        col.size = new (width, height, depth);
        col.center = new (offsetX, offsetY);
    }

    void Awake() => train = GetComponentInParent<Train>();
    
    void Start()
    {
        Debug.Assert(chargeCircle != null, "Charge circle is null");
        chargeCircle.color      = baseColor;
        chargeCircle.fillAmount = 0;

        onTaskPerformed.AddListener
        (() =>
        {
            onTaskCompleteSequence = DOTween.Sequence();
            onTaskCompleteSequence.OnStart(() => chargeCircle.color = completedColor);

            onTaskCompleteSequence.AppendInterval(waitOnTaskCompletion).OnComplete
            (() =>
            {
                chargeCircle.DOFillAmount(0, 1f);
                chargeCircle.DOColor(baseColor, 0.5f);
            });
        });
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (!train.CanPerformTask(task)) return;
        if (train.IsTaskComplete(task)) return;

        // Start task
        LogFormatted("ing"); // Log "Cleaning!" for example
        PerformTask();
    }

    void OnTriggerStay(Collider other)
    {
        //TODO: Check if the condition is met to start the task while the player is still in the collider
    }

    void PerformTask()
    {
        chargeCircle.DOColor(completedColor, TimeToCompleteTask);
        chargeCircle.DOFillAmount(1, TimeToCompleteTask).OnComplete(CompleteTask);
    }

    void CompleteTask()
    {
        train.SetTaskStatus(task);
        LogFormatted("ed");
        onTaskPerformed?.Invoke();

        // If the task is still not complete, wait for onTaskCompleteSequence (animation) to finish and then start the task again
        if (!train.IsTaskComplete(task))
        {
            onTaskCompleteSequence.OnComplete(PerformTask);
        }
        else
        {
            onTaskComplete?.Invoke(task);
        }
    }

    void OnTriggerExit(Collider other)
    {
        chargeCircle.fillAmount = 0;
    }

    #region Utility
    void LogFormatted(string append)
    {
        if (task.ToString().EndsWith("e"))
        {
            string formattedString = task.ToString()[..(task.ToString().Length - 1)];
            Debug.Log($"{formattedString}{append}!");
        }
    }
    #endregion
}
