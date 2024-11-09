using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("The entire UI panel for the dialogue system")]
    public GameObject dialogueUIPanel;

    [Tooltip("The text component where dialogue lines will be displayed")]
    public TMP_Text dialogueText;

    [Tooltip("The image component for the speaker's profile picture")]
    public Image profileImage;

    [Tooltip("The audio source for playing voice-over clips")]
    public AudioSource dialogueAudioSource;

    private Queue<Dialogue> dialogueQueue = new Queue<Dialogue>();
    private Dialogue currentDialogue;
    private int currentLineIndex;
    private GameObject highlightedObject;
    private ShipScanner scanner => GameManager.Instance.ShipScanner;

    [SerializeField]
    private Dialogue testDialogue;

    private void Start()
    {
        if (testDialogue != null)
        {
            StartDialogue(testDialogue);
        }
    }

    // Function to start dialogue
    public void StartDialogue(Dialogue dialogue, GameObject highlightTarget = null)
    {
        if (currentDialogue != null)
        {
            // If there's already a dialogue running, queue the new one
            dialogueQueue.Enqueue(dialogue);
            Debug.Log("Dialogue queued: " + dialogue.name);
            return;
        }

        BeginDialogue(dialogue, highlightTarget);
    }

    // Function to actually begin a dialogue
    private void BeginDialogue(Dialogue dialogue, GameObject highlightTarget = null)
    {
        currentDialogue = dialogue;
        currentLineIndex = 0;
        highlightedObject = highlightTarget;

        // Use the ShipScanner on the GameManager if it exists and the highlighted object still exists
        if (GameManager.Instance != null)
        {
            if (scanner != null && highlightedObject != null && highlightedObject.activeInHierarchy)
            {
                scanner.HighlightObject(highlightedObject);
            }
        }
        else
        {
            Debug.LogWarning("GameManager or ShipScanner not found, skipping highlight logic.");
        }

        dialogueUIPanel.SetActive(true);
        Debug.Log("Starting dialogue with: " + currentDialogue.name);
        DisplayNextLine();
    }

    // Function to display the next line of dialogue
    public void DisplayNextLine()
    {
        if (currentDialogue == null || currentLineIndex >= currentDialogue.dialogueLines.Length)
        {
            EndDialogue();
            return;
        }

        DialogueLine line = currentDialogue.dialogueLines[currentLineIndex];
        dialogueText.text = line.dialogueText;
        profileImage.sprite = line.speakerProfilePicture;

        if (line.voiceOverClip != null)
        {
            dialogueAudioSource.clip = line.voiceOverClip;
            dialogueAudioSource.Play();
        }

        StartCoroutine(DisplayLineForDuration(line.timing));
        currentLineIndex++;
    }

    // Coroutine to wait for the timing duration before displaying the next line
    private IEnumerator DisplayLineForDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        DisplayNextLine();
    }

    // Function to end the dialogue
    public void EndDialogue()
    {
        dialogueUIPanel.SetActive(false);
        Debug.Log("Dialogue ended.");

        // Use the ShipScanner on the GameManager if it exists and the highlighted object still exists
        if (GameManager.Instance != null)
        {
            if (scanner != null && highlightedObject != null && highlightedObject.activeInHierarchy)
            {
                scanner.HighlightObject(null);
            }
        }
        else
        {
            Debug.LogWarning("GameManager or ShipScanner not found, skipping highlight logic.");
        }

        currentDialogue = null;
        highlightedObject = null;

        // Check if there are any queued dialogues
        if (dialogueQueue.Count > 0)
        {
            Dialogue nextDialogue = dialogueQueue.Dequeue();
            Debug.Log("Starting next queued dialogue: " + nextDialogue.name);
            BeginDialogue(nextDialogue);
        }
    }
}
