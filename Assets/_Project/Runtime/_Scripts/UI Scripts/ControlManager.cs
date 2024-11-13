using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ControlManager : MonoBehaviour
{
    public TMP_Text downKeyText, upKeyText, rightKeyText, leftKeyText, dashKeyText, grabKeyText, repairKeyText;
    public Button downKeyButton, upKeyButton, rightKeyButton, leftKeyButton, dashKeyButton, grabKeyButton, repairKeyButton;

    Dictionary<string, KeyCode> keys = new Dictionary<string, KeyCode>();

    string keyRebind = null;

    Dictionary<string, KeyCode> defaultKeys = new Dictionary<string, KeyCode>
    {
        { "Up", KeyCode.W },
        { "Down", KeyCode.S },
        { "Left", KeyCode.A },
        { "Right", KeyCode.D },
        { "Dash", KeyCode.LeftShift },
        { "Grab", KeyCode.E },
        { "Repair", KeyCode.Space }
    };

    void Start()
    {
        // Load saved key bindings
        keys["Up"] = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Up", KeyCode.W.ToString()));
        keys["Down"] = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Down", KeyCode.S.ToString()));
        keys["Left"] = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Left", KeyCode.A.ToString()));
        keys["Right"] = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Right", KeyCode.D.ToString()));
        keys["Dash"] = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Dash", KeyCode.LeftShift.ToString()));
        keys["Grab"] = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Grab", KeyCode.E.ToString()));
        keys["Repair"] = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Repair", KeyCode.Space.ToString()));

        downKeyButton.onClick.AddListener(() => RebindKey("Down"));
        upKeyButton.onClick.AddListener(() => RebindKey("Up"));
        leftKeyButton.onClick.AddListener(() => RebindKey("Left"));
        rightKeyButton.onClick.AddListener(() => RebindKey("Right"));
        dashKeyButton.onClick.AddListener(() => RebindKey("Dash"));
        grabKeyButton.onClick.AddListener(() => RebindKey("Grab"));
        repairKeyButton.onClick.AddListener(() => RebindKey("Repair"));

        UpdateUI();
    }

    void Update()
    {
        if (keyRebind != null && Input.anyKeyDown)
        {
            foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(key))
                {
                    keys[keyRebind] = key;
                    PlayerPrefs.SetString(keyRebind, key.ToString());
                    keyRebind = null;
                    UpdateUI();
                    break;
                }
            }
        }
    }

    void RebindKey(string action)
    {
        keyRebind = action;
    }

    void UpdateUI()
    {
        downKeyText.text = keys["Down"].ToString();
        upKeyText.text = keys["Up"].ToString();
        rightKeyText.text = keys["Right"].ToString();
        leftKeyText.text = keys["Left"].ToString();
        dashKeyText.text = keys["Dash"].ToString();
        grabKeyText.text = keys["Grab"].ToString();
        repairKeyText.text = keys["Repair"].ToString();
    }

    public KeyCode GetKey(string action)
    {
        return keys.ContainsKey(action) ? keys[action] : KeyCode.None;
    }

    public void ResetToDefault()
    {
        foreach (var key in defaultKeys)
        {
            keys[key.Key] = key.Value;
            PlayerPrefs.SetString(key.Key, key.Value.ToString());
        }

        PlayerPrefs.Save();
        UpdateUI();
    }
}