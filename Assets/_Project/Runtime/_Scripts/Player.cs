#region
using System;
using System.Collections;
using DG.Tweening;
using Lumina.Essentials.Attributes;
using Lumina.Essentials.Modules;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.Custom.Attributes;
using UnityEngine.InputSystem;
using static Lumina.Essentials.Modules.Helpers;
#endregion

[Author("Alex", "alexander.andrejeff@edu.futuregames.se"), DisallowMultipleComponent]
public class Player : MonoBehaviour
{
    [Header("Multiplayer")]
    [SerializeField] [ReadOnly] int playerID;

    [Header("Movement")]
    [SerializeField] float moveSpeed;

    [Header("Dash")]
    [SerializeField] float dashForce = 25f;
    [SerializeField] float dashDuration = 0.05f;
    [SerializeField] float dashDampingStart;
    [SerializeField] float dashDampingEnd = 2.5f;
    [SerializeField] float dashCooldown = 1f;

    [Header("Stun")]
    [SerializeField] float stunDuration = 2f;

    float dashTimer;
    bool canMove = true;

    // <- Cached Components ->
    
    Rigidbody rb;

    /// <summary>
    /// The player index.
    /// Uses a 1-based index. (Player 1 is 1, Player 2 is 2, etc.)
    /// </summary>
    public int PlayerID
    {
        get => playerID;
        set => playerID = value + 1;
    }

    public PlayerInput PlayerInput { get; private set; }
    public InputManager InputManager { get; private set; }
    public PlayerAnimation PlayerAnimation { get; private set; }

    Action<bool> onDash;
    Action<bool> onDashEnd;
    
    void Awake()
    {
        canMove = true;
        
        rb = GetComponent<Rigidbody>();
        PlayerInput = GetComponentInChildren<PlayerInput>();
        InputManager = GetComponentInChildren<InputManager>();
        PlayerAnimation = GetComponentInChildren<PlayerAnimation>();
    }

    #region Dash Animation Helper
    void OnEnable()
    {
        onDash += DashAnims;
        onDashEnd += DashAnims;
    }

    void OnDisable()
    {
        onDash -= DashAnims;
        onDashEnd -= DashAnims;
    }

    void DashAnims(bool dashing)
    {
        if (dashing) PlayerAnimation.Dash();
        else PlayerAnimation.StopDash();
    }
    #endregion

    // Player moves in parallel with the train if it's a child of the train. Simplest solution.
    void Start()
    {
        transform.SetParent(Train.Instance.transform);
        transform.position = transform.parent.position + new Vector3(-5, -5, 0);
        transform.rotation = new Quaternion(0, 180, 0, 0); // i dont know why but this must be done
    }

    void Update() => Move();

    public void Freeze(bool freeze) => canMove = !freeze;

    
    void Move()
    {
        if (!canMove) return;
        Vector2 dir = InputManager.MoveInput.normalized;
        rb.AddForce(
            new Vector3(dir.x, dir.y) * (moveSpeed * Time.deltaTime),
            ForceMode.Acceleration
        );
    }

    public void Dash()
    {
        if (dashTimer > 0) return;
        StartCoroutine(DashRoutine(rb));
        onDash?.Invoke(true);
        PlayerAnimation.Dash();
    }

    IEnumerator DashRoutine(Rigidbody rb)
    {
        dashTimer = dashCooldown;

        rb.linearDamping = dashDampingStart;
        var dir = InputManager.MoveInput.normalized;
        rb.AddForce(dir * dashForce, ForceMode.Impulse);
        yield return new WaitForSeconds(dashDuration);
        DOVirtual.Float(dashDampingStart, dashDampingEnd, dashDuration, x => rb.linearDamping = x);
        while (dashTimer > 0)
        {
            dashTimer -= Time.deltaTime;
            yield return null;
        }

        onDashEnd?.Invoke(false);
    }

    static Resource heldResource;

    /// <summary>
    /// Check if the player is holding an item.
    /// </summary>
    /// <param name="resource"> The held resource. </param>
    /// <returns></returns>
    public static bool HoldingResource(out Resource resource)
    {
        if (heldResource == null)
        {
            resource = null;
            return false;
        }

        resource = heldResource;
        return resource is { Grabbed: true };
    }

    public static bool HoldingResource(out Battery battery)
    {
        if (heldResource == null)
        {
            battery = null;
            return false;
        }
        
        battery = heldResource.GetComponent<Battery>();
        return battery != null;
    }
    
    /// <summary>
    ///     Find the closest resource to the player.
    /// </summary>
    /// <returns> An array of resources sorted by distance to the player.
    ///     <para> The closest resource is at index 0. </para>
    /// </returns>
    Resource[] ClosestResources()
    {
        Resource[] resources = FindMultiple<Resource>();
        Array.Sort(
            resources,
            (a, b) =>
                Vector3
                    .Distance(a.transform.position, transform.position)
                    .CompareTo(Vector3.Distance(b.transform.position, transform.position))
        );

        return resources;
    }

    public void Grab()
    {
        if (heldResource != null) return;

        var resources = ClosestResources();
        var closest = resources[0];

        if (closest.Reach > Vector3.Distance(transform.position, closest.transform.position))
        {
            closest.Grab(this);
            heldResource = closest;
        }
    }

    public void Release()
    {
        if (heldResource == null) return;

        if (heldResource.Item == IGrabbable.Items.Battery)
        {
           if (!heldResource.GetComponent<Battery>().Deposit()) return;

           heldResource.Release();
           heldResource = null;
        }
        else
        {
            heldResource.Release();
            heldResource = null;
        }
    }

    public void Stun() => StartCoroutine(StunRoutine());

    IEnumerator StunRoutine()
    {
        Freeze(true);
        PlayerAnimation.Stun(stunDuration);
        yield return new WaitForSeconds(stunDuration);
        Freeze(false);
    }
}

public static class PlayerExtensions
{
    public static Player FindPlayer(this MonoBehaviour monoBehaviour, int playerID)
    {
        var players = FindMultiple<Player>();
        foreach (var player in players)
        {
            if (player.PlayerID == playerID) return player;
        }

        return null;
    }
}
