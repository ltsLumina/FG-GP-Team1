using UnityEngine;

public class DialogueEvents : MonoBehaviour
{
    [SerializeField]
    private DialogueManager dialogueManager;

    [SerializeField]
    private Dialogue introDialogue;

    private void Awake()
    {
        if (dialogueManager == null)
        {
            dialogueManager = FindObjectOfType<DialogueManager>();
        }
    }

    private void Start()
    {
        // Subscribe to GameManager events
        // Game Intro
        // Game First Started
        // Game First Play
        // First Kelp on Screen
        // First Rock on Screen
        // First Rock Hits SHIP
        // First Hull Breach Pressure Event
        // First Critical Hull Damage
        // First Critical Fuel Level
        // First Critical Battery Charge Level
        // First Death From Running Out of Fuel
        // First Death from Hull Destroyed
        // First Lights Out
        // Enter the Deep
        // First Hull Repair
        // First Fuel Refill
        // First Battery Charge
        // First Player Stun
        // First Electric Fish on Screen
    }
}
