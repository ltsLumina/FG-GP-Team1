using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class GameAnimation : MonoBehaviour
{
    [SerializeField]
    GameObject ploraTitle;

    [SerializeField]
    Train ship;

    Animator anim;

    void Start()
    {
        if (playerAnim == null)
        {
            playerAnim = FindFirstObjectByType<PlayerAnimation>();
        }

        anim = GetComponent<Animator>();
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

    public void Intro()
    {
        anim.SetTrigger("Intro");
    }

    public void Replay()
    {
        anim.SetTrigger("Replay");
    }

    void DestroyPloraTitle()
    {
        Destroy(ploraTitle);
    }

    void ActivateShip()
    {
        ship.gameObject.SetActive(true);
        StartCoroutine(SpeedUpShipOverTime());
    }

    IEnumerator SpeedUpShipOverTime()
    {
        // Speed up the ship from 0 to 5 over 15s
        float time = 0;
        while (time < 15)
        {
            time += Time.deltaTime;
            ship.Speed = Mathf.Lerp(0, 5, time / 15);
            yield return null;
        }
    }
}
