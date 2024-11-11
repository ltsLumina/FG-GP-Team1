using System;
using Lumina.Essentials.Modules;
using UnityEngine;
using UnityEngine.Custom.Attributes;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

[Author("Alex")]
public class InputManager : MonoBehaviour
{
    Player player;
    
    public Vector2 MoveInput { get; private set; }

    void Start() => player = this.GetParentComponent<Player>();
    
    public static Action OnTapInteraction;
    public static Action OnHoldInteraction;

    public void OnMove(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>(); 
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        player.Dash();
    }
    
    public void OnGrabAndRelease(InputAction.CallbackContext context)
    {
        switch (context.interaction)
        {
            case TapInteraction:
                Debug.Log("Tap Interaction" + "\nGrabbing Item");
                OnTapInteraction?.Invoke();
                player.Grab();
                break;

            case HoldInteraction:
                Debug.Log("Hold Interaction" + "\nReleasing Item");
                OnHoldInteraction?.Invoke();
                player.Release();
                break;
        }
    }
    
    public void OnRepair(InputAction.CallbackContext context)
    {
        OnHoldInteraction?.Invoke();
    }
}
