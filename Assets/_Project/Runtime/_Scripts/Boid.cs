using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Boid : MonoBehaviour
{

    [SerializeField, Range(0.0f, 40f)] private float moveSpeed;
    [SerializeField] private float turnSpeed;
    [SerializeField] private float boidContext;
    [SerializeField] private float wallContext;
    [SerializeField] LayerMask groundLayer;

    private Vector3 targetDir;

    [SerializeField] private List<Boid> boids = new List<Boid>();


    private void Start()
    {
        transform.right = new Vector3(Random.Range(-1f,1f), 0, Random.Range(-1f, 1f)).normalized;
        GetComponent<SphereCollider>().radius = boidContext;
    }

    private void Update()
    {
        Vector3 sumForces = Vector3.zero;

        sumForces += CheckWall(transform.right);
        sumForces += Seperation();
        sumForces += Alignment();
        sumForces += Cohesion();

        targetDir = sumForces.normalized;

        transform.right = Vector3.MoveTowards(transform.right, targetDir, turnSpeed * Time.deltaTime);

        Debug.Log(transform.right);

        transform.position += transform.right * moveSpeed * Time.deltaTime;

    }


    private Vector3 CheckWall(Vector3 dir)
    {

        RaycastHit hit;

        if (Physics.Raycast(transform.position, dir, out hit, wallContext, groundLayer))

        {
            Debug.DrawRay(transform.position, dir * hit.distance, Color.red);
       //     CheckWall(new Vector3(dir.x-0.1f, dir.y , dir.z+0.1f));
        }
        else
        {
            Debug.DrawRay(transform.position, dir * wallContext, Color.green);
            transform.right = dir;
            return dir;
        }
        return Vector3.zero;
    }

    private Vector3 Seperation()
    {
        Vector3 seperationVector = Vector3.zero;

        for (int i = 0; i < boids.Count; i++)
        {
            Vector3 tempVector = Vector3.zero;
            tempVector += transform.position - boids[i].transform.position;
            tempVector /= Vector3.Distance(transform.position, boids[i].transform.position);
            seperationVector += tempVector;
        }

        return seperationVector.normalized;

    }

    private Vector3 Alignment()
    {
        Vector3 alignmentVector = Vector3.zero;

        for(int i = 0; i < boids.Count; i++)
        {
            alignmentVector += boids[i].TargetDir;
        }

        alignmentVector /= boids.Count;
        Debug.Log(alignmentVector);
        return alignmentVector.normalized;
    }

    private Vector3 Cohesion()
    {
        Vector3 cohesionVector = Vector3.zero;

        for (int i = 0; i < boids.Count; i++)
        {
            cohesionVector += boids[i].transform.position;
        }

        cohesionVector /= boids.Count;

        return (cohesionVector - transform.position).normalized;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boid"))
        {
            boids.Add(other.GetComponent<Boid>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Boid"))
        {
            boids.Remove(other.GetComponent<Boid>());
        }
    }

    public Vector3 TargetDir
    {
        get => targetDir;
    }

}
