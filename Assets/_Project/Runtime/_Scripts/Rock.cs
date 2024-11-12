using System;
using System.Collections.Generic;
using Lumina.Essentials.Modules;
using UnityEngine;

public class Rock : MonoBehaviour, IDestructible
{
    [SerializeField] Vector3 direction = Vector3.one;
    [SerializeField] float force = 10;
    [SerializeField] float torque = 10;
    [Tooltip("The weight of the magnetization towards the train. A higher value will make the rock move faster.")]
    [SerializeField] float magnetizationStrength = 0.1f;
    [Tooltip("The maximum distance at which the magnetization effect kicks in.")]
    [SerializeField] float magnetizationDistance = 5f;

    [SerializeField] List<GameObject> rockModels;

    void Start()
    {
        var rockModel = rockModels[UnityEngine.Random.Range(0, rockModels.Count)];
        Instantiate(rockModel, transform);
    }

    void FixedUpdate()
    {
        // Move towards the train very slightly if within the magnetization distance
        var train    = Train.Instance;
        var distance = Vector3.Distance(train.transform.position, transform.position);

        if (distance <= magnetizationDistance)
        {
            var dir = (train.transform.position - transform.position).normalized;
            transform.position += dir * magnetizationStrength / 100;
        }
    }

    void OnCollisionEnter(Collision other)
    {
        // add force and torque to the rock
        var rb = GetComponent<Rigidbody>();
        rb.AddForce(direction   * force, ForceMode.Impulse);
        rb.AddTorque(Vector3.up * torque, ForceMode.Impulse);
    }
}
