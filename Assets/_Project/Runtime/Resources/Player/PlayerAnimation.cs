using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField]
    float animationTiming = 0.5f;

    [SerializeField]
    float motorSpeed = 1f;

    [SerializeField]
    float dashMotorSpeed = 5.0f;

    InputManager input;
    public Animator Animator { get; private set; }

    [SerializeField]
    bool isFakePlayer = false;

    [SerializeField] FMODUnity.EventReference repairSound;

    private void Start()
    {
        input = transform.parent.GetComponentInChildren<InputManager>();
        Animator = GetComponent<Animator>();
        SetMotorSpeed(motorSpeed);
    }

    public void Dash()
    {
        Animator.SetTrigger("Dash");
        SetMotorSpeed(dashMotorSpeed);
    }

    public void Spawn()
    {
        Animator.SetTrigger("Spawn");
    }

    public void PlayRepairAudioHit()
    {
        var repair = FMODUnity.RuntimeManager.CreateInstance(repairSound);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(repair, transform);
        repair.start();
    }

    public void StopDash()
    {
        SetMotorSpeed(motorSpeed);
    }

    private void SetMotorSpeed(float speed)
    {
        Animator.SetFloat("MotorSpeed", speed);
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

    public void Stun(float stunDuration)
    {
        Debug.Log("Stun animation not implemented");
        Animator.SetTrigger("Stunned");
    }

    private void Update()
    {
        // Set blend tree parameters for player movement based on input of new input system Player action Move
        // Do a smooth transition between animations
        if (!isFakePlayer)
            SetMovementBlendTree(input.MoveInput);
    }

    public void ObjectReleased()
    {
        // TODO Make object release correctly
        //???
        
    }

    // Set blend tree parameters for player movement based on input of new input system Player action Move
    public void SetMovementBlendTree(Vector2 movement)
    {
        // Smooth out the transition between animations, so the player doesn't snap between animations
        float horizontalUpdate = Mathf.Lerp(
            Animator.GetFloat("Horizontal"),
            movement.x,
            Time.deltaTime / animationTiming
        );
        float verticalUpdate = Mathf.Lerp(
            Animator.GetFloat("Vertical"),
            movement.y,
            Time.deltaTime / animationTiming
        );
        Animator.SetFloat("Horizontal", horizontalUpdate);
        Animator.SetFloat("Vertical", verticalUpdate);
    }
}
