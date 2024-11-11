using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Resource Spawner")]
public class ResourceWaves : ScriptableObject
{
    [Header("Depth for this wave to start spawning")]
    [SerializeField]
    public int depth;

    [Header("Spawn types in this wave")]
    [SerializeField]
    public List<RandomizeObjectsSpawn> spawnObjects = new List<RandomizeObjectsSpawn>();
}

[System.Serializable]
public class RandomizeObjectsSpawn
{
    public GameObject objectToSpawn;

    [SerializeField]
    public int amountToSpawn; // How many times to spawn this object in the wave

    [SerializeField]
    public float spawnInterval; // Time interval in which all objects must be spawned

    [SerializeField]
    public bool spawnOnWall; // Whether to spawn the object on the wall or not
}
