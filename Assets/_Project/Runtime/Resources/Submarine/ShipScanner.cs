using UnityEngine;

public class ShipScanner : MonoBehaviour
{
    public void HighlightObject(GameObject obj)
    {
        if (obj == null)
        {
            Debug.LogWarning("Object to highlight is null.");
            //Turn off Scanner
            return;
        }
        // Turn on Scanner
        // Highlight the object
        Debug.Log("Highlighting object: " + obj.name);
    }
}
