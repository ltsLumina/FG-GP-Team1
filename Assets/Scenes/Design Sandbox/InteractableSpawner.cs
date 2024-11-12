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
    private float intervalTimer;
    private Dictionary<RandomizeObjectsSpawn, int> spawnCounts =
        new Dictionary<RandomizeObjectsSpawn, int>();

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
        StartWave(spawnWaves[0]);
    }

    private void Update()
    {
        // Update the spawner's position to track the transform's y-offset
        transform.position = new Vector3(
            transform.position.x,
            trackTransform.position.y + startYOffset,
            transform.position.z
        );

        // Check if we should start spawning based on the depth
        if (currentWave != null && trackTransform.position.y <= -currentWave.depth)
        {
            intervalTimer += Time.deltaTime;

            foreach (var spawnObject in currentWave.spawnObjects)
            {
                if (spawnCounts[spawnObject] < spawnObject.amountToSpawn)
                {
                    float targetInterval = spawnObject.spawnInterval / spawnObject.amountToSpawn;

                    if (intervalTimer >= targetInterval)
                    {
                        Vector3 spawnPosition = GetSpawnPosition(spawnObject);
                        Quaternion rotation = GetSpawnRotation(spawnObject, spawnPosition);
                        Instantiate(spawnObject.objectToSpawn, spawnPosition, rotation);
                        spawnCounts[spawnObject]++;
                        intervalTimer = 0f; // Reset timer for the next spawn
                    }
                }
            }

            // Reset the spawn counts once all objects have been spawned, to allow continuous spawning
            if (AllObjectsSpawned())
            {
                ResetSpawnCounts();
            }
        }
    }

    private void StartWave(ResourceWaves wave)
    {
        currentWave = wave;
        intervalTimer = 0;
        spawnCounts.Clear();
        foreach (var spawnObject in currentWave.spawnObjects)
        {
            spawnCounts[spawnObject] = 0;
        }
    }

    private void StartNextWave()
    {
        spawnWaves.RemoveAt(0);
        if (spawnWaves.Count > 0)
        {
            StartWave(spawnWaves[0]);
        }
        else
        {
            StartWave(currentWave); // Restart the current wave if no more waves are left
        }
    }

    private bool AllObjectsSpawned()
    {
        foreach (var spawnObject in currentWave.spawnObjects)
        {
            if (spawnCounts[spawnObject] < spawnObject.amountToSpawn)
            {
                return false;
            }
        }
        return true;
    }

    private void ResetSpawnCounts()
    {
        foreach (var spawnObject in currentWave.spawnObjects)
        {
            spawnCounts[spawnObject] = 0;
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
