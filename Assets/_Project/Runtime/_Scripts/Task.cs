using System;
using System.Collections;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Custom.Attributes;
using UnityEngine.Events;
using UnityEngine.UI;
using VInspector;

public enum Tasks
{
    Clean,
    Refuel,
    Repair,
    Recharge,
}

[Author("Alex")]
[DisallowMultipleComponent, RequireComponent(typeof(BoxCollider))]
public class Task : MonoBehaviour
{
    #pragma warning disable 0414

    #region vInspector (hide)
    [HideInInspector, UsedImplicitly, Tooltip("DO NOT REMOVE! This is used by VInspector")]
    public VInspectorData vInspectorData;
    #endregion
    
    [SerializeField] Tasks task = Tasks.Refuel;

    #region Collider
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
    #endregion
    
    #region Task
    [Tab("Task")]
    [Header("Timers")]
    [Tooltip("This is just a spacer variable because of how VInspector works")]
    //[SerializeField, ReadOnly, TextArea] string spacer;
    [ShowIf(nameof(task), Tasks.Clean)]
    [RangeResettable(1, 10)]
    [SerializeField] float cleanTime = 5f;
    [ShowIf(nameof(task), Tasks.Refuel)]
    [RangeResettable(1, 10)]
    [SerializeField] float refuelTime = 5f;
    [ShowIf(nameof(task), Tasks.Repair)]
    [RangeResettable(1, 10)]
    [SerializeField] float repairTime = 5f;
    [ShowIf(nameof(task), Tasks.Recharge)]
    [RangeResettable(1, 10)]
    [SerializeField] float rechargeTime = 5f;
    [EndIf]
    
    [Tooltip("The amount of time to wait after the task is complete before starting the task again")]
    [RangeResettable(0.1f, 5f)]
    [SerializeField] float waitOnTaskCompletion = 1f;
    [Space(15)]
    [SerializeField] UnityEvent onTaskPerformed;
    [SerializeField] UnityEvent<Tasks> onTaskComplete;
    #endregion

    #region UI
    [Space(10)]
    [Tab("UI")]
    [SerializeField] Image chargeCircle;
    [ColorUsage(false, true)]
    [SerializeField] Color completedColor = Color.green;
    [ColorUsage(false, true)]
    [SerializeField] Color baseColor = Color.grey;
    #endregion

    // <- Cached Components ->
    
    Train train;
    Sequence onTaskCompleteSequence;
    
    // <- Properties ->
    
    public float TimeToCompleteTask => task switch
    { Tasks.Clean    => cleanTime,
      Tasks.Refuel   => refuelTime,
      Tasks.Repair   => repairTime,
      Tasks.Recharge => rechargeTime,
      _                   => 0 
    };
    
    #pragma warning restore 0414

    void OnValidate()
    {
        name = task.ToString();
        string[] validNames = Enum.GetNames(typeof(Tasks));
        if (name != validNames[(int) task])
        {
            Logger.LogError($"Name must be the same as the task: {string.Join(", ", validNames)}");
            name = task.ToString();
        }
        
        // move component to top
#if UNITY_EDITOR
        if (Application.isPlaying) return;
        UnityEditorInternal.ComponentUtility.MoveComponentUp(this);
#endif
        
        var col = GetComponent<BoxCollider>();
        col.size = new (width, height, depth);
        col.center = new (offsetX, offsetY);
    }
    
    Coroutine taskCoroutine;

    void Awake() => train = GetComponentInParent<Train>();
    
    void Start()
    {
        Debug.Assert(chargeCircle != null, "Charge circle is null");
        chargeCircle.color = baseColor;
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

        taskCoroutine = StartCoroutine(PerformTask());
    }

    void PerformPerformTask() => taskCoroutine = StartCoroutine(PerformTask());

    IEnumerator PerformTask()
    {
        chargeCircle.color = completedColor;
        float elapsedTime = 0f;

        while (elapsedTime < TimeToCompleteTask)
        {
            if (!train.CanPerformTask(task))
            {
                chargeCircle.DOKill();
                chargeCircle.color      = baseColor;
                chargeCircle.fillAmount = 0;
                yield break; // Exit the coroutine
            }

            elapsedTime += Time.deltaTime;
            chargeCircle.fillAmount =  elapsedTime / TimeToCompleteTask;
            yield return null;
        }

        CompleteTask();
    }

    void CompleteTask()
    {
        LogFormatted();
        train.SetTaskStatus(task);
        onTaskPerformed?.Invoke();

        // If the task is still not complete, wait for onTaskCompleteSequence (animation) to finish and then start the task again
        if (!train.IsTaskComplete(task))
        {
            onTaskCompleteSequence.OnComplete(PerformPerformTask);
        }
        else
        {
            onTaskComplete?.Invoke(task);
            
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (taskCoroutine != null) StopCoroutine(taskCoroutine);
        
        chargeCircle.DOKill();
        chargeCircle.color      = baseColor;
        chargeCircle.fillAmount = 0;
    }

    #region Utility
    void LogFormatted()
    {
        var taskName = task.ToString();
        switch (taskName.EndsWith("e") ? taskName : taskName + "e")
        {
            case "Clean":
                Logger.Log("Cleaning!");
                break;
            
            case "Refuel":
                Logger.Log("Refueling!");
                break;
            
            case "Repair":
                Logger.Log("Repairing!");
                break;
            
            case "Recharge":
                Logger.Log("Recharging!");
                break;
        }
      
    }
    #endregion
}
