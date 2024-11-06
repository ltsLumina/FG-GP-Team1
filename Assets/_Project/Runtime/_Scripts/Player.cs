#region
using System.Collections;
using DG.Tweening;
using Lumina.Essentials.Attributes;
using UnityEngine;
using UnityEngine.Custom.Attributes;
using UnityEngine.InputSystem;
using static Lumina.Essentials.Modules.Helpers;
#endregion

[Author("Alex", "alexander.andrejeff@edu.futuregames.se"), DisallowMultipleComponent]
public class Player : MonoBehaviour
{
    [Header("Multiplayer")]
    [SerializeField, ReadOnly] int playerID;
    
    [Header("Movement")]
    [SerializeField] float moveSpeed;
    [Tooltip("The amount of push force applied when the player dives. \nA higher value will make it tougher to dive down.")]
    [SerializeField] float diveForce = 15f;
    
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

    void Awake()
    {
        playerInput = GetComponentInChildren<PlayerInput>();
        input       = GetComponentInChildren<InputManager>();
        rb          = GetComponent<Rigidbody>();
    }

    Resource rock;

    void Update()
    {
        Move();
        Dive();
        StayInBounds();
        Rotate();
        
        if (!rock) return;
        if (rock.Grabbed)
        {
            var dir = (rock.transform.position - transform.position).normalized;
            rock.GetComponent<Rigidbody>().AddForce(-dir * 5000, ForceMode.Impulse);
        }
    }

    void Dive()
    {
        var down = Vector3.down * (diveForce * Time.deltaTime);
        rb.AddForce(down, ForceMode.Force);
    }

    void Move()
    {
        var dir  = input.MoveInput.normalized;
        rb.AddForce(new Vector3(dir.x, dir.y * 1.5f) * (moveSpeed * Time.deltaTime), ForceMode.Acceleration);
    }

    public void Dash()
    {
        if (dashTimer > 0) return;
        StartCoroutine(DashRoutine(rb));
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
    }

    public void Grab()
    {
        var resource = Find<Resource>();
        resource.Grab();
    }

    void StayInBounds()
    {
        switch (transform.position.x)
        {
            case < -25:
                transform.position = new (-25, transform.position.y);
                break;

            case > 25:
                transform.position = new (25, transform.position.y);
                break;
        }
    }

    void Rotate()
    {
        
    }

    void OnDrawGizmos()
    {
        // draw line to nearest rock
        var rock = Find<Rock>();
        if (!rock) return;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, rock.transform.position);
    }
}
