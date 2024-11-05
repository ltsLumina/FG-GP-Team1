using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CoralPlacer : MonoBehaviour
{

    [SerializeField] public MeshFilter WallMesh;
    [SerializeField] GameObject wall;

    [SerializeField] GameObject CoralPrefabGreen;
    [SerializeField] GameObject CoralPrefabRed;

    Mesh mesh;
    Vector3[] meshVertices;
    Vector3[] meshNormals;
    int[] meshTriangles;

    [Range(0,1)]
    [SerializeField] float lerpRatio;

    List<GameObject> cubes = new List<GameObject>();


    // Start is called before the first frame update
    void Start()
    {
        transform.position = wall.transform.position;
        mesh = WallMesh.mesh;
        meshVertices = mesh.vertices;
        meshNormals = mesh.normals;
        meshTriangles = mesh.triangles;

        Debug.Log("Number of normals in Mesh: " + meshNormals.Length);
        Debug.Log("Number of vertices in Mesh: " + meshVertices.Length);
        Debug.Log("Number of triangles in Mesh: " + meshTriangles.Length);


        for (int i = 0; i < meshTriangles.Length; i += 3)
        {
            Vector3 vert1pos = RotateAround(meshVertices[meshTriangles[i]], wall.transform.position, wall.transform.rotation);
            Vector3 vert2pos = RotateAround(meshVertices[meshTriangles[i+1]], wall.transform.position, wall.transform.rotation);
            Vector3 vert3pos = RotateAround(meshVertices[meshTriangles[i+2]], wall.transform.position, wall.transform.rotation);

            Vector3 finalPos = (vert1pos + vert2pos + vert3pos) / 3;

            Vector3 A = vert2pos - vert1pos;
            Vector3 B = vert3pos - vert1pos;

            Vector3 triNorm = Vector3.Cross(A, B).normalized;

            float normXAngle = Mathf.Acos(triNorm.x);
            GameObject newCube = null;
            if (triNorm.y > 0)
            {
                newCube = Instantiate(CoralPrefabGreen, finalPos, Quaternion.FromToRotation(Vector3.forward, triNorm));
            }
    

            cubes.Add(newCube);
        }

    }

    static Vector3 RotateAround(Vector3 position, Vector3 pivotPoint, Quaternion rot)
    {
        Vector3 finalVec = new Vector3();
        finalVec = position;
        finalVec = rot * position + pivotPoint;
        return finalVec;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = wall.transform.position;

    }

}
