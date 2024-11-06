using System;
using System.Collections;
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
        [UsedImplicitly] Kelp,
        [UsedImplicitly] Battery,
    }
}

public class Resource : MonoBehaviour, IGrabbable
{
    [SerializeField] IGrabbable.Items item;
    [SerializeField, ReadOnly] bool grabbed;
    
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
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    void Destroy() => Destroy(gameObject);

    public void Grab()
    {
        CancelInvoke(nameof(Destroy));
        
        grabbed = true;
        onGrabbed?.Invoke();
    }
    
    public void Release()
    {
        Invoke(nameof(Destroy), Lifetime);
        
        grabbed = false;
        onReleased?.Invoke();
    }

    void Start()
    {
        if (Lifetime <= 5) Debug.LogWarning("Lifetime is set too low. Object will likely be destroyed before it has left the screen bounds.");
        Invoke(nameof(Destroy), Lifetime);
    }

    void Update()
    {
        if (!Grabbed) return;
        
        var player = Find<Player>();
        var moveInput = player.GetComponentInChildren<InputManager>().MoveInput;
        var offset = new Vector3(moveInput.x * 3f, moveInput.y * 3f);
        if (moveInput == Vector2.zero) offset = new (2, 2);

        transform.position = player.transform.position + offset;
    }
    
    void SetMesh(bool useGrabbedMesh)
    {
        var regularMesh = transform.GetChild(0);
        var grabbedMesh = transform.GetChild(1);
        
        regularMesh.gameObject.SetActive(!useGrabbedMesh);
        grabbedMesh.gameObject.SetActive(useGrabbedMesh);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, reach);
    }
}
