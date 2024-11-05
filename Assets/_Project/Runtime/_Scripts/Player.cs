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
    
    // <- Cached Components ->
    
    InputManager input;
    PlayerInput playerInput;

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
    }

    Resource rock;

    void Update()
    {
        Move();
        StayInBounds();
        Rotate();
        
        if (!rock) return;

        if (rock.Grabbed)
        {
            var dir = (rock.transform.position - transform.position).normalized;
            rock.GetComponent<Rigidbody>().AddForce(-dir * 5000, ForceMode.Impulse);
        }
    }

    void Move()
    {
        var dir  = input.MoveInput.normalized;
        var down = diveForce * Time.deltaTime;
        //transform.position += new Vector3(dir.x, dir.y - down) * (moveSpeed * Time.deltaTime);
        var rb = GetComponent<Rigidbody>();
        rb.AddForce(new Vector3(dir.x, dir.y - down) * (moveSpeed * Time.deltaTime), ForceMode.Acceleration);

        // var trainPosY = Find<Train>().transform.position.y;
        //
        // if (transform.position.y < trainPosY + 5)
        // {
        //     transform.position = new Vector3(transform.position.x, trainPosY + 5);
        // }
    }

    public void Dash()
    {
        Debug.Log("Dash");
        StartCoroutine(DashRoutine(GetComponent<Rigidbody>())); // TODO: Cache
    }

    IEnumerator DashRoutine(Rigidbody rb)
    {
        rb.linearDamping = dashDampingStart;
        var dir = input.MoveInput.normalized;
        rb.AddForce(dir * dashForce, ForceMode.Impulse);
        yield return new WaitForSeconds(dashDuration);
        DOVirtual.Float(rb.linearDamping, dashDampingEnd, dashDuration, x => rb.linearDamping = x);
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
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, rock.transform.position);
    }
}
