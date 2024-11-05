using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    [SerializeField] public MapDisplayV2 Display;
    [SerializeField] Camera MainCam;

    private Transform _cameraTranform;
    private float _initialCameraPos;

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

    private GridDS _grid;
    private int _numRowsAdded;

    [SerializeField] public float WallSpeed;



    // Start is called before the first frame update
    void Start()
    {
        _cameraTranform = MainCam.transform;
        _initialCameraPos = _cameraTranform.position.y;

        _numRowsAdded = 0;

        _grid = new GridDS(transform.position, Width, Height, CellSize, HeightMultiplier, ScaleX, ScaleY, Offset, GlobalOffset, GlobalScale, GlobalAmplitude, Octaves, Persistance, Lacunarity, 0);


        _grid.SetupWall();


        Display.DrawMesh(_grid.GenerateMesh(Display.mesh, transform.position));
    }

    /*
    private void Update()
    {
        Vector3 newPosition = new Vector3(transform.position.x, transform.position.y - WallSpeed * Time.deltaTime, transform.position.z);
        transform.position = newPosition;
    }
    */

    // Update is called once per frame
    void Update()
    {
        Vector3 newPosition = new Vector3(transform.position.x, transform.position.y - WallSpeed * Time.deltaTime, transform.position.z);
        transform.position = newPosition;

        float cameraDiff = Mathf.Abs(_cameraTranform.position.y - _initialCameraPos);
        int numberRowsRequired = (int)Mathf.Floor(cameraDiff / CellSize);
        while (numberRowsRequired > _numRowsAdded)
        {
            _grid.GenerateNextRow();
            _numRowsAdded++;
        }


        Display.DrawMesh(_grid.GenerateMesh(Display.mesh, transform.position));
    }
}
