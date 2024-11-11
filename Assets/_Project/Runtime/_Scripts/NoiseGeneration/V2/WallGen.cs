using System.Collections;
using System.Collections.Generic;
using Lumina.Essentials.Modules;
using UnityEditor;
using UnityEngine;

public class WallGen : MonoBehaviour
{
    [SerializeField] public MapDisplayV2 Display;

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

    [SerializeField] public float HeightMultiplier;
    [SerializeField] public float CellSize;


    private List<GridDS> _grids = new List<GridDS>();

    private List<Mesh> _meshes = new List<Mesh>();

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

    //temp
    private Vector3[] _triNormals;
    private Vector3[] _triCentres;
    private List<int> _corals;


    [SerializeField] public List<GameObject> CoralPrefabs = new List<GameObject>();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (GameObject coralPrefab in CoralPrefabs)
        {
            ObjectPoolStaticBatch.Instance.CreateNewPool(coralPrefab.name, coralPrefab);
        }

        grid1 = new GridDS(transform.position, Width, Height, CellSize, HeightMultiplier, ScaleX, ScaleY, Offset, GlobalOffset, GlobalScale, GlobalAmplitude, Octaves, Persistance, Lacunarity, 0);
        grid2 = new GridDS(transform.position, Width, Height, CellSize, HeightMultiplier, ScaleX, ScaleY, Offset, GlobalOffset, GlobalScale, GlobalAmplitude, Octaves, Persistance, Lacunarity, 0);
        grid3 = new GridDS(transform.position, Width, Height, CellSize, HeightMultiplier, ScaleX, ScaleY, Offset, GlobalOffset, GlobalScale, GlobalAmplitude, Octaves, Persistance, Lacunarity, 0);

        _grids.Add(grid1);
        _grids.Add(grid2);
        _grids.Add(grid3);

        _meshes.Add(mesh1);
        _meshes.Add(mesh2);
        _meshes.Add(mesh3);

        grid1.SetupWall();
        mesh1 = grid1.GenerateMesh(mesh1);
        plane1.sharedMesh = mesh1;
        _lastRow = grid1.GetLastRow();
        Offset.y += (Height);
        GlobalOffset.y += (Height);
        _levelCount++;

        Vector3 newPosition = new Vector3(transform.position.x, transform.position.y - ((Height - 2) * CellSize * _levelCount), transform.position.z);
        StartCoroutine(newWall(1, newPosition, transform.rotation));
        _levelCount++;
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
            Vector3 newPosition = new Vector3(transform.position.x, transform.position.y - ((Height - 2) * CellSize * _levelCount), transform.position.z);
            StartCoroutine(newWall((_currentWallIndex + 1) % 3, newPosition, transform.rotation));
            _levelCount++;

            for (int i = 0; i < _corals.Count; i++)
            {
                Vector3 centre = _triCentres[_corals[i]];
                Vector3 normal = _triNormals[_corals[i]];

                int coralPrefabIndex = Random.Range(0, CoralPrefabs.Count);
                GameObject coral = CoralPrefabs[coralPrefabIndex];
                GameObject temp = Instantiate(coral, centre, Quaternion.Euler(normal));
                temp.transform.localScale = new Vector3(3, 3, 3);
            }

        }

        if (_currentWall != null)
        {
            _triCentres = _currentWall.GetTriCentres();
            _triNormals = _currentWall.GetTriNormals();
            _corals = _currentWall.GetCoralIndices();
        }

        

        // TODO: Optimize/Refactor this. For whatever reason a for loop doesn't work.
        var meshColliders = GetComponentsInChildren<MeshCollider>();
        meshColliders[0].sharedMesh = mesh1;
        meshColliders[1].sharedMesh = mesh2;
        meshColliders[2].sharedMesh = mesh3;
    }

    public void OnDrawGizmos()
    {

        for (int i = 0; i < _corals.Count; i++)
        {
            Vector3 centre = _triCentres[_corals[i]];
            Vector3 normal = _triNormals[_corals[i]];
            Gizmos.color = Color.yellow;
            //Gizmos.DrawCube(centre, new Vector3(1,1,1));
            //Gizmos.DrawRay(centre, normal);
        }
    }

    static Vector3 RotateAround(Vector3 position, Vector3 pivotPoint, Quaternion rot)
    {
        Vector3 finalVec = new Vector3();
        finalVec = position;
        finalVec = rot * position + pivotPoint;
        return finalVec;
    }

    private void CheckMeshInlineWithPlayer()
    {
        int index = 0;

        foreach (GridDS grid in _grids)
        {
            float upperBound = grid.GetPosition().y + (Height * CellSize / 2f);
            float lowerBound = grid.GetPosition().y - (Height * CellSize / 2f);
            //Debug.Log(upperBound + " " + lowerBound);

            if (Helpers.CameraMain.transform.position.y < upperBound && Helpers.CameraMain.transform.position.y > lowerBound)
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
        if (Helpers.CameraMain.transform.position.y < _currentWall.GetPosition().y)
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

    IEnumerator newWall(int index, Vector3 position, Quaternion rotation)
    {
        int numberOfFrameToCalculateOver = 60;
        GridDS newGrid = _grids[index];

        newGrid.Clear();
        newGrid.SetOffset(Offset, GlobalOffset);
        newGrid.SetPosition(position);
        newGrid.SetRotation(rotation);

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
