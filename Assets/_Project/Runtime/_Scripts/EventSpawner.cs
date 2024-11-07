using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class EventSpawner : MonoBehaviour
{

    [SerializeField] private List<EventRandomizer> spawnObjects = new List<EventRandomizer>();

    public float baseSpawnRate = 1.0f; // The base rate for spawning objects
    public float randomAdjustment = 1.0f; // Maximum adjustment to the base rate
    public float moveLength = 5.0f;
    public float moveSpeed = 2.0f; // Speed of the left-right movement

    private float startLocalX; // Starting local X position of the spawner

    private void Start()
    {

        if (Application.isPlaying)
        {
            startLocalX = transform.localPosition.x;
            StartCoroutine(SpawnObject());
            StartCoroutine(MoveBackAndForth());
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


    // Coroutine to move the spawner back and forth on the local X-axis
    private IEnumerator MoveBackAndForth()
    {
        while (true)
        {
            // Move to the right
            while (transform.localPosition.x < startLocalX + moveLength)
            {
                transform.localPosition += Vector3.right * moveSpeed * Time.deltaTime;
                yield return null;
            }

            // Move to the left
            while (transform.localPosition.x > startLocalX - moveLength)
            {
                transform.localPosition -= Vector3.right * moveSpeed * Time.deltaTime;
                yield return null;
            }
        }
    }



    // Spawning coroutine with randomized spawn rate
    private IEnumerator SpawnObject()
    {
        while (true)
        {

            GameObject spawnObject = RandomizeSpawn();

            if (spawnObject != null)
            {
                Instantiate(spawnObject, transform.position, Quaternion.identity);
            }
            // Spawn the object at the current position of the spawner
            // Calculate a random spawn rate based on the base rate and adjustment
            float spawnTimer = baseSpawnRate + Random.Range(-randomAdjustment, randomAdjustment);
            spawnTimer = Mathf.Max(0.1f, spawnTimer); // Ensure the timer is not less than 0.1 seconds

            // Wait for the calculated random duration
            yield return new WaitForSeconds(spawnTimer);
        }
    }




    // Draw the movement range in the editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 start = transform.position + Vector3.right * moveLength;
        Vector3 end = transform.position - Vector3.right * moveLength;
        Gizmos.DrawLine(start, end);
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