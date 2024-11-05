using Lumina.Essentials.Modules;
using UnityEngine;

public class Rock : MonoBehaviour
{
    [SerializeField] Vector3 direction = Vector3.one;
    [SerializeField] float force = 10;
    [SerializeField] float torque = 10;
    [SerializeField] float approachSpeed = 0.1f;

    void FixedUpdate()
    {
        // Move towards the train very slightly
        var   train = Helpers.Find<Train>();
        var   dir   = (train.transform.position - transform.position).normalized;
        transform.position += dir * approachSpeed;
    }

    void OnCollisionEnter(Collision other)
    {
        // add force and torque to the rock
        var rb = GetComponent<Rigidbody>();
        rb.AddForce(direction * force, ForceMode.Impulse);
        rb.AddTorque(Vector3.up * torque, ForceMode.Impulse);
    }
}
