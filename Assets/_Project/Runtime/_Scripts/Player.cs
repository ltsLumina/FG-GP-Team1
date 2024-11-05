#region
using Lumina.Essentials.Attributes;
using UnityEngine;
using UnityEngine.Custom.Attributes;
using UnityEngine.InputSystem;
using static Lumina.Essentials.Modules.Helpers;
#endregion

[Author("Alex", "alexander.andrejeff@edu.futuregames.se"), DisallowMultipleComponent]
public class Player : MonoBehaviour
{
    [SerializeField, ReadOnly] int playerID;
    [SerializeField] float moveSpeed;
    [Tooltip("The amount of push force applied when the player dives. \nA higher value will make it tougher to dive down.")]
    [SerializeField] float diveForce;
    [SerializeField] float dashForce;
    
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

    void Update()
    {
        Move();
        StayInBounds();
        
        // var rock = Find<Rock>();
        // var dir = (rock.transform.position - transform.position).normalized;
        // rock.GetComponent<Rigidbody>().AddForce(-dir * 50, ForceMode.Acceleration);
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
        
        // dash in moveinput direction with a force of 10
        var dir = input.MoveInput.normalized;
        var rb  = GetComponent<Rigidbody>();
        rb.AddForce(dir * dashForce, ForceMode.Impulse);
    }

    void Grab()
    {
        
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

    void OnDrawGizmos()
    {
        // draw line to nearest rock
        var rock = Find<Rock>();
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, rock.transform.position);
    }
}
