using UnityEngine;

public class PlayerAnimationStates : MonoBehaviour
{
    [SerializeField]
    float animationTiming = 0.5f;

    Animator anim;
    InputManager input;

    private void Start()
    {
        anim = GetComponent<Animator>();
        // Get parent gameobject and get component in children
        input = transform.parent.GetComponentInChildren<InputManager>();
    }

    public void Dash()
    {
        anim.SetTrigger("Dash");
    }

    public void Grab(GameObject grabbedObject)
    {
        anim.SetTrigger("Grab");
    }

    public void Release()
    {
        anim.SetTrigger("Release");
    }

    public void Die()
    {
        anim.SetTrigger("Die");
    }

    public void Spawn()
    {
        anim.SetTrigger("Spawn");
    }

    public void Stun()
    {
        anim.SetTrigger("Stun");
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
