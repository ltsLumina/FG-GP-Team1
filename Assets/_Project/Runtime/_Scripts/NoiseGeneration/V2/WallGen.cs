using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGen : MonoBehaviour
{
    [SerializeField] public MapDisplayV2 Display;
    [SerializeField] Camera MainCam;

    [SerializeField] MeshFilter plane1;
    [SerializeField] MeshFilter plane2;
    [SerializeField] MeshFilter plane3;

    [SerializeField] public int Width;
    [SerializeField] public int Height;

    [SerializeField] public float ScaleX;
    [SerializeField] public float ScaleY;
    [SerializeField] public int Octaves;
    [Range(0, 1)]
    [SerializeField] public float Persistance;
    [SerializeField] public float Lacunarity;
    [SerializeField] public Vector2 Offset;

    [SerializeField] public Vector2 GlobalOffset;
    [SerializeField] public float GlobalScale;
    [SerializeField] public float GlobalAmplitude;

    [SerializeField] public AnimationCurve Curve;
    [SerializeField] public float HeightMultiplier;
    [SerializeField] public float CellSize;


    private List<GridDS> _grids = new List<GridDS>();

    private List<Mesh> _meshs = new List<Mesh>();

    GridDS grid1;
    Mesh mesh1 = null;

    GridDS grid2;
    Mesh mesh2 = null;

    GridDS grid3;
    Mesh mesh3 = null;

    private RowData _lastRow;

    private GridDS _currentWall;
    private int _currentWallIndex;

    private bool _inSecondHalf = true;
    private bool _inSecondHalfTrailing = true;
    private bool _updateWalls = false;

    private int _levelCount = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        grid1 = new GridDS(Curve, transform.position, Width, Height, CellSize, HeightMultiplier, ScaleX, ScaleY, Offset, GlobalOffset, GlobalScale, GlobalAmplitude, Octaves, Persistance, Lacunarity, 0);
        grid2 = new GridDS(Curve, transform.position, Width, Height, CellSize, HeightMultiplier, ScaleX, ScaleY, Offset, GlobalOffset, GlobalScale, GlobalAmplitude, Octaves, Persistance, Lacunarity, 0);
        grid3 = new GridDS(Curve, transform.position, Width, Height, CellSize, HeightMultiplier, ScaleX, ScaleY, Offset, GlobalOffset, GlobalScale, GlobalAmplitude, Octaves, Persistance, Lacunarity, 0);

        _grids.Add(grid1);
        _grids.Add(grid2);
        _grids.Add(grid3);

        _meshs.Add(mesh1);
        _meshs.Add(mesh2);
        _meshs.Add(mesh3);


        grid1.SetupWall();
        mesh1 = grid1.GenerateMesh(mesh1);
        plane1.sharedMesh = mesh1;
        _lastRow = grid1.GetLastRow();
        Offset.y += (Height);
        GlobalOffset.y += (Height);
        _levelCount++;

        Vector3 newPosition = new Vector3(transform.position.x, transform.position.y - ((Height - 2) * CellSize * _levelCount), transform.position.z);
        StartCoroutine(newWall(1, newPosition));
    }

    // Update is called once per frame
    void Update()
    {
        CheckMeshInlineWithPlayer();

        if (mesh1 != null)
        {
            plane1.sharedMesh = mesh1;
            plane1.transform.position = grid1.GetPosition();
        }
        if (mesh2 != null)
        {
            plane2.sharedMesh = mesh2;
            plane2.transform.position = grid2.GetPosition();
        }
        if (mesh3 != null)
        {
            plane3.sharedMesh = mesh3;
            plane3.transform.position = grid3.GetPosition();
        }


        CamBelowHalfOfCurrentWall();

        if (_updateWalls)
        {
            Debug.Log((_currentWallIndex + 1) % 3);

            Vector3 newPosition = new Vector3(transform.position.x, transform.position.y - ((Height - 2) * CellSize * _levelCount), transform.position.z);
            StartCoroutine(newWall((_currentWallIndex + 1) % 3, newPosition));
            _levelCount++;
        }


    }

    private void CheckMeshInlineWithPlayer()
    {
        int index = 0;

        foreach (GridDS grid in _grids)
        {
            float upperBound = grid.GetPosition().y + (Height * CellSize / 2f);
            float lowerBound = grid.GetPosition().y - (Height * CellSize / 2f);
            //Debug.Log(upperBound + " " + lowerBound);

            if (MainCam.transform.position.y < upperBound && MainCam.transform.position.y > lowerBound)
            {
                _currentWall = grid;
                _currentWallIndex = index;
                break;

            }
            index++;
        }
    }

    private void CamBelowHalfOfCurrentWall()
    {
        if (MainCam.transform.position.y < _currentWall.GetPosition().y)
        {
            _inSecondHalf = true;
            if (_inSecondHalf != _inSecondHalfTrailing)
            {
                _updateWalls = true;
                _inSecondHalfTrailing = _inSecondHalf;
                return;
            }
        }
        else
        {
            _inSecondHalf = false;
        }
        _inSecondHalfTrailing = _inSecondHalf;
        _updateWalls = false;
    }

    IEnumerator newWall(int index, Vector3 position)
    {
        int numberOfFrameToCalculateOver = 60;
        GridDS newGrid = _grids[index];

        /*
        switch (index)
        {
            case 0:
                newGrid = grid1;
                break;
            case 1:
                newGrid = grid2;
                break;
            case 2:
                newGrid = grid3;
                break;     
        }
        */

        newGrid.Clear();
        newGrid.SetOffset(Offset, GlobalOffset);
        newGrid.SetPosition(position);

        int numRowsPerFrame = (int)Mathf.Ceil(Height / (float)numberOfFrameToCalculateOver);
        int totalRowsCalculated = 0;
        for (int i = 0; i < numberOfFrameToCalculateOver; i++)
        {
            if (totalRowsCalculated < Height - 1)
            {
                for (int j = 0; j < numRowsPerFrame; j++)
                {
                    newGrid.GenerateNextRow();
                    totalRowsCalculated++;
                }
            }
            if (i == numberOfFrameToCalculateOver - 1)
            {
                newGrid.CullExtraRows();
                newGrid.SetFirstRow(_lastRow);

                //_meshs[index] = newGrid.GenerateMesh(_meshs[index]);
                //Debug.Log(_meshs[index]);
                
                switch (index)
                {
                    case 0:
                        mesh1 = newGrid.GenerateMesh(mesh1);
                        break;
                    case 1:
                        mesh2 = newGrid.GenerateMesh(mesh2);
                        break;
                    case 2:
                        mesh3 = newGrid.GenerateMesh(mesh3);
                        break;
                }
                

                _lastRow = newGrid.GetLastRow();
                Offset.y += (Height);
                GlobalOffset.y += (Height);
            }
            yield return null;
        }
    }

}
