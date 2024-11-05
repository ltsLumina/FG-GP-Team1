using UnityEngine;
using UnityEngine.Custom.Attributes;
using UnityEngine.InputSystem;

[Author("Alex")]
public class InputManager : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; }

    public void OnMove(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>(); 
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        var player = GetComponentInParent<Player>();
        player.Dash();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        Debug.Log("Action");
        var player = GetComponentInParent<Player>();
        player.Grab();
    }
}
