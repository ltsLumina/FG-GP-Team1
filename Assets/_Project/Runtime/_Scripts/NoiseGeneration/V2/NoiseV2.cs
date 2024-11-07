using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BetterNoise
{

    public static float[,] GetNoiseMap(int width, int startHeight, int endHeight, float scaleX, float scaleY, Vector2 offset, Vector2 globalOffset, float globalScale, float globalAmplitude, int octaves, float persistance, float lacunarity, int seed)
    {
        int mapHeight = endHeight - startHeight;
        float[,] noiseMap = new float[width, mapHeight];

        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            float xOffset = offset.x; //prng.Next(-1000, 1000) + globalOffset.x;
            float yOffset = offset.y; //prng.Next(-1000, 1000) + globalOffset.y;
            octaveOffsets[i] = new Vector2(xOffset, yOffset);
        }

        if (scaleX == 0)
        {
            scaleX = 0.001f;
        }
        if (scaleY == 0)
        {
            scaleY = 0.001f;
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        for (int x = 0; x < width; x++)
        {
            for (int y = startHeight; y < endHeight; y++)
            {

                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                noiseHeight += (Mathf.PerlinNoise(0, (y + globalOffset.y) / globalScale) * 2 - 1) * globalAmplitude;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = x / scaleX * frequency + (octaveOffsets[i].x / scaleX);
                    float sampleY = y / scaleY * frequency + (octaveOffsets[i].y / scaleY);

                    float noiseVal = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;

                    float perlinValue = Mathf.Pow(noiseVal, 3) + 0.1f * noiseVal;

                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }

                noiseMap[x, y - startHeight] = noiseHeight;
            }
        }
        
        return noiseMap;
    }


}
