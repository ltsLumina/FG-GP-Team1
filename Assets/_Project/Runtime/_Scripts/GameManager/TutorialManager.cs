using Lumina.Essentials.Modules;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public GameState state;

    public TextMeshProUGUI tutorialText;

    // Movement check
    bool pressedW;
    bool pressedA;
    bool pressedS;
    bool pressedD;

    bool isObjectGrabbed;
    bool checkFuelInteraction;
    bool checkRepairInteraction;
    bool checkRechargeInteraction;

    bool lowFuel;

    public enum GameState
    {
        Movement,
        Pickup,
        Fuel,
        Repair,
        Recharge,
        TurtorialDone,

        // Add more game states if needed
    }

    void Start()
    {
        GameStateChanger(GameState.Movement);
        
        Player.OnGrab += () => isObjectGrabbed = true;
        Train.Instance.OnRefuel.AddListener(() => checkFuelInteraction = true);
        Train.Instance.OnHullRepair.AddListener(_ => checkRepairInteraction = true);
        Train.Instance.OnRecharge.AddListener(() => checkRechargeInteraction = true);
    }

    void Update()
    {
        switch (state)
        {
            case GameState.Movement:
                // Player try movement
                tutorialText.text = "Use WASD to move around.";
                if (Input.GetKey(KeyCode.W)) pressedW = true;
                if (Input.GetKey(KeyCode.A)) pressedA = true;
                if (Input.GetKey(KeyCode.S)) pressedS = true;
                if (Input.GetKey(KeyCode.D)) pressedD = true;

                if (pressedW && pressedA && pressedS && pressedD)
                {
                    TurtorialPickup();
                }
                break;

            case GameState.Pickup:
                // Player try pickup
                tutorialText.text = "Press E to pick up items.";
                if (Input.GetKeyDown(KeyCode.E)  && isObjectGrabbed)
                {
                    TurtorialFuel();
                }
                break;

            case GameState.Fuel:
                // Player try fuel
                tutorialText.text = "Place fuel into the submarine to keep it running.";
                if (checkFuelInteraction)
                {
                    TurtorialRepair();
                }
                break;

            case GameState.Repair:
                // Player try repair
                tutorialText.text = "Repair the submarine hull by interacting with it.";
                if (checkRepairInteraction)
                {
                    TurtorialRecharge();
                }
                break;

            case GameState.Recharge:
                // Player try recharge
                tutorialText.text = "Recharge the submarine battery using collected energy.";
                if (CheckRechargeInteraction())
                {
                    TurtorialDone();
                }
                break;

            case GameState.TurtorialDone:
                // Change scene
                tutorialText.text = "Tutorial Complete! Good luck!";
                Invoke(nameof(LoadMainGameScene), 3f);
                break;
                
                // Add more cases if needed
        }
    }

    public void GameStateChanger(GameState currentState)
    {
        state = currentState;
    }

    //Movement
    public void TurtorialMovement()
    {
        GameStateChanger(GameState.Movement);
    }

    //Pickup
    public void TurtorialPickup()
    {
        GameStateChanger(GameState.Pickup);
    }
    public void SetGrabbedObject(bool isGrabbed)
    {
        isGrabbed = isObjectGrabbed;
    }

    //Fuel
    public void TurtorialFuel()
    {
        GameStateChanger(GameState.Fuel);
    }

    //Repair
    public void TurtorialRepair()
    {
        GameStateChanger(GameState.Repair);
    }

    //Recharge
    public void TurtorialRecharge()
    {
        GameStateChanger(GameState.Recharge);
    }

    bool CheckRechargeInteraction()
    {
        // Replace this with actual logic for checking if recharge is done
        return true; // Placeholder
    }

    //Done
    public void TurtorialDone()
    {
        GameStateChanger(GameState.TurtorialDone);
    }

    void LoadMainGameScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Sandbox");
    }
}