#region
using System;
using System.Collections;
using DG.Tweening;
using Lumina.Essentials.Attributes;
using Lumina.Essentials.Modules;
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
    

    float dashTimer;

    // <- Cached Components ->

    InputManager input;
    PlayerInput playerInput;
    Rigidbody rb;
    PlayerAnimation playerAnimation;

    /// <summary>
    /// The player index.
    /// Uses a 1-based index. (Player 1 is 1, Player 2 is 2, etc.)
    /// </summary>
    public int PlayerID
    {
        get => playerID;
        set => playerID = value + 1;
    }

    public PlayerInput PlayerInput => playerInput;

    Action<bool> onDash;
    Action<bool> onDashEnd;
    
    void Awake()
    {
        playerInput = GetComponentInChildren<PlayerInput>();
        input = GetComponentInChildren<InputManager>();
        rb = GetComponent<Rigidbody>();
        playerAnimation = GetComponentInChildren<PlayerAnimation>();
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
        if (dashing) playerAnimation.Dash();
        else playerAnimation.StopDash();
    }
    #endregion

    // Player moves in parallel with the train if it's a child of the train. Simplest solution.
    void Start() => transform.SetParent(Find<Train>().transform);

    void Update() => Move();

    void Move()
    {
        Vector2 dir = input.MoveInput.normalized;
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
        playerAnimation.Dash();
    }

    IEnumerator DashRoutine(Rigidbody rb)
    {
        dashTimer = dashCooldown;

        rb.linearDamping = dashDampingStart;
        var dir = input.MoveInput.normalized;
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
    /// <param name="heldResource"> The held resource. </param>
    /// <returns></returns>
    public static bool HoldingResource(out Resource heldResource)
    {
        if (Player.heldResource == null)
        {
            heldResource = null;
            return false;
        }

        heldResource = Player.heldResource;
        return heldResource is { Grabbed: true };
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

    public static Action OnGrab;
    public static Action OnRelease;
    
    public void Grab()
    {
        if (heldResource != null)
            return;

        var resources = ClosestResources();
        var closest = resources[0];

        if (closest.Reach > Vector3.Distance(transform.position, closest.transform.position))
        {
            closest.Grab();
            OnGrab?.Invoke();
            heldResource = closest;
        }
    }

    public void Release()
    {
        if (heldResource == null)
            return;

        heldResource.Release();
        OnRelease?.Invoke();
        heldResource = null;
    }
}
