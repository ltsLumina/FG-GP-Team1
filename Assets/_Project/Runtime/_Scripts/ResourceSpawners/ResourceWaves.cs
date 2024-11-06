using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Resource Spawner")]
public class ResourceWaves : ScriptableObject
{

    [Header("Depth for this wave")]
    [SerializeField] public int depth;

    [Header("Enemy types in this wave")]
    [SerializeField] public List<RandomizeObjectsSpawn> spawnObjects = new List<RandomizeObjectsSpawn>();

}


[System.Serializable]
public class RandomizeObjectsSpawn
{
    public GameObject objectToSpawn;
    [SerializeField][Range(0, 10)] public float spawnRate;
}