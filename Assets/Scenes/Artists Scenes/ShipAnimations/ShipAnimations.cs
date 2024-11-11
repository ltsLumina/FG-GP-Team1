using UnityEngine;

public class ShipAnimations : MonoBehaviour
{
    [SerializeField]
    private Animator anim;

    [SerializeField]
    private int damage = 0;

    //Slider from 0 - 1
    [Range(0.0f, 1.0f)]
    public float frontLightIntensity = 0.0f;

    //Slider from 0 - 1 Setup for unity inspector
    [Range(0.0f, 1.0f)]
    public float sideLightIntensity = 0.0f;

    void Update()
    {
        //For debug
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Damage Ship");
            DamageShip();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Repair Ship");
            RepairShip();
        }

        //Set the front light intensity
        anim.SetFloat("FrontLightStrength", frontLightIntensity);
        //Set the side light intensity
        anim.SetFloat("SideLightStrength", sideLightIntensity);
    }

    void DamageShip()
    {
        damage++;
        anim.SetInteger("Damage", damage);
    }

    void RepairShip()
    {
        damage--;
        anim.SetInteger("Damage", damage);
    }
}
