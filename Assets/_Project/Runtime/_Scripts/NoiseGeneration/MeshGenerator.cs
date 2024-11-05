using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{

    public static MeshData GenerateMesh(float[,] heightMap, float heightMultiplier, float gridCellLength)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        float topLeftX = (width - 1) / -2f * gridCellLength;
        float topLeftZ = (height - 1) / 2f * gridCellLength;

        MeshData meshData = new MeshData(width, height);
        int vertexIndex = 0;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float cameraEdgeOffset = 0;
                float fallOffZone = 0.2f;
                if (y >= height * (1 - fallOffZone))
                {
                    float heightEdgeDist = height * fallOffZone;
                    float yDiff = height - y;
                    cameraEdgeOffset = Mathf.Lerp(0.4f, 0, yDiff / heightEdgeDist);
                }

                float VertexX = topLeftX + (x * gridCellLength);
                float VertexY = (heightMap[x, y] - cameraEdgeOffset) * heightMultiplier;
                float VertexZ = topLeftZ - (y * gridCellLength);



                meshData.Vertices[vertexIndex] = new Vector3(VertexX, VertexY, VertexZ);
                meshData.UVs[vertexIndex] = new Vector2(x / (float)width, y / (float)height);

                if (x < width - 1 && y < height - 1)
                {
                    meshData.AddTriangle(vertexIndex, vertexIndex + width + 1, vertexIndex + width);
                    meshData.AddTriangle(vertexIndex + width + 1, vertexIndex, vertexIndex + 1);


                }

                vertexIndex++;
            }
        }

        return meshData;

    }

}


public class MeshData
{
    public Vector3[] Vertices;
    public int[] Triangles;
    public Vector2[] UVs; 

    int triangleIndex;

    public MeshData(int meshWidth, int meshHeight)
    {
        Vertices = new Vector3[meshWidth * meshHeight];
        UVs = new Vector2[meshWidth * meshHeight];
        Triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
    }

    public void AddTriangle(int a, int b, int c)
    {
        Triangles[triangleIndex] = a;
        Triangles[triangleIndex+1] = b;
        Triangles[triangleIndex+2] = c;
        triangleIndex += 3;
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = Vertices;
        mesh.triangles = Triangles;
        mesh.uv = UVs;
        mesh.RecalculateNormals();
        return mesh;
    }

}