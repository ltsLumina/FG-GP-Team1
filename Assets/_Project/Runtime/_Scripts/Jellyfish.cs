#region
using Lumina.Essentials.Attributes;
using UnityEngine;
using UnityEngine.Events;
using VInspector;
using Random = UnityEngine.Random;
#endregion

public class Jellyfish : MonoBehaviour
{
    [Tab("Collider")]
    [SerializeField] Vector3 offset;
    [SerializeField] int radius;
    
    SphereCollider col;
    
    void Start()
    {
        col = GetComponent<SphereCollider>();
    }

    void Update()
    {
        col.isTrigger = true;
        col.radius = radius;
        col.center = offset;
    }

    void OnTriggerEnter(Collider other)
    {
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
                    battery.Charge = 100;
                }
                else
                {
                    Debug.Log("Player stunned");
                    player.Stun();
                }
            }
        }
    }
}
