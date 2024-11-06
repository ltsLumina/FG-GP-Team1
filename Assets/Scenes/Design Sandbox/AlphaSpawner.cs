using System.Collections;
using UnityEngine;

public class AlphaSpawner : MonoBehaviour
{
    public GameObject objectToSpawn;
    public float baseSpawnRate = 1.0f; // The base rate for spawning objects
    public float randomAdjustment = 1.0f; // Maximum adjustment to the base rate
    public float moveLength = 5.0f;
    public float moveSpeed = 2.0f; // Speed of the left-right movement

    private float startLocalX; // Starting local X position of the spawner

    private void Start()
    {
        // Store the initial local X position
        startLocalX = transform.localPosition.x;
        StartCoroutine(SpawnObject());
        StartCoroutine(MoveBackAndForth());
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
            // Spawn the object at the current position of the spawner
            Instantiate(objectToSpawn, transform.position, Quaternion.identity);

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
}
