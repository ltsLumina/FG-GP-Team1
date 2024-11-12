using System;
using Lumina.Essentials.Attributes;
using UnityEngine;
using static Lumina.Essentials.Modules.Helpers;

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
    [SerializeField] float throwForce;
    
    [Range(1,60)]
    [SerializeField] float lifetime = 10f;

    Action onGrabbed;
    Action onReleased;

    public IGrabbable.Items Item => item;

    public bool Grabbed => grabbed;

    public float Reach => reach;

    public float Lifetime => lifetime;

    public bool Bypass { get; private set; }

    // Duct-tape fix
    bool GrabbedMeshActive => grabbedMesh.gameObject.activeSelf;
    public bool HasBeenGrabbed => GrabbedMeshActive;

    Player currentPlayer;
    
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
            Throw(currentPlayer);
            currentPlayer = null;
        };
    }

    void OnDisable()
    {
        onGrabbed -= ResetVelocity;
    }

    void ResetVelocity()
    {
        TryGetComponent(out Rigidbody rb);
        if (rb == null) return;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    void Throw(Player player)
    {
        TryGetComponent(out Rigidbody rb);
        if (rb == null || !player) return;
        var moveInput = player.InputManager.MoveInput;
        if (moveInput == Vector2.down)
        {
            const float y = 8f; // don't change
            transform.position += Vector3.down * y;
            rb.AddForce(Vector3.down * throwForce, ForceMode.Impulse);
        }
        else
        {
            rb.AddForce(moveInput * throwForce, ForceMode.Impulse);
        }
    }

    public void Grab(Player player)
    {
        grabbed = true;
        currentPlayer = player;
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
        if (!Grabbed || currentPlayer == null) return;

        var moveInput = currentPlayer.InputManager.MoveInput;
        var offset    = new Vector3(moveInput.x * 2f, 3.5f);

        transform.position = currentPlayer.transform.position + offset;
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
