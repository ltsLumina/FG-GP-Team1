using UnityEngine;

public class Rock : MonoBehaviour
{
    [SerializeField] Vector3 direction = Vector3.one;
    [SerializeField] float force = 10;
    [SerializeField] float torque = 10;
    

    
    
    void OnCollisionEnter(Collision other)
    {
        // add force and torque to the rock
        var rb = GetComponent<Rigidbody>();
        rb.AddForce(direction * force, ForceMode.Impulse);
        rb.AddTorque(Vector3.up * torque, ForceMode.Impulse);
    }
}
