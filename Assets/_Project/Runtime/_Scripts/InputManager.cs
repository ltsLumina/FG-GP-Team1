using UnityEngine;
using UnityEngine.Custom.Attributes;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

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

    bool pressed;
    bool held;

    public void OnInteract(InputAction.CallbackContext context)
    {
        var player = GetComponentInParent<Player>();

        if (context.interaction is TapInteraction)
        {
            Debug.Log("Tap Interaction" + "\nGrabbing Item");
            player.Grab();
        }
        
        if (context.interaction is HoldInteraction)
        {
            Debug.Log("Hold Interaction" + "\nReleasing Item");
            player.Release();
        }
    }
}
