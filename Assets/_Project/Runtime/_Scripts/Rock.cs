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
    float torque = 10;

    private bool collided = false;

    [SerializeField]
    List<GameObject> rockModels;

    void Start()
    {
        var rockModel = rockModels[UnityEngine.Random.Range(0, rockModels.Count)];
        Instantiate(rockModel, transform);
    }

    void OnCollisionEnter(Collision other)
    {
        // add force and torque to the rock
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Ship"))
        {
            var rb = GetComponent<Rigidbody>();
            collided = true;
            rb.AddForce(direction * force, ForceMode.Impulse);
            rb.AddTorque(Vector3.up * torque, ForceMode.Impulse);
        }
    }
}
