using UnityEngine;
using UnityEngine.Custom.Attributes;
using UnityEngine.InputSystem;

[Author("Alex")]
public class PlayerInputsManager : PlayerInputManager
{
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
    }

    void OnPlayerLeft(PlayerInput obj) { }
}
