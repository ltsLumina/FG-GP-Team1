#region
using System;
using System.Collections;
using DG.Tweening;
using JetBrains.Annotations;
using Lumina.Essentials.Modules;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Custom.Attributes;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using VInspector;
#endregion

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
    [ShowIf(nameof(task), Tasks.Clean)]
    [RangeResettable(1, 10)]
    [SerializeField] float cleanTime = 5f;
    [ShowIf(nameof(task), Tasks.Refuel)]
    [RangeResettable(0, 10)]
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
    #endregion

    // <- Cached Components ->
    
    Train train;
    Coroutine taskCoroutine;
    bool isInTrigger;
    InputAction repairAction;
    
    void Awake() => train = GetComponentInParent<Train>();
    
    void OnDisable() => repairAction.started -= HandleInteract;

    void Start()
    {
        taskCoroutine = null;
        chargeCircle.fillAmount = 0;

        onTaskPerformed.AddListener
        (() =>
        {
            chargeCircle.color = completedColor;

            DOTween.Sequence().AppendInterval(waitOnTaskCompletion).OnComplete
            (() =>
            {
                chargeCircle.DOFillAmount(0, 1f);
            });
        });
        
        repairAction ??= this.FindPlayer(1).PlayerInput.actions["Repair"];
        
    }
    
    bool performingTask => taskCoroutine != null;

    void HandleInteract(InputAction.CallbackContext context)
    {
        if (!isInTrigger) return;
        
        switch (context.phase)
        {
            case InputActionPhase.Started: {
                if (!performingTask && train.CanPerformTask(task) && !train.IsTaskComplete(task)) 
                    StartTask();
                break;
            }

            case InputActionPhase.Canceled: {
                if (performingTask)
                    CancelTask();
                break;
            }

            default:
                Debug.Log("Invalid InputActionPhase");
                break;
        }
    }

    void StartTask()
    {
        var player = Helpers.Find<Player>();
        player.PlayerAnimation.Animator.SetTrigger("StartRepairing");
        player.transform.DORotate(new (player.transform.rotation.x, 120, player.transform.rotation.z), 0.5f);
        
        chargeCircle.color = completedColor;
        this.FindPlayer(1).Freeze(true);
        taskCoroutine = StartCoroutine(PerformTask());
    }

    void CancelTask()
    {
        this.FindPlayer(1).PlayerAnimation.Animator.SetTrigger("StopRepairing");
        var player = Helpers.Find<Player>();
        player.transform.DORotate(new (player.transform.rotation.x, 180, player.transform.rotation.z), 0.5f);
        
        if (taskCoroutine != null)
        {
            StopCoroutine(taskCoroutine);
            taskCoroutine = null;
        }

        chargeCircle.DOKill();
        chargeCircle.fillAmount = 0;

        this.FindPlayer(1).Freeze(false);
    }

    IEnumerator PerformTask()
    {
        float elapsedTime = 0f;
        while (elapsedTime < repairTime)
        {
            if (!train.CanPerformTask(task) || !repairAction.IsPressed())
            {
                CancelTask();
                yield break;
            }

            elapsedTime             += Time.deltaTime;
            chargeCircle.fillAmount =  elapsedTime / repairTime;
            yield return null;
        }

        CompleteTask();
    }

    void CompleteTask()
    {
        this.FindPlayer(1).PlayerAnimation.Animator.SetTrigger("StopRepairing");
        var player = Helpers.Find<Player>();
        player.transform.DORotate(new (player.transform.rotation.x, 180, player.transform.rotation.z), 0.5f);
        
        taskCoroutine = null;
        train.SetTaskStatus(task);
        onTaskPerformed?.Invoke();

        this.FindPlayer(1).Freeze(false);
        
        if (train.IsTaskComplete(task)) onTaskComplete?.Invoke(task);
        else StartTask();
    }

    void OnTriggerEnter(Collider other)
    {
        switch (task)
        {
            case Tasks.Refuel:
                if (Player.HoldingResource(out Resource heldResource) && heldResource.Item == IGrabbable.Items.Kelp && task == Tasks.Refuel)
                {
                    train.SetTaskStatus(Tasks.Refuel);
                    Destroy(heldResource.gameObject);
                }

                // Prevents non-grabbed resources such as base kelp, for instance, from being destroyed.
                if (other.TryGetComponent(out Resource resource) && resource.Item == IGrabbable.Items.Kelp && resource.HasBeenGrabbed)
                {
                    train.SetTaskStatus(Tasks.Refuel);
                    Destroy(resource.gameObject);
                }
                break;
            
            case Tasks.Repair:
                repairAction         =  this.FindPlayer(1).PlayerInput.actions["Repair"];
                isInTrigger          =  true;
                repairAction.started += HandleInteract;
                break;
            
            case Tasks.Recharge:
                // Recharge
            break;
        }
    }

    void OnTriggerExit(Collider other)
    {
        isInTrigger = false;
        repairAction.started -= HandleInteract;
    }

    void OnValidate()
    {
        var collider = GetComponent<BoxCollider>();
        collider.size = new (width, height, depth);
        collider.center = new (offsetX, offsetY);
    }
}
