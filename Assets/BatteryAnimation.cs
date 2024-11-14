using UnityEngine;

public class BatteryAnimation : MonoBehaviour
{
    Animator anim;
    Train train;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
        train = Train.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        // Update the animation state
        anim.SetFloat("Charge", train.Power);
    }
}
