using OpenCover.Framework.Model;
using UnityEngine;

/// <summary>
/// A simple component for adding comments to GameObjects in the Unity Editor.
/// This has no runtime impact and is purely for organizational purposes.
/// </summary>

/// <summary>
///  A class used for maintaining per-object documentation
/// </summary>
[System.Serializable] // This attribute is needed for Unity to serialize the class
public class Documentation : MonoBehaviour
{
    [System.Serializable] // This makes the class serializable and visible in the Inspector
    public class Comment
    {
        public string subject;
        public string author;

        [TextArea(3, 10)] // This attribute makes the field a multi-line text field
        public string comments;
    }

    public Comment[] comments;
}
