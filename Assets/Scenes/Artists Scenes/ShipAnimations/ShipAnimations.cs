using Lumina.Essentials.Modules;
using UnityEngine;

public class ShipAnimations : MonoBehaviour
{
    [SerializeField]
    private Train ship;

    [SerializeField]
    private Animator anim;

    [SerializeField]
    private int damage = 0;

    [SerializeField]
    private int maxHullIntegrity = 3;

    //Slider from 0 - 1
    [Range(0.0f, 1.0f)]
    public float frontLightIntensity = 0.0f;

    //Slider from 0 - 1 Setup for unity inspector
    [Range(0.0f, 1.0f)]
    public float sideLightIntensity = 0.0f;

    void Start()
    {
        if (ship == null) ship = FindObjectOfType<Train>(true);

        //Subscribe to UnityEvent
        ship.OnHullIntegrityChanged.AddListener(onHullIntegrityChanged);
    }

    void onHullIntegrityChanged(int hullIntegrity)
    {
        // Convert Hull integrety to damage
        damage = maxHullIntegrity - hullIntegrity;
        anim.SetInteger("Damage", damage);
    }
}
