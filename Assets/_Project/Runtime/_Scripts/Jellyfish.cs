#region
using Lumina.Essentials.Attributes;
using UnityEngine;
using UnityEngine.Events;
using VInspector;
using Random = UnityEngine.Random;
#endregion

public class Jellyfish : MonoBehaviour
{
    [Tab("Jellyfish")]
    [RangeResettable(0, 5)]
    [SerializeField] int chargeLevel;
    [SerializeField, ReadOnly] bool charged;

    [Tooltip("The percentage of a chargeLevel to convert to a float value. E.g. 1 charge level = 10f")]
    [SerializeField] float conversionRate = 10f;

    [Tab("Collider")]
    [SerializeField] Vector3 offset;
    [SerializeField] int radius;

    [Tab("Events")]
    [SerializeField] UnityEvent<int> onChargeChanged;
    [SerializeField] UnityEvent onChargeDepleted;
    
    public int ChargeLevel => chargeLevel;
    public bool Charged => charged;

    int reductionAmount = 1;
    SphereCollider col;
    
    void Start()
    {
        chargeLevel = Random.Range(0, 5 + 1);
        charged = chargeLevel > 0;

        col = GetComponent<SphereCollider>();
    }

    void Update()
    {
        charged = chargeLevel > 0;
        
        col.isTrigger = true;
        col.radius = radius;
        col.center = offset;
    }

    void OnTriggerEnter(Collider other)
    {
        if (chargeLevel == 0) return;

        Shock();

        return;
        void Shock()
        {
            // Stun the player if they are not holding a battery.
            if (other.TryGetComponent(out Player player))
            {
                if (Player.HoldingResource(out Battery battery))
                {
                    Debug.Log("Battery charged");
                    battery.Charge += ConvertCharge();
                    chargeLevel = 0;
                }
                else
                {
                    Debug.Log("Player stunned");
                    player.Stun();

                    // Reduce the charge level by the current reduction amount
                    chargeLevel = Mathf.Clamp(chargeLevel - reductionAmount, 0, chargeLevel);
                    reductionAmount++;
                }
            }
        }
    }

    /// <summary>
    /// Converts the charge level to a float value.
    /// <example> 1 charge level = 0.1f </example>
    /// </summary>
    /// <returns></returns>
    float ConvertCharge()
    {
        float conversion = chargeLevel * conversionRate;
        Debug.Log(conversion);
        return conversion;
    }
}
