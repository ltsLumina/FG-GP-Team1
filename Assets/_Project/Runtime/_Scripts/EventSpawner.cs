using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class EventSpawner : MonoBehaviour
{

    [SerializeField] private List<EventRandomizer> spawnObjects = new List<EventRandomizer>();

    public float baseSpawnRate = 1.0f;
    public float randomAdjustment = 1.0f;
    public float moveLength = 5.0f;
    public float moveSpeed = 2.0f;

    private float startLocalX;

    private void Start()
    {

        if (Application.isPlaying)
        {
            startLocalX = transform.localPosition.x;
            StartCoroutine(SpawnObject());
        }
    }

    private void Update()
    {
        float totalSpawnChance = 0;

        for (int i = 0; i < spawnObjects.Count; i++)
        {
            totalSpawnChance += spawnObjects[i].eventChance;
        }

        if (totalSpawnChance > 100)
        {
            for (int i = 0; i < spawnObjects.Count; i++)
            {
                spawnObjects[i].eventChance--;
                spawnObjects[i].eventChance = Mathf.Max(spawnObjects[i].eventChance, 0);
            }
        }

    }


    private IEnumerator SpawnObject()
    {
        while (true)
        {

            GameObject spawnObject = RandomizeSpawn();

            if (spawnObject != null)
            {
                Instantiate(spawnObject, transform.position, Quaternion.identity);
            }

            float spawnTimer = baseSpawnRate + Random.Range(-randomAdjustment, randomAdjustment);
            spawnTimer = Mathf.Max(0.1f, spawnTimer); 

            yield return new WaitForSeconds(spawnTimer);
        }
    }


    private GameObject RandomizeSpawn()
    {
        float totalChance = 0;
        float randomizer = Random.Range(0, 100);

        for (int i = 0; i < spawnObjects.Count; i++)
        {
            float startChance = totalChance;
            totalChance += spawnObjects[i].eventChance;
            if (randomizer > startChance && randomizer < totalChance)
            {
                Debug.Log(spawnObjects[i].eventToTrigger);
                return spawnObjects[i].eventToTrigger;
            }
        }


        return null;
    }

}

[System.Serializable]
public class EventRandomizer
{
    public GameObject eventToTrigger;
    [SerializeField][Range(0, 100)] public float eventChance;
}