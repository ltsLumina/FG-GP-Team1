using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;



public class BoidSpawner : MonoBehaviour
{
    [Header("Spawning")]
    [SerializeField] private int amountOfBoids = 300;
    [SerializeField] private int spawnRangeX = 100;
    [SerializeField] private int spawnRangeY = 50;
    [SerializeField] private int spawnRangeZ = 100;

    [SerializeField] private BoidDataSO boidDataSO;

    private Dictionary<GameObject, Boid> boids = new Dictionary<GameObject, Boid>();

    private void Start()
    {
        SpawnFish();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0, 0.2f);
        Gizmos.DrawCube(transform.position, new Vector3(spawnRangeX, spawnRangeY, spawnRangeZ));
    }

    private void SpawnFish()
    {
        for (int i = 0; i < amountOfBoids; i++)
        {
            GameObject fish = Instantiate(boidDataSO.prefab, transform.position, Quaternion.identity);
            fish.transform.position = new Vector3(transform.position.x + (Random.Range(-spawnRangeX, spawnRangeX) / 2), transform.position.y + (Random.Range(-spawnRangeY, spawnRangeY) / 2), transform.position.z + (Random.Range(-spawnRangeZ, spawnRangeZ) / 2));
            fish.transform.parent = transform;
            var boid = fish.AddComponent<Boid>();
            fish.AddComponent<SphereCollider>().isTrigger = true;
            fish.tag = "Boid";
            fish.AddComponent<Rigidbody>().useGravity = false;
            boid.MoveSpeed = boidDataSO.moveSpeed;
            boid.TurnSpeed = boidDataSO.turnSpeed;
            boid.WallContext = boidDataSO.wallContext;
            boid.SeperationStrength = boidDataSO.seperationStrength;
            boid.SeperationContext = boidDataSO.seperationContext;

            boid.AlignmentStrength = boidDataSO.alignmentStrength;
            boid.AlignmentContext = boidDataSO.alignmentContext;

            boid.CohesionStrength = boidDataSO.cohesionStrength;
            boid.CohesionContext = boidDataSO.cohesionContext;

            boid.FleeContext = boidDataSO.fleeContext;
            boid.FleeingMoveSpeedMult = boidDataSO.fleeingMoveSpeedMult;
            boid.FleeTime = boidDataSO.fleeTimeSeconds;

            boid.Spawner = this;

            boids.Add(fish, boid);
        }
    }
    public Dictionary<GameObject, Boid> Boids
    {
        get => boids;
    }

}
