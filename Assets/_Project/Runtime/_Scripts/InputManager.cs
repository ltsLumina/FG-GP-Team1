using Lumina.Essentials.Modules;
using UnityEngine;
using UnityEngine.Custom.Attributes;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

[Author("Alex")]
public class InputManager : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; }

    Player player;

    void Start() => player = this.GetParentComponent<Player>();

    public void OnMove(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>(); 
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        player.Dash();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        switch (context.interaction)
        {
            case TapInteraction:
                Debug.Log("Tap Interaction" + "\nGrabbing Item");
                player.Grab();
                break;

            case HoldInteraction:
                Debug.Log("Hold Interaction" + "\nReleasing Item");
                player.Release();
                break;
        }
    }
}
