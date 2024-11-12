using UnityEngine;

public class BatteryAnimation : MonoBehaviour
{
    Battery battery;
    Animator anim;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        battery = GetComponent<Battery>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // Update the animation state
        anim.SetFloat("Charge", battery.Charge);
    }
}
