using System.Collections;
using System.Collections.Generic;
using Lumina.Essentials.Modules;
using UnityEngine;

public class AlphaSpawner : MonoBehaviour
{
    #pragma warning disable 0414
    
    [SerializeField]
    // Transform to track
    private Transform trackTransform;

    [SerializeField]
    private List<ResourceWaves> spawnWaves = new List<ResourceWaves>();

    [SerializeField]
    float baseSpawnRate = 1.0f; // The base rate for spawning objects

    [SerializeField]
    float randomAdjustment = 1.0f; // Maximum adjustment to the base rate

    [SerializeField]
    float moveLength = 5.0f;

    [SerializeField]
    float moveSpeed = 2.0f; // Speed of the left-right movement

    private float startYOffset; // Starting Y offset of the spawner

    private float startLocalX; // Starting local X position of the spawner

    private float timer;

    private ResourceWaves currentWave;

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

        // Get the distance from the spawner to the tracked object
        startYOffset = transform.position.y - trackTransform.position.y;
        currentWave = spawnWaves[0];
        spawnWaves.Remove(currentWave);
        startLocalX = transform.localPosition.x;
    }

    private void Update()
    {
        if (currentWave == null)
            return;

        // track the transform's y position with the spawners y-offset
        transform.position = new Vector3(
            transform.position.x,
            trackTransform.position.y + startYOffset,
            transform.position.z
        );

        timer += Time.deltaTime;

        for (int i = 0; i < currentWave.spawnObjects.Count; i++)
        {
            if (
                Mathf.Repeat(timer, 2 / currentWave.spawnObjects[i].spawnRate)
                >= 2 / currentWave.spawnObjects[i].spawnRate - Time.deltaTime
            )
            {
                Instantiate(
                    currentWave.spawnObjects[i].objectToSpawn,
                    spawnPos(),
                    Quaternion.identity
                );
            }
        }

        for (int i = 0; i < spawnWaves.Count; i++)
        {
            if (Train.Instance.Depth < spawnWaves[i].depth)
            {
                currentWave = spawnWaves[i];
                spawnWaves.Remove(currentWave);
            }
        }
    }

    private Vector3 spawnPos()
    {
        return new Vector3(
            Random.Range(-moveLength, moveLength),
            transform.position.y,
            transform.position.z
        );
    }

    // Draw the movement range in the editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 start = transform.position + Vector3.right * moveLength;
        Vector3 end = transform.position - Vector3.right * moveLength;
        Gizmos.DrawLine(start, end);
    }
}
