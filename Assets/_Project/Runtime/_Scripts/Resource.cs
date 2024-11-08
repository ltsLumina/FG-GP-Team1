using System;
using System.Collections;
using DG.Tweening;
using JetBrains.Annotations;
using Lumina.Essentials.Attributes;
using Unity.VisualScripting;
using UnityEngine;
using VInspector;
using static Lumina.Essentials.Modules.Helpers;
using Object = UnityEngine.Object;

public interface IGrabbable
{
    public enum Items
    {
        Kelp,
        Battery,
    }
}

public class Resource : MonoBehaviour, IGrabbable, IDestructible
{
    [SerializeField] IGrabbable.Items item;
    [SerializeField, ReadOnly] bool grabbed;

    [Header("Mesh")]
    [SerializeField] MeshRenderer standardMesh;
    [SerializeField] MeshRenderer grabbedMesh;
    
    [Header("Settings")]
    [SerializeField] float reach;
    [Range(1,60)]
    [SerializeField] float lifetime = 10f;

    Action onGrabbed;
    Action onReleased;

    public IGrabbable.Items Item => item;

    public bool Grabbed => grabbed;

    public float Reach => reach;

    public float Lifetime => lifetime;

    public bool Bypass { get; set; }

    void OnEnable()
    {
        onGrabbed += () =>
        {
            SetMesh(true);
            ResetVelocity();
        };
        onReleased += () =>
        {
            SetMesh(true);
            ResetVelocity();
        };
    }

    void OnDisable() => onGrabbed -= ResetVelocity;

    void ResetVelocity()
    {
        TryGetComponent(out Rigidbody rb);
        if (rb == null) return;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    public void Grab()
    {
        grabbed = true;
        onGrabbed?.Invoke();
    }
    
    public void Release()
    {
        grabbed = false;
        onReleased?.Invoke();
    }

    void Start()
    {
        Debug.Assert(standardMesh != null, "Standard mesh is not set. Please set it in the inspector.", this);
        Debug.Assert(grabbedMesh != null, "Grabbed mesh is not set. Please set it in the inspector.", this);
        
        if (Lifetime <= 5) Debug.LogWarning("Lifetime is set too low. Object will likely be destroyed before it has left the screen bounds.");
        Bypass = item == IGrabbable.Items.Battery; // Don't destroy the battery. (obviously, lol)
    }

    void Update()
    {
        if (!Grabbed) return;
        
        var player = Find<Player>();
        var moveInput = player.GetComponentInChildren<InputManager>().MoveInput;
        var offset = new Vector3(moveInput.x * 2f, 3f);
        
        transform.position = player.transform.position + offset;
    }
    
    void SetMesh(bool useGrabbedMesh)
    {
        standardMesh.gameObject.SetActive(!useGrabbedMesh);
        grabbedMesh.gameObject.SetActive(useGrabbedMesh);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, reach);
    }
}
