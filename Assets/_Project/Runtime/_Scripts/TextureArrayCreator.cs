using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class TextureArrayCreator : MonoBehaviour
{
#if UNITY_EDITOR
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

        // Sort the textures numerically by the number at the front of their name
        textures = textures.OrderBy(texture => GetLeadingNumber(texture.name)).ToArray();

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

    // Extract leading number from the texture name
    static int GetLeadingNumber(string name)
    {
        // Regex to match a number at the beginning of the string
        Match match = Regex.Match(name, @"^\d+");
        return match.Success ? int.Parse(match.Value) : int.MaxValue; // If no leading number, push to the end
    }
#endif
}
