using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] public MapDisplay display;

    public enum DrawMode
    {
        NoiseMap, ColourMap, Mesh
    }

    [SerializeField] public DrawMode drawMode;

    [SerializeField] public int mapWidth;
    [SerializeField] public int mapHeight;
    [SerializeField] public float scaleX;
    [SerializeField] public float scaleY;

    [SerializeField] public int octaves;
    [Range(0,1)]
    [SerializeField] public float persistance;
    [SerializeField] public float lacunarity;

    [SerializeField] public int seed;
    [SerializeField] public Vector2 offset;

    [SerializeField] public float heightMultiplier;
    [SerializeField] public float cellSize;
    [SerializeField] public AnimationCurve Curve;

    [SerializeField] public bool autoUpdate;

    [SerializeField] public TerrainType[] regions;


    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, scaleX, scaleY, octaves, persistance, lacunarity, offset);


        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                noiseMap[x,y] = Curve.Evaluate(noiseMap[x, y]);
            }
        }


        Color[] colourArray = new Color[mapWidth * mapHeight];
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions[i].height)
                    {
                        colourArray[y * mapWidth + x] = regions[i].colour;
                        break;
                    }
                }
            }
        }

        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
        }
        else if (drawMode == DrawMode.ColourMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColourMap(colourArray, mapWidth, mapHeight));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGenerator.GenerateMesh(noiseMap, heightMultiplier, cellSize), TextureGenerator.TextureFromColourMap(colourArray, mapWidth, mapHeight));
        }


    }

    private void OnValidate()
    {
        if (mapWidth < 1) mapWidth = 1;
        if (mapHeight < 1) mapHeight = 1;

        if (lacunarity < 1) lacunarity = 1;

        if (octaves < 1) octaves = 1;

    }

}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color colour;
}