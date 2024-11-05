using UnityEngine;
using Lumina.Essentials.Attributes;

/// <summary>
/// For Artists.
/// </summary>
[DisallowMultipleComponent, SelectionBase]
public class SelectionBase : MonoBehaviour
{
    #pragma warning disable 0414 
    
	[Tooltip("Add this component to the parent object you want the selection to highlight when selected.")]
    [SerializeField, ReadOnly] string label = "Add this component to the parent object you want the selection to highlight when selected.";
}
