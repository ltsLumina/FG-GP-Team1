#region
using System;
using Lumina.Essentials.Modules;
using UnityEngine;
using VInspector;
#endregion

public class Battery : MonoBehaviour
{
    [SerializeField] Transform startPos;
    [Range(0, 10)]
    [SerializeField] float batteryReturnDistance = 5f;

    [Header("Charge")]
    [SerializeField] float charge = 100f;
    [SerializeField] float chargeRate = 25f;

    [Space]
    
    Train train;
    
    void Start()
    {
        train = Helpers.Find<Train>();
        transform.position = startPos.position;

        charge = 0;
    }
    
    /// <summary>
    /// Deposits the battery into the train.
    /// </summary>
    /// <returns> Returns true if the battery is deposited. (Within range to be deposited), else false. </returns>
    public bool Deposit()
    {
        // If the battery is not within range of the train, return false.
        if (Vector3.Distance(transform.position, startPos.position) > batteryReturnDistance) return false;

        transform.position = startPos.position;
        train.Power += charge;
        charge = 0;
        return true;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Jellyfish"))
        {
            charge += chargeRate * Time.deltaTime;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Vector3.Distance(Helpers.Find<Player>().transform.position, startPos.position) > batteryReturnDistance ? Color.red : Color.green;
        Gizmos.DrawWireSphere(startPos.position, batteryReturnDistance);
    }
}
