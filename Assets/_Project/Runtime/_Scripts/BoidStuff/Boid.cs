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

    private int rayPoints;

    private bool fleeing = false;

    private BoidSpawner spawner;

    LayerMask groundLayer = 1 << 3;
    private Vector3 targetDir;
    [SerializeField] private List<Boid> boids = new List<Boid>();
    private float timer;
    private float updateTimer = 0.5f;

    private Camera cam;

    private void Start()
    {
        cam = Camera.main;

        transform.up = -new Vector3(Random.Range(-1f,1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;

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

        CheckWalls(-transform.up);

        CheckVisible();

        if (timer < 0)
        {
            UpdateBehaviour();
            timer = updateTimer;
        }

        Move();

    }

    private void CheckVisible()
    {
        if (Mathf.Abs(cam.transform.position.x - transform.position.x) > 100 || Mathf.Abs(cam.transform.position.y - transform.position.y) > 100 || cam.transform.position.z - transform.position.z > 30)
        {
            Destroy(gameObject);
        }
    }

    private void CheckWalls(Vector3 dir)
    {

        
        RaycastHit hit;

        if (Physics.Raycast(transform.position, dir, out hit, wallContext, groundLayer))
        {
            Debug.DrawRay(transform.position, dir * hit.distance, Color.red);
            targetDir = new Vector3(dir.x, dir.y+0.4f, dir.z);      
            Move();
        }
        else
        {
            Debug.DrawRay(transform.position, dir * wallContext, Color.green);
            timer -= Time.deltaTime;
            currentTurnSpeed = turnSpeed;

        }
    }


    private void Move()
    {
        transform.up = -Vector3.MoveTowards(-transform.up, targetDir, currentTurnSpeed * Time.deltaTime);
        transform.position += -transform.up * currentMoveSpeed * Time.deltaTime;
    }

    private void UpdateBehaviour()
    {

        if (fleeing)
        {
            return;
        }

        Vector3 sumForces = Vector3.zero;



        sumForces += Seperation() * seperationStrength;
        sumForces += Alignment() * alignmentStrength;
        sumForces += Cohesion() * cohesionStrength;


        targetDir = sumForces;
    }

    private Vector3 Seperation()
    {
        Vector3 seperationVector = Vector3.zero;

        for (int i = 0; i < boids.Count; i++)
        {
            if(boids[i] != null && Vector3.Distance(transform.position, boids[i].transform.position) < seperationContext)
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
            if (boids[i] != null && Vector3.Distance(transform.position, boids[i].transform.position) < alignmentContext)
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
            if (boids[i] != null && Vector3.Distance(transform.position, boids[i].transform.position) < cohesionContext)
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
