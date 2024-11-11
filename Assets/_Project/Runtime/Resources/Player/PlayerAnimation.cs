using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField]
    float animationTiming = 0.5f;

    [SerializeField]
    float motorSpeed = 1f;

    [SerializeField]
    float dashMotorSpeed = 5.0f;

    Animator anim;
    InputManager input;

    private void Start()
    {
        anim = GetComponent<Animator>();
        input = transform.parent.GetComponentInChildren<InputManager>();
        SetMotorSpeed(motorSpeed);
    }

    public void Dash()
    {
        anim.SetTrigger("Dash");
        SetMotorSpeed(dashMotorSpeed);
    }

    public void StopDash()
    {
        SetMotorSpeed(motorSpeed);
    }

    private void SetMotorSpeed(float speed)
    {
        anim.SetFloat("MotorSpeed", speed);
    }

    public void Grab(GameObject grabbedObject)
    {
        Debug.Log("Grab animation not implemented");
        // anim.SetTrigger("Grab");
    }

    public void Release()
    {
        Debug.Log("Release animation not implemented");
        // anim.SetTrigger("Release");
    }

    public void Die()
    {
        Debug.Log("Die animation not implemented");
        // anim.SetTrigger("Die");
    }

    public void Spawn()
    {
        Debug.Log("Spawn animation not implemented");
        // anim.SetTrigger("Spawn");
    }

    public void Stun(float stunDuration)
    {
        Debug.Log("Stun animation not implemented");
        // anim.SetTrigger("Stun");
    }

    private void Update()
    {
        // Set blend tree parameters for player movement based on input of new input system Player action Move
        // Do a smooth transition between animations

        SetMovementBlendTree(input.MoveInput);
    }

    // Set blend tree parameters for player movement based on input of new input system Player action Move
    public void SetMovementBlendTree(Vector2 movement)
    {
        // Smooth out the transition between animations, so the player doesn't snap between animations
        float horizontalUpdate = Mathf.Lerp(
            anim.GetFloat("Horizontal"),
            movement.x,
            Time.deltaTime / animationTiming
        );
        float verticalUpdate = Mathf.Lerp(
            anim.GetFloat("Vertical"),
            movement.y,
            Time.deltaTime / animationTiming
        );
        anim.SetFloat("Horizontal", horizontalUpdate);
        anim.SetFloat("Vertical", verticalUpdate);
    }
}
