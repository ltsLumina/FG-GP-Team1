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

    [Header("Audio")]
    [SerializeField] FMODUnity.EventReference shockSound;
    
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

    bool canStun = true;

    void OnTriggerEnter(Collider other)
    {
        Shock();

        return;
        void Shock()
        {
            // Stun the player if they are not holding a battery.
            if (other.TryGetComponent(out Player player))
            {

                var shock = FMODUnity.RuntimeManager.CreateInstance(shockSound);
                FMODUnity.RuntimeManager.AttachInstanceToGameObject(shock, transform);
                shock.start();

                if (Player.HoldingResource(out Battery battery))
                {
                    Debug.Log("Battery charged");
                    battery.Charge = 100;
                    canStun = false;
                }
                else
                {
                    if (!canStun) return;
                    Debug.Log("Player stunned");
                    player.Stun();
                }
            }
        }
    }
}
