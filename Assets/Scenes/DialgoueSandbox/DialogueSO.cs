using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Scriptable Objects/DialogueSO")]
public class Dialogue : ScriptableObject
{
    [Tooltip("Array of all dialogue lines for this dialogue sequence")]
    public DialogueLine[] dialogueLines;
}

[System.Serializable]
public class DialogueLine
{
    [Tooltip("Name of the speaker for this line of dialogue")]
    public string speakerName; // Name of the speaker

    [Tooltip("Profile picture of the speaker (optional)")]
    public Sprite speakerProfilePicture; // Profile picture of the speaker (optional)

    [Tooltip("The text to display for this line of dialogue")]
    [TextArea(3, 10)]
    public string dialogueText; // The text for the dialogue line

    [Tooltip("Voice-over audio clip for this line (optional)")]
    public AudioClip voiceOverClip; // VO line file (AudioClip)

    [Tooltip("Time in seconds to display this line")]
    public float timing; // Time in seconds to display this line
}
