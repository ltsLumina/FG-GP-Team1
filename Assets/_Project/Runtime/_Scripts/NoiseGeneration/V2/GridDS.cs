using System.Collections.Generic;
using UnityEngine;

public class GridDS
{
    //Grid Integrity
    LinkedList<RowData> _meshData = new LinkedList<RowData>();
    private Vector3[] _triNormals;
    private Vector3[] _triCentres;

    private List<int> _coralIndex = new List<int>();

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
    private Quaternion _rotation;
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
        _offset.y = 0;//-(_height * _gridCellLength / 2);
        for (int i = 0; i < _height - 1; i++)
        {
            GenerateNextRow();
        }
    }

    public void SetOffset(Vector2 offset, Vector2 globalOffset)
    {
        _offset = offset;
        _globalOffset = globalOffset;
    }

    public RowData GetLastRow()
    {
        return _meshData.Last.Value;
    }

    public void SetFirstRow(RowData row)
    {
        _meshData.RemoveFirst();
        _meshData.AddFirst(row);
    }

    public int GetRowCount()
    {
        return _meshData.Count;
    }

    public void GenerateNextRow()
    {
        float[,] heightMap = BetterNoise.GetNoiseMap(_width, _totalRows, _totalRows+2, _scaleX, _scaleY, _offset, _globalOffset, _globalScale, _globalAmplitude, _octaves, _persistance, _lacunarity, _seed);
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
                float heightMapVal = heightMap[x, y];
                float vertexVal = heightMapVal;

                float VertexX = topLeftX + (x * _gridCellLength);
                float VertexY = vertexVal * _heightMultiplier;
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

    public void CullExtraRows()
    {
        if (_meshData.Count >= _height)
        {
            while (_meshData.Count >= _height)
            {
                _meshData.RemoveFirst();
            }
        }
    }

    public Vector3 GetPosition()
    {
        return _position;
    }

    public void SetPosition(Vector3 position)
    {
        _position = position;
    }

    public void SetRotation(Quaternion rotation)
    {
        _rotation = rotation;
    }

    public Mesh GenerateMesh(Mesh mesh)
    {
        if (mesh == null)
        {
            mesh = new Mesh();
        }

        Vector3[] vertices = new Vector3[_width * _height];
        Vector2[] UVs = new Vector2[_width * _height];
        int[] triangles = new int[(_width - 1) * (_height - 1) * 6];

        _triNormals = new Vector3[(_width - 1) * (_height - 1) * 2];
        _triCentres = new Vector3[(_width - 1) * (_height - 1) * 2];


        float zPos = -(_height * _gridCellLength / 2);

        int vertexIndex = 0;
        int triangleIndex = 0;

        if (_meshData.Count >= _height)
        {
            while (_meshData.Count >= _height)
            {
                Debug.Log("Row Removed");
                _meshData.RemoveFirst();
            }
        }
        RowData last = _meshData.Last.Value;
        int index = 0;
        int lastRowIndex = _meshData.Count - 1;

        foreach (RowData row in _meshData)
        {
            if (index < lastRowIndex)
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
            index++;
        }

        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 vert1pos = RotateAround(vertices[triangles[i]], _position, _rotation);
            Vector3 vert2pos = RotateAround(vertices[triangles[i + 1]], _position, _rotation);
            Vector3 vert3pos = RotateAround(vertices[triangles[i + 2]], _position, _rotation);

            Vector3 triPos = (vert1pos + vert2pos + vert3pos) / 3;

            Vector3 A = vert2pos - vert1pos;
            Vector3 B = vert3pos - vert1pos;

            Vector3 triNorm = Vector3.Cross(A, B).normalized;

            _triNormals[i / 3] = triNorm;
            _triCentres[i / 3] = triPos;

            if (triNorm.y >= 0)
            {
                if (Random.Range(0f, 1f) < 0.5f)
                {
                    _coralIndex.Add(i / 3);
                }
            }


        }

        Debug.Log("Number of triNormals: " + _triNormals.Length);
        Debug.Log("Number of triCentres: " + _triCentres.Length);


        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = UVs;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }

    public Vector3[] GetTriNormals()
    {
        return _triNormals;
    }

    public Vector3[] GetTriCentres()
    {
        return _triCentres;
    }

    public List<int> GetCoralIndices()
    {
        return _coralIndex;
    }

    static Vector3 RotateAround(Vector3 position, Vector3 pivotPoint, Quaternion rot)
    {
        Vector3 finalVec = new Vector3();
        finalVec = position;
        finalVec = rot * position + pivotPoint;
        return finalVec;
    }


}

public class RowData
{
    public Vector3[] Vertices;
    public int[] Triangles;
    public Vector2[] UVs;
    public Vector3[] triNormals;
    public Vector3[] triCentres;

    public int rowNumber;
    int triangleIndex = 0;

    public RowData(int meshWidth, int meshHeight)
    {
        Vertices = new Vector3[meshWidth * meshHeight];
        UVs = new Vector2[meshWidth * meshHeight];
        Triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
        triNormals = new Vector3[(meshWidth - 1) * (meshHeight - 1) * 2];
        triCentres = new Vector3[(meshWidth - 1) * (meshHeight - 1) * 2];
    }

    public void AddTriangle(int a, int b, int c)
    {
        Triangles[triangleIndex] = a;
        Triangles[triangleIndex + 1] = b;
        Triangles[triangleIndex + 2] = c;
        triangleIndex += 3;
    }
}