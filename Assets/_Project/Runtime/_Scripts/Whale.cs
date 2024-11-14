using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Whale : MonoBehaviour
{
    public static int SpawnCounter = 0;

    private float _movementSpeed = 4.75f;

    void Start()
    {
        SpawnCounter++;
        if (SpawnCounter < 3)
        {
            Destroy(gameObject, 60);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, transform.position+transform.forward, _movementSpeed * Time.deltaTime);
    }

}
