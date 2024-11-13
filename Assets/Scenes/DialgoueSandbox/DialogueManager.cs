using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("The entire UI panel for the dialogue system")]
    public GameObject dialogueUIPanel;

    [Tooltip("The text component where dialogue lines will be displayed")]
    public TMP_Text dialogueText;

    [Tooltip("The image component where the speaker's profile picture will be displayed")]
    public RawImage profileImage;

    [Tooltip("The audio source for playing voice-over clips")]
    public AudioSource dialogueAudioSource;

    [Tooltip("The image used to display static noise over the profile image")]
    public Image staticOverlayImage;

    [Tooltip("Static effect intensity")]
    [Range(0, 1)]
    public float staticIntensity = 0.5f;

    [Tooltip("Static effect duration in seconds")]
    public float staticEffectDuration = 0.2f;

    [Tooltip("The duration each glitch frame lasts")]
    public float glitchFrameDuration = 0.05f;

    [Tooltip("Typing duration modifier (0 = instant, 1 = full duration)")]
    [Range(0, 1)]
    public float typingDuration = 0.5f;

    private Queue<Dialogue> dialogueQueue = new Queue<Dialogue>();
    private Dialogue currentDialogue;
    private int currentLineIndex;
    private GameObject highlightedObject;
    private Scanner scanner => GameManager.Instance.ShipScanner;

    [SerializeField]
    private Dialogue testDialogue;

    private void Start()
    {
        // Ensure the static overlay image is initially hidden
        if (staticOverlayImage != null)
        {
            Color initialOverlayColor = staticOverlayImage.color;
            initialOverlayColor.a = 0f;
            staticOverlayImage.color = initialOverlayColor;
            staticOverlayImage.gameObject.SetActive(true);
        }

        if (testDialogue != null)
        {
            StartDialogue(testDialogue);
        }
    }

    private void Update()
    {
        // Skip the current dialogue line when 'G' is pressed, but only in editor
        if (Application.isEditor && Input.GetKeyDown(KeyCode.G))
        {
            SkipDialogueLine();
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
                scanner.Scan(highlightedObject);
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
        profileImage.texture = line.speakerProfilePicture;

        if (line.voiceOverClip != null)
        {
            dialogueAudioSource.clip = line.voiceOverClip;
            dialogueAudioSource.Play();
        }

        // Start the typing effect coroutine
        StartCoroutine(TypeDialogueText(line.dialogueText, line.timing));

        // Start the static effect coroutine
        StartCoroutine(ApplyStaticEffect(line.timing));
        currentLineIndex++;
    }

    // Coroutine for typing effect
    private IEnumerator TypeDialogueText(string text, float lineDuration)
    {
        dialogueText.text = "";
        float adjustedDuration = lineDuration * typingDuration; // Adjust the total duration based on typingDuration slider
        float typingSpeed = adjustedDuration / text.Length;
        float accumulatedTime = 0f;
        int currentIndex = 0;

        while (currentIndex < text.Length)
        {
            accumulatedTime += Time.unscaledDeltaTime;

            while (accumulatedTime >= typingSpeed && currentIndex < text.Length)
            {
                dialogueText.text += text[currentIndex];
                currentIndex++;
                accumulatedTime -= typingSpeed;
            }

            yield return null; // Wait until the next frame
        }
    }

    // Function to skip the current dialogue line and display the next one
    private void SkipDialogueLine()
    {
        // Stop any running coroutines to ensure the next line displays immediately
        StopAllCoroutines();

        // Stop the voice-over if playing
        if (dialogueAudioSource.isPlaying)
        {
            dialogueAudioSource.Stop();
        }

        // Move to the next line
        DisplayNextLine();
    }

    // Coroutine to apply static effect to profile image
    private IEnumerator ApplyStaticEffect(float lineDuration)
    {
        if (staticOverlayImage == null)
        {
            yield break;
        }

        float elapsedTime = 0f;

        while (elapsedTime < staticEffectDuration)
        {
            // Set a random intensity to simulate glitching
            Color overlayColor = staticOverlayImage.color;
            overlayColor.a = Random.Range(0.2f, staticIntensity);
            staticOverlayImage.color = overlayColor;

            elapsedTime += glitchFrameDuration;
            yield return new WaitForSeconds(glitchFrameDuration);
        }

        // Fade out the static overlay after the effect
        Color finalOverlayColor = staticOverlayImage.color;
        finalOverlayColor.a = 0f;
        staticOverlayImage.color = finalOverlayColor;

        // Wait for the remaining duration of the dialogue line
        yield return new WaitForSeconds(lineDuration);
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
                scanner.ResetScanner();
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
