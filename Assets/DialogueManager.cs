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

    [Tooltip("The image component for the speaker's profile picture")]
    public Image profileImage;

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

    private Queue<Dialogue> dialogueQueue = new Queue<Dialogue>();
    private Dialogue currentDialogue;
    private int currentLineIndex;
    private GameObject highlightedObject;
    private ShipScanner scanner => GameManager.Instance.ShipScanner;

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

        // Start the static effect coroutine
        StartCoroutine(ApplyStaticEffect(line.timing));
        currentLineIndex++;
    }

    // Coroutine to wait for the timing duration before displaying the next line
    private IEnumerator DisplayLineForDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
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

#if UNITY_EDITOR
public class NoiseTextureCreator
{
    [MenuItem("Assets/Create/Noise Texture")]
    public static void CreateNoiseTexture()
    {
        int width = 256;
        int height = 256;
        Texture2D noiseTexture = new Texture2D(width, height);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float grayValue = Random.Range(0f, 1f);
                Color color = new Color(grayValue, grayValue, grayValue);
                noiseTexture.SetPixel(x, y, color);
            }
        }

        noiseTexture.Apply();

        // Save texture to the Assets folder
        byte[] bytes = noiseTexture.EncodeToPNG();
        string path = "Assets/NoiseTexture.png";
        System.IO.File.WriteAllBytes(path, bytes);
        AssetDatabase.Refresh();

        Debug.Log("Noise texture created at: " + path);
    }
}
#endif
