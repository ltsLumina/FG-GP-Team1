#region
using UnityEngine;
using VInspector;
using static Lumina.Essentials.Modules.Helpers;
#endregion

public class Battery : MonoBehaviour
{
    [SerializeField] Transform startPos;
    [Range(0, 10)]
    [SerializeField] float batteryReturnDistance = 5f;

    [Header("Charge")]
    [RangeResettable(0,100)]
    [SerializeField] float charge = 100f;

    public float Charge
    {
        get => charge;
        set => charge = value;
    }

    [Space]
    
    Train train;
    
    void Start()
    {
        Debug.Assert(startPos, "Start position is not assigned!", this);

        train              = Train.Instance;
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

    void OnDrawGizmosSelected()
    {
        if (PlayerInputsManager.Player1)
        {
            Gizmos.color = Vector3.Distance(PlayerInputsManager.Player1.transform.position, startPos.position) > batteryReturnDistance ? Color.red : Color.green;
            Gizmos.DrawWireSphere(startPos.position, batteryReturnDistance);
        }

        if (PlayerInputsManager.Player2 != null)
        {
            Gizmos.color = Vector3.Distance(PlayerInputsManager.Player2.transform.position, startPos.position) > batteryReturnDistance ? Color.red : Color.green;
            Gizmos.DrawWireSphere(startPos.position, batteryReturnDistance);
        }
    }
}