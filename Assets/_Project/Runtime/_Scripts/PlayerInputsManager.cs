using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Custom.Attributes;
using UnityEngine.InputSystem;

[Author("Alex")]
public class PlayerInputsManager : PlayerInputManager
{
    public static List<Player> Players { get; private set; } = new ();
    
    public static Player Player1 => Players.Count == 0 ? null : Players[0];
    public static Player Player2 => Players.Count == 1 ? null : Players[1];
    
    void Awake()
    {
        onPlayerJoined += OnPlayerJoined;
        onPlayerLeft += OnPlayerLeft;
    }

    void Start() 
    {
        // If there is a player already in the scene (likely for debugging purposes), 
        PlayerInput[] existingPlayersInScene = FindObjectsByType<PlayerInput>(FindObjectsSortMode.InstanceID);
        foreach (PlayerInput player in existingPlayersInScene) { OnPlayerJoined(player); }
    }

    void OnPlayerJoined(PlayerInput obj)
    {
        var player = obj.GetComponentInParent<Player>();
        player.PlayerID = obj.playerIndex;
        
        Players.Add(player);
    }

    void OnPlayerLeft(PlayerInput obj)
    {
        Players.Remove(obj.GetComponentInParent<Player>());
    }
}
