using UnityEngine;

public class BatteryAnimation : MonoBehaviour
{
    Battery battery;
    Animator anim;
    Train train;
    Resource resource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        battery = GetComponent<Battery>();
        resource = battery.GetComponent<Resource>();
        anim = GetComponent<Animator>();
        train = Train.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        // Update the animation state
        if (resource.Grabbed)
        {
            anim.SetFloat("Charge", battery.Charge + train.Power);
        }
        else // on ship
        {
            anim.SetFloat("Charge", train.Power);
        }
    }
}
