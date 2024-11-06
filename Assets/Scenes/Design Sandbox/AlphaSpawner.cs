using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lumina.Essentials.Modules;

public class AlphaSpawner : MonoBehaviour
{

    [SerializeField] private List<ResourceWaves> spawnWaves = new List<ResourceWaves>();

    public float baseSpawnRate = 1.0f; // The base rate for spawning objects
    public float randomAdjustment = 1.0f; // Maximum adjustment to the base rate
    public float moveLength = 5.0f;
    public float moveSpeed = 2.0f; // Speed of the left-right movement

    private float startLocalX; // Starting local X position of the spawner

    private float timer;

    private ResourceWaves currentWave;


    private void Start()
    {
        currentWave = spawnWaves[0];
        spawnWaves.Remove(currentWave);
    }

    private void Update()
    {

        timer += Time.deltaTime;


        for (int i = 0; i < currentWave.spawnObjects.Count; i++)
        {
            if (Mathf.Repeat(timer, 2/currentWave.spawnObjects[i].spawnRate) >= 2/ currentWave.spawnObjects[i].spawnRate - Time.deltaTime)
            {
                Instantiate(currentWave.spawnObjects[i].objectToSpawn, spawnPos(), Quaternion.identity);
            }
        }

        for(int i = 0;i < spawnWaves.Count; i++)
        {
            if(Helpers.Find<Train>().Depth < spawnWaves[i].depth)
            {
                currentWave = spawnWaves[i];
                spawnWaves.Remove(currentWave);
            }
        }

    }

    private Vector3 spawnPos()
    {
        return new Vector3(Random.Range(-moveLength, moveLength), transform.position.y, transform.position.z);
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