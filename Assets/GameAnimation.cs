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
    MainMenu menuManager;

    void Awake()
    {
        anim = GetComponent<Animator>();
        // Sets itself in the gameManager
        GameManager.Instance.gameAnimation = this;
        menuManager = GetComponentInChildren<MainMenu>();
    }

    void Update()
    {
        // Only modify the animation speed
        anim.speed = Input.GetKey(KeyCode.F) ? 5f : 1f;
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

    public void TurnOnMainMenu()
    {
        Debug.Log("Turn on main menu");
        menuManager.MainMenuPanel.SetActive(true);
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
