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
                        Instantiate(
                            spawnObject.objectToSpawn,
                            GetSpawnPosition(spawnObject),
                            Quaternion.identity
                        );
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
        if (spawnObject.spawnOnWall)
        {
            // If spawnOnWall is true, spawn at either end of the x range
            float xPosition = Random.value < 0.5f ? -spawnRangeX : spawnRangeX;
            return new Vector3(xPosition, trackTransform.position.y + startYOffset, 0f);
        }
        else
        {
            // Otherwise, spawn within the range
            return new Vector3(
                Random.Range(-spawnRangeX, spawnRangeX),
                trackTransform.position.y + startYOffset,
                0f
            );
        }
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
