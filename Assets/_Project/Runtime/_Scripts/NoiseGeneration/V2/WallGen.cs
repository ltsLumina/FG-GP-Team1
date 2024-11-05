using DG.Tweening;
using System.Collections;
using UnityEngine;

public class WallGen : MonoBehaviour
{
    [SerializeField] public MapDisplayV2 Display;
    [SerializeField] Camera MainCam;

    [SerializeField] MeshFilter plane1;
    [SerializeField] MeshFilter plane2;

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

    GridDS grid1;
    Mesh mesh1;

    GridDS grid2;
    Mesh mesh2;

    private RowData _lastRow;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        grid1 = new GridDS(Curve, transform.position, Width, Height, CellSize, HeightMultiplier, ScaleX, ScaleY, Offset, GlobalOffset, GlobalScale, GlobalAmplitude, Octaves, Persistance, Lacunarity, 0);
        grid2 = new GridDS(Curve, transform.position, Width, Height, CellSize, HeightMultiplier, ScaleX, ScaleY, Offset, GlobalOffset, GlobalScale, GlobalAmplitude, Octaves, Persistance, Lacunarity, 0);

        grid1.SetupWall();
        mesh1 = grid1.GenerateMesh(mesh1, transform.position);
        plane1.sharedMesh = mesh1;
        _lastRow = grid1.GetLastRow();
        Offset.y += (Height);
        GlobalOffset.y += (Height);
        StartCoroutine(newWall(false, transform.position));
    }

    // Update is called once per frame
    void Update()
    {
        if (mesh2 != null)
        {
            plane2.sharedMesh = mesh2;
            Vector3 newPosition = new Vector3(transform.position.x, transform.position.y - ((Height-2) * CellSize), transform.position.z);
            plane2.gameObject.transform.position = newPosition;
        }
    }

    IEnumerator newWall(bool bufferFlag, Vector3 position)
    {
        int numberOfFrameToCalculateOver = 60;
        GridDS newGrid;
        if (bufferFlag)
        {
            newGrid = grid1;
            newGrid.Clear();
        }
        else
        {
            newGrid = grid2;
            newGrid.Clear();
        }
        newGrid.SetOffset(Offset, GlobalOffset);

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
                
                if (bufferFlag)
                {
                    mesh1 = newGrid.GenerateMesh(mesh1, position);
                }
                else
                {
                    mesh2 = newGrid.GenerateMesh(mesh2, position);
                }

                _lastRow = newGrid.GetLastRow();
                Offset.y += (Height);
                GlobalOffset.y += (Height);
            }
            yield return null;
        }
    }

}
