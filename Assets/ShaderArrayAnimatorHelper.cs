using UnityEngine;

public class ShaderArrayAnimatorHelper : MonoBehaviour
{
    [SerializeField]
    private Material instanceMaterial;

    // Attach this to the Animator-controlled variable
    [Range(0, 30)]
    public float animatedArrayIndex = 0;

    void Update()
    {
        // Set the property on the material instance each frame
        instanceMaterial?.SetFloat("_ArrayIndex", animatedArrayIndex);
    }
}
