using System;
using System.Collections.Generic;
using Lumina.Essentials.Modules;
using UnityEngine;

public class Rock : MonoBehaviour, IDestructible
{
    [SerializeField]
    Vector3 direction = Vector3.one;

    [SerializeField]
    float force = 10;

    [SerializeField]
    List<GameObject> rockModels;

    bool collided = false;

    void Start()
    {
        var rockModel = rockModels[UnityEngine.Random.Range(0, rockModels.Count)];
        Instantiate(rockModel, transform);
        rockModel.transform.position = Vector3.zero;
    }

    void FixedUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, Train.Instance.transform.position, 0.05f);
    }

    void OnCollisionEnter(Collision other)
    {
        // add force and torque to the rock
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Ship"))
        {
            if (collided) return;
            collided = true;
            
            var rb = GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.None;
            rb.AddForce(direction * force, ForceMode.Impulse);
        }
    }
}
