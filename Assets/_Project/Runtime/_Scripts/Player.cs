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
using Object = UnityEngine.Object;
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
    public Animator Animator => PlayerAnimation.Animator;

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

    float blinkTimer;
    
    void Update()
    {
        Move();
        
        // blink every 5 seconds if the animator is playing the idle animation
        if (Animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            blinkTimer += Time.deltaTime;
            if (blinkTimer >= 5)
            {
                Animator.SetTrigger("Blink");
                blinkTimer = 0;
            }
        }
        
        Animator.SetBool("Grabbing", heldResource != null);
    }

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
        PlayerAnimation.Dash();
        StartCoroutine(DashRoutine(rb));
        onDash?.Invoke(true);
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
        }

        heldResource.Release();
        heldResource = null;
    }

    void OnCollisionEnter(Collision other)
    {
        Animator.SetTrigger("Collide");
    }

    public void Stun() => StartCoroutine(StunRoutine());

    IEnumerator StunRoutine()
    {
        Animator.SetTrigger("Stunned");
        
        Freeze(true);
        yield return new WaitForSeconds(stunDuration);
        Freeze(false);
    }
}

public static class PlayerExtensions
{
    public static void DoForEachPlayer(this MonoBehaviour _, Action<Player> action)
    {
        int playerCount = PlayerInputManager.instance.playerCount;
        if (playerCount == 1)
        {
            var player = Object.FindAnyObjectByType<Player>();
            action(player);
        }
        else
        {
            Player[] players = FindMultiple<Player>();
            foreach (Player player in players) { action(player); }
        }
    }
    
    public static Player FindPlayer(this MonoBehaviour _, int playerID)
    {
        var players = FindMultiple<Player>();
        foreach (var player in players)
        {
            if (player.PlayerID == playerID) return player;
        }

        return null;
    }
}
