using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Searcher.SearcherWindow.Alignment;
using static UnityEngine.Mesh;
using static UnityEngine.Rendering.DebugUI.Table;

public class GridDS
{
    //Grid Integrity
    LinkedList<RowData> _meshData = new LinkedList<RowData>();
    private int _totalRows;

    //Noise Related Variables
    private int _width;
    private int _height;
    private float _scaleX;
    private float _scaleY;
    private float _globalScale;
    private float _globalAmplitude;
    private int _octaves;
    private float _persistance;
    private float _lacunarity;
    private int _seed;

    private Vector2 _offset;
    private Vector2 _globalOffset;

    private float _gridCellLength;

    private Vector3 _position;
    private float _heightMultiplier;

    public GridDS(Vector3 position, int width, int height, float gridCellLength, float heightMultiplier, float scaleX, float scaleY, Vector2 offset, Vector2 globalOffset, float globalScale, float globalAmplitude, int octaves, float persistance, float lacunarity, int seed)
    {
        _position = position;
        _heightMultiplier = heightMultiplier;

        _width = width;
        _height = height;
        _scaleX = scaleX;
        _scaleY = scaleY;
        _globalScale = globalScale;
        _globalAmplitude = globalAmplitude;
        _octaves = octaves;
        _persistance = persistance;
        _lacunarity = lacunarity;
        _seed = seed;
        
        _offset = offset;
        _globalOffset = globalOffset;

        _gridCellLength = gridCellLength;
    }

    public void Clear()
    {
        _meshData.Clear();
    }

    public void SetupWall()
    {
        _offset.y = -(_height * _gridCellLength / 2);
        for (int i = 0; i < _height - 1; i++)
        {
            GenerateNextRow();
        }
    }

    public void GenerateNextRow()
    {
        float[,] heightMap = BetterNoise.GetNoiseMap(_width, _totalRows, _totalRows+2, _scaleX, _scaleY, new Vector2(0,0), _globalOffset, _globalScale, _globalAmplitude, _octaves, _persistance, _lacunarity, _seed);
        
        float topLeftX = _position.x - (_width * _gridCellLength / 2);
        float topLeftZ = _offset.y;

        int vertexIndex = 0;

        RowData newRow = new RowData(_width, 2);
        newRow.rowNumber = _totalRows;

        newRow.Triangles = new int[(_width - 1) * 6];

        for (int y = 0; y < 2; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                float VertexX = topLeftX + (x * _gridCellLength);
                float VertexY = heightMap[x, y] * _heightMultiplier;
                float VertexZ = topLeftZ - (y * _gridCellLength);

                newRow.Vertices[vertexIndex] = new Vector3(VertexX, VertexY, VertexZ);
                newRow.UVs[vertexIndex] = new Vector2(x / (float)_width, y / 2.0f);

                if (y < 1 && x < _width - 1)
                {
                    newRow.AddTriangle(vertexIndex, vertexIndex + _width + 1, vertexIndex + _width);
                    newRow.AddTriangle(vertexIndex + _width + 1, vertexIndex, vertexIndex + 1);
                }

                vertexIndex++;
            }
        }

        _meshData.AddLast(newRow);


        _totalRows++;
        _offset.y += _gridCellLength;
    }

    public Mesh GenerateMesh(Mesh mesh, Vector3 position)
    {
        if (mesh == null)
        {
            mesh = new Mesh();
        }
        //Mesh outmesh = new Mesh();

        Vector3[] vertices = new Vector3[_width * _height];
        Vector2[] UVs = new Vector2[_width * _height];
        int[] triangles = new int[(_width - 1) * (_height - 1) * 6];

        float zPos = position.z - _height * _gridCellLength / 2;

        int vertexIndex = 0;
        int triangleIndex = 0;

        if (_meshData.Count >= _height - 1)
        {
            while (_meshData.Count >= _height - 1)
            {
                _meshData.RemoveFirst();
            }
        }
        RowData last = _meshData.Last.Value;

        foreach (RowData row in _meshData)
        {
            if (row.rowNumber < last.rowNumber - 1)
            {
                int rowLength = row.Vertices.Length;
                int triangleOffset = vertexIndex;
                for (int i = 0; i < rowLength / 2; i++)
                {
                    Vector3 rowVerts = row.Vertices[i];
                    rowVerts.z = zPos;
                    vertices[vertexIndex] = rowVerts;
                    UVs[vertexIndex] = row.UVs[i];
                    vertexIndex++;
                }
                for (int i = 0; i < row.Triangles.Length; i++)
                {
                    triangles[triangleIndex] = row.Triangles[i] + triangleOffset;
                    triangleIndex++;
                }
            }
            else
            {
                int rowLength = row.Vertices.Length;
                int triangleOffset = vertexIndex;
                for (int i = 0; i < rowLength; i++)
                {
                    Vector3 rowVerts = row.Vertices[i];
                    rowVerts.z = zPos;
                    vertices[vertexIndex] = rowVerts;
                    UVs[vertexIndex] = row.UVs[i];
                    vertexIndex++;
                }
                for (int i = 0; i < row.Triangles.Length; i++)
                {
                    triangles[triangleIndex] = row.Triangles[i] + triangleOffset;
                    triangleIndex++;
                }
            }
            zPos += _gridCellLength;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = UVs;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }

}

public class RowData
{
    public Vector3[] Vertices;
    public int[] Triangles;
    public Vector2[] UVs;

    public int rowNumber;
    int triangleIndex = 0;

    public RowData(int meshWidth, int meshHeight)
    {
        Vertices = new Vector3[meshWidth * meshHeight];
        UVs = new Vector2[meshWidth * meshHeight];
        Triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
    }

    public void AddTriangle(int a, int b, int c)
    {
        Triangles[triangleIndex] = a;
        Triangles[triangleIndex + 1] = b;
        Triangles[triangleIndex + 2] = c;
        triangleIndex += 3;
    }
}