using System;
using DG.Tweening;
using Lumina.Essentials.Attributes;
using Lumina.Essentials.Modules;
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

    static GameObject container;
    
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
            Throw(currentPlayer);
            currentPlayer = null; // Set to null after throwing.
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
        rb.isKinematic = false;
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

        if (Item != IGrabbable.Items.Battery)
        {
            const string fuelModelName = "FuelModel";
            var fuelModel = GameObject.Find(fuelModelName);
            transform.position = fuelModel.transform.position;
            grabbedMesh.gameObject.SetActive(false);
            fuelModel.GetComponent<MeshRenderer>().enabled = true;
        }
    }
    
    public void Release()
    {
        if (Item != IGrabbable.Items.Battery)
        {
            const string fuelModelName = "FuelModel";
            var fuelModel = GameObject.Find(fuelModelName);

            fuelModel.GetComponent<MeshRenderer>().enabled = false;
            grabbedMesh.gameObject.SetActive(true);
        }
        
        grabbed = false;
        onReleased?.Invoke();
        
        Find<Player>().PlayerAnimation.ObjectReleased();
    }

    void Start()
    {
        if (Item != IGrabbable.Items.Battery)
        {
            Debug.Assert(standardMesh != null, "Standard mesh is not set. Please set it in the inspector.", this);
            Debug.Assert(grabbedMesh != null, "Grabbed mesh is not set. Please set it in the inspector.", this);
        }
        else if (Item == IGrabbable.Items.Kelp)
        {
            container = GameObject.Find("Resource Container");
            if (container == null) container = new GameObject("Resource Container");
            transform.SetParent(container.transform);
        }
        
        if (Lifetime <= 5) Debug.LogWarning("Lifetime is set too low. Object will likely be destroyed before it has left the screen bounds.");
        Bypass = item == IGrabbable.Items.Battery; // Don't destroy the battery. (obviously, lol)
    }

    void Update()
    {
        if (Grabbed)
        {
            const string fuelModelName = "FuelModel";
            var fuelModel = GameObject.Find(fuelModelName);
            transform.position = fuelModel.transform.position;
        }
    }

    void SetMesh(bool useGrabbedMesh)
    {
        if (Item == IGrabbable.Items.Battery) return;
        
        standardMesh.gameObject.SetActive(!useGrabbedMesh);
        grabbedMesh.gameObject.SetActive(useGrabbedMesh);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, reach);
    }
}
