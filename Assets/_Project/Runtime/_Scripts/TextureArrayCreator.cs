using UnityEditor;
using UnityEngine;

public class TextureArrayCreator : MonoBehaviour
{
    [MenuItem("Assets/Create/Texture Array")]
    static void CreateTextureArray()
    {
        // Prompt user to select multiple textures
        Texture2D[] textures = Selection.GetFiltered<Texture2D>(SelectionMode.Assets);

        if (textures.Length == 0)
        {
            Debug.LogError(
                "No textures selected! Please select some textures to create a texture array."
            );
            return;
        }

        int width = textures[0].width;
        int height = textures[0].height;
        TextureFormat format = textures[0].format;

        // Create the texture array with the size of the first texture
        Texture2DArray textureArray = new Texture2DArray(
            width,
            height,
            textures.Length,
            format,
            false
        );

        // Copy each texture into the array
        for (int i = 0; i < textures.Length; i++)
        {
            if (textures[i].width != width || textures[i].height != height)
            {
                Debug.LogError("All textures must have the same dimensions and format!");
                return;
            }

            Graphics.CopyTexture(textures[i], 0, 0, textureArray, i, 0);
        }

        // Save the texture array asset
        string path = "Assets/TextureArray.asset";
        AssetDatabase.CreateAsset(textureArray, path);
        Debug.Log("Texture Array created at " + path);
    }
}
