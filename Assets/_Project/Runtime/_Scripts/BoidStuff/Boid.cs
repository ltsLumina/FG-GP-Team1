using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Boid : MonoBehaviour
{

    private float moveSpeed;
    private float currentMoveSpeed;
    private float turnSpeed;
    private float currentTurnSpeed;
    private float wallContext;

    private float seperationContext;
    private float seperationStrength;

    private float alignmentContext;
    private float alignmentStrength;

    private float cohesionContext;
    private float cohesionStrength;

    private float fleeingMoveSpeedMult;
    private float fleeContext;
    private float fleeTime;

    private bool fleeing = false;

    private BoidSpawner spawner;

    LayerMask groundLayer = 1 << 3;

    private Vector3 targetDir;

    [SerializeField] private List<Boid> boids = new List<Boid>();

    private float timer;

    private float updateTimer = 0.5f;
    

    private void Start()
    {

        transform.forward = new Vector3(Random.Range(-1f,1f), 0, Random.Range(-1f, 1f)).normalized;

        updateTimer = Random.Range(0, updateTimer);

        float maxContext = 0;

        if(seperationContext > maxContext)
        {
            maxContext = seperationContext;
        }
        if(alignmentContext > maxContext)
        {
            maxContext = alignmentContext;
        }
        if(cohesionContext > maxContext)
        {
            maxContext = cohesionContext;
        }
         
        GetComponent<SphereCollider>().radius = maxContext;

        currentMoveSpeed = moveSpeed;

        currentTurnSpeed = turnSpeed;


        UpdateBehaviour();
    }

    private void FixedUpdate()
    {

        timer -= Time.deltaTime;
        if(timer < 0)
        {
            UpdateBehaviour();
            timer = updateTimer;
        }

        transform.forward = Vector3.MoveTowards(transform.forward, targetDir, currentTurnSpeed * Time.deltaTime);

        transform.position += transform.forward * currentMoveSpeed * Time.deltaTime;

    }

    private void UpdateBehaviour()
    {

        if (fleeing)
        {
            return;
        }

        Vector3 sumForces = Vector3.zero;

        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, wallContext, groundLayer))
        {
            //Debug.DrawRay(transform.position, transform.forward * hit.distance, Color.red);
            sumForces = transform.forward * -1;
        }
        else
        {
            //Debug.DrawRay(transform.position, transform.forward * wallContext, Color.green);
            sumForces += Seperation() * seperationStrength;
            sumForces += Alignment() * alignmentStrength;
            sumForces += Cohesion() * cohesionStrength;
        }

        targetDir = sumForces;
    }

    private Vector3 Seperation()
    {
        Vector3 seperationVector = Vector3.zero;

        for (int i = 0; i < boids.Count; i++)
        {
            if(Vector3.Distance(transform.position, boids[i].transform.position) < seperationContext)
            {
                Vector3 tempVector = Vector3.zero;
                tempVector += transform.position - boids[i].transform.position;
                tempVector /= Mathf.Pow(Vector3.Distance(transform.position, boids[i].transform.position), 2);
                seperationVector += tempVector;
            }
        }

        return seperationVector.normalized;

    }

    private Vector3 Alignment()
    {
        Vector3 alignmentVector = Vector3.zero;

        for(int i = 0; i < boids.Count; i++)
        {
            if (Vector3.Distance(transform.position, boids[i].transform.position) < alignmentContext)
            {
                alignmentVector += boids[i].TargetDir;
            } 
        }

        alignmentVector /= boids.Count;
        return alignmentVector.normalized;
    }

    private Vector3 Cohesion()
    {
        Vector3 cohesionVector = Vector3.zero;

        for (int i = 0; i < boids.Count; i++)
        {
            if (Vector3.Distance(transform.position, boids[i].transform.position) < cohesionContext)
            {
                cohesionVector += boids[i].transform.position - transform.position;
            }
        }

        cohesionVector /= boids.Count;

        return (cohesionVector).normalized;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boid"))
        {
           spawner.Boids.TryGetValue(other.gameObject, out Boid boid);
           if(boid != null)
           {
               boids.Add(boid);
           }

        }
        else if (other.CompareTag("Player"))
        {
            StopAllCoroutines();
            StartCoroutine(Fleeing(other));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Boid"))
        {
            spawner.Boids.TryGetValue(other.gameObject, out Boid boid);
            if (boid != null)
            {
                boids.Remove(boid);
            }
        }
    }

    private IEnumerator Fleeing(Collider other)
    {
        fleeing = true;
        targetDir = transform.position - other.transform.position;
        currentMoveSpeed = moveSpeed * fleeingMoveSpeedMult;
        yield return new WaitForSeconds(fleeTime);
        fleeing = false;
        currentMoveSpeed = moveSpeed;
    }

    public Vector3 TargetDir
    {
        get => targetDir;
    }

    public float MoveSpeed
    {
        set => moveSpeed = value;
    }

    public float TurnSpeed
    {
        set => turnSpeed = value;
    }

    public float WallContext
    {
        set => wallContext = value;
    }



    public LayerMask GroundLayer
    {
        set => groundLayer = value;
    }
    public float SeperationContext { get => seperationContext; set => seperationContext = value; }
    public float SeperationStrength { get => seperationStrength; set => seperationStrength = value; }
    public float AlignmentContext { get => alignmentContext; set => alignmentContext = value; }
    public float AlignmentStrength { get => alignmentStrength; set => alignmentStrength = value; }
    public float CohesionContext { get => cohesionContext; set => cohesionContext = value; }
    public float CohesionStrength { get => cohesionStrength; set => cohesionStrength = value; }
    public float FleeContext { get => fleeContext; set => fleeContext = value; }
    public float FleeTime { get => fleeTime; set => fleeTime = value; }
    public float FleeingMoveSpeedMult { get => fleeingMoveSpeedMult; set => fleeingMoveSpeedMult = value; }
    public BoidSpawner Spawner {  get => spawner; set => spawner = value; }
}
