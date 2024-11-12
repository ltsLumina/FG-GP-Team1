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

    void Start()
    {
        player = this.GetParentComponent<Player>();
        
        // Sometimes these actions will not be assigned when instantiating the player prefab upon joining the game.
        Debug.Assert(player.PlayerInput.actions.FindAction("Move") != null, "Move action is not assigned!", this);
        Debug.Assert(player.PlayerInput.actions.FindAction("Dash") != null, "Dash action is not assigned!", this);
        Debug.Assert(player.PlayerInput.actions.FindAction("GrabAndRelease") != null, "Grab and Release action is not assigned!", this);
        Debug.Assert(player.PlayerInput.actions.FindAction("Repair") != null, "Repair action is not assigned!", this);
    }

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
                OnTapInteraction?.Invoke();
                player.Grab();
                break;

            case HoldInteraction:
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
