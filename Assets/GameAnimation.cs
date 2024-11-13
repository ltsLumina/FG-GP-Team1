using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class GameAnimation : MonoBehaviour
{
    [SerializeField]
    GameObject ploraTitle;

    [SerializeField]
    Train ship;

    [SerializeField]
    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
        // Sets itself in the gameManager
        GameManager.Instance.gameAnimation = this;
    }

    void Update()
    {
        // Only modify the animation speed if we are in the editor
        if (Application.isEditor)
        {
            if (Input.GetKey(KeyCode.F))
            {
                anim.speed = 5f; // Increase animation speed by 5 when 'F' is held down
            }
            else
            {
                anim.speed = 1f; // Reset to normal speed
            }
        }
    }

    public void Play()
    {
        anim.SetTrigger("Play");
    }

    public void IntroPlaying()
    {
        GameManager.Instance.TriggerPlayIntro();
    }

    public void Intro()
    {
        anim.SetTrigger("Intro");
    }

    public void Replay()
    {
        anim.SetTrigger("Replay");
    }

    public void MainMenu()
    {
        anim.SetTrigger("MainMenu");
    }

    void DestroyPloraTitle()
    {
        Destroy(ploraTitle);
    }

    void ActivateShip()
    {
        ship.gameObject.SetActive(true);
    }

    void SpawnPlayer() { }
}
