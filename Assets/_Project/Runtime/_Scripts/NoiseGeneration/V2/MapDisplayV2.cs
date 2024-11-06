using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplayV2 : MonoBehaviour
{
    [SerializeField] Renderer textureRenderer;
    [SerializeField] MeshFilter meshFilter;
    [SerializeField] MeshRenderer meshRenderer;

    public void DrawTexture(Texture2D texture)
    {
        textureRenderer.sharedMaterial.mainTexture = texture;
        textureRenderer.transform.localScale = new Vector3(texture.width, 1, texture.height);
    }

    public void DrawMesh(Mesh meshData)
    {
        meshFilter.sharedMesh = meshData;
    }

    public Mesh mesh => meshFilter.sharedMesh;

}
