using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableSpawner : MonoBehaviour
{
    [SerializeField]
    private Transform trackTransform; // The transform to track (submarine/train)

    [SerializeField]
    private List<ResourceWaves> spawnWaves = new List<ResourceWaves>();

    [SerializeField]
    private float spawnRangeX = 5.0f; // The range in which to spawn objects on the x-axis

    [SerializeField]
    private float spawnYOffset = -35.0f; // The Y offset to spawn slightly off the bottom of the screen

    [SerializeField]
    private bool raycastToWall = true; // Toggle to decide if we should raycast to wall or use predefined x range

    private float startYOffset; // Starting Y offset of the spawner
    private ResourceWaves currentWave;
    private int currentWaveIndex = 0;
    private Dictionary<RandomizeObjectsSpawn, float> spawnTimers =
        new Dictionary<RandomizeObjectsSpawn, float>();
    private Dictionary<RandomizeObjectsSpawn, int> spawnedCount =
        new Dictionary<RandomizeObjectsSpawn, int>();
    private Dictionary<RandomizeObjectsSpawn, float> cooldownTimers =
        new Dictionary<RandomizeObjectsSpawn, float>(); // Cooldown timers for each spawn object

    private void Start()
    {
        if (spawnWaves.Count <= 0)
        {
            Debug.LogError("No waves to spawn");
            enabled = false;
            return;
        }

        if (trackTransform == null)
        {
            Debug.LogError("No track transform assigned");
            enabled = false;
            return;
        }

        startYOffset = transform.position.y - trackTransform.position.y;
        StartWave(spawnWaves[currentWaveIndex]);
    }

    private void Update()
    {
        // Update the spawner's position to track the transform's y-offset
        transform.position = new Vector3(
            transform.position.x,
            trackTransform.position.y + startYOffset,
            transform.position.z
        );

        // Check if we should transition to the next wave
        if (
            currentWaveIndex < spawnWaves.Count - 1
            && trackTransform.position.y <= -spawnWaves[currentWaveIndex + 1].depth
        )
        {
            currentWaveIndex++;
            StartWave(spawnWaves[currentWaveIndex]);
        }

        // Continue spawning objects for the current wave
        if (currentWave != null && trackTransform.position.y <= -currentWave.depth)
        {
            foreach (var spawnObject in currentWave.spawnObjects)
            {
                if (!spawnTimers.ContainsKey(spawnObject))
                {
                    spawnTimers[spawnObject] = 0f;
                    spawnedCount[spawnObject] = 0;
                    cooldownTimers[spawnObject] = 0f;
                }

                spawnTimers[spawnObject] += Time.deltaTime;
                cooldownTimers[spawnObject] -= Time.deltaTime;

                if (spawnTimers[spawnObject] >= currentWave.spawnObjects[0].spawnInterval)
                {
                    // Reset the timer and spawned count every interval
                    spawnTimers[spawnObject] = 0f;
                    spawnedCount[spawnObject] = 0;
                }

                if (
                    spawnedCount[spawnObject] < spawnObject.amountToSpawn
                    && cooldownTimers[spawnObject] <= 0f
                )
                {
                    float targetInterval = spawnObject.spawnInterval / spawnObject.amountToSpawn;
                    if (
                        spawnTimers[spawnObject]
                        >= Random.Range(
                            spawnedCount[spawnObject] * targetInterval,
                            (spawnedCount[spawnObject] + 1) * targetInterval
                        )
                    )
                    {
                        Vector3 spawnPosition = GetSpawnPosition(spawnObject);
                        Quaternion rotation = GetSpawnRotation(spawnObject, spawnPosition);
                        Instantiate(spawnObject.objectToSpawn, spawnPosition, rotation);
                        spawnedCount[spawnObject]++;
                        cooldownTimers[spawnObject] = targetInterval; // Set cooldown timer to prevent immediate re-spawning
                    }
                }
            }
        }
    }

    private void StartWave(ResourceWaves wave)
    {
        Debug.Log("Starting wave: " + wave.name);
        currentWave = wave;
        spawnTimers.Clear();
        spawnedCount.Clear();
        cooldownTimers.Clear();
        foreach (var spawnObject in currentWave.spawnObjects)
        {
            spawnTimers[spawnObject] = 0f;
            spawnedCount[spawnObject] = 0;
            cooldownTimers[spawnObject] = 0f;
        }
    }

    private Vector3 GetSpawnPosition(RandomizeObjectsSpawn spawnObject)
    {
        float yPosition =
            Camera.main.ViewportToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane)).y
            + spawnYOffset;
        if (spawnObject.spawnOnWall)
        {
            float xPosition;
            if (raycastToWall)
            {
                // If raycastToWall is true, perform a raycast in 3D to find the wall position
                Vector3 direction = Random.value < 0.5f ? Vector3.left : Vector3.right;
                Vector3 origin = new Vector3(
                    direction == Vector3.left ? -spawnRangeX : spawnRangeX,
                    yPosition,
                    transform.position.z
                );
                RaycastHit hit;
                if (Physics.Raycast(origin, direction, out hit))
                {
                    xPosition = hit.point.x;
                }
                else
                {
                    // If no wall is hit, fallback to the edge of the spawn range
                    xPosition = direction == Vector3.left ? -spawnRangeX : spawnRangeX;
                }
            }
            else
            {
                // If raycastToWall is false, simply spawn at one end of the x range or the other
                xPosition = Random.value < 0.5f ? -spawnRangeX : spawnRangeX;
            }

            return new Vector3(xPosition, yPosition, 0f);
        }
        else
        {
            // Otherwise, spawn within the range
            return new Vector3(Random.Range(-spawnRangeX, spawnRangeX), yPosition, 0f);
        }
    }

    private Quaternion GetSpawnRotation(RandomizeObjectsSpawn spawnObject, Vector3 spawnPosition)
    {
        if (spawnObject.spawnOnWall)
        {
            // Rotate slightly inward between 0 and 35 degrees
            float angle = Random.Range(0f, 35f);
            if (spawnPosition.x < 0)
            {
                // If spawned on the left, rotate to the left
                return Quaternion.Euler(0f, 0f, -angle);
            }
            else
            {
                // If spawned on the right, rotate to the right
                return Quaternion.Euler(0f, 0f, angle);
            }
        }
        // Default rotation if not on wall
        return Quaternion.identity;
    }

    // Draw the spawn range in the editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 start = transform.position + Vector3.right * spawnRangeX;
        Vector3 end = transform.position - Vector3.right * spawnRangeX;
        Gizmos.DrawLine(start, end);
    }
}
