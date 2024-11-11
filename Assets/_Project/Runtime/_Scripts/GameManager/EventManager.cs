using UnityEngine;

public class EventManager : MonoBehaviour
{
    void OnEnable()
    {
        GameManager.Instance.OnIntro += PlayIntroAnimation;
        GameManager.Instance.OnFirstKelp += ShowKelpMessage;
        GameManager.Instance.OnLowFuel += ShowLowFuelWarning;
        GameManager.Instance.OnFirstRock += ShowRockMessage;
        GameManager.Instance.OnCriticalHull += ShowCriticalHullWarning;
        GameManager.Instance.OnFirstJellyfish += ShowJellyfishMessage;
        GameManager.Instance.OnLightOut += ShowLightOutWarning;
        GameManager.Instance.OnLowBattery += ShowLowBatteryWarning;
        GameManager.Instance.OnPlayerDeath += HandlePlayerDeath;
        GameManager.Instance.OnFirstHullDamage += ShowHullDamageMessage;
    }

    void OnDisable()
    {
        GameManager.Instance.OnIntro -= PlayIntroAnimation;
        GameManager.Instance.OnFirstKelp -= ShowKelpMessage;
        GameManager.Instance.OnLowFuel -= ShowLowFuelWarning;
        GameManager.Instance.OnFirstRock -= ShowRockMessage;
        GameManager.Instance.OnCriticalHull -= ShowCriticalHullWarning;
        GameManager.Instance.OnFirstJellyfish -= ShowJellyfishMessage;
        GameManager.Instance.OnLightOut -= ShowLightOutWarning;
        GameManager.Instance.OnLowBattery -= ShowLowBatteryWarning;
        GameManager.Instance.OnPlayerDeath -= HandlePlayerDeath;
        GameManager.Instance.OnFirstHullDamage -= ShowHullDamageMessage;
    }

    void PlayIntroAnimation() => Debug.Log("Playing intro animation...");
    void ShowKelpMessage() => Debug.Log("First time seeing kelp!");
    void ShowLowFuelWarning() => Debug.Log("Fuel is low! Refuel now!");
    void ShowRockMessage() => Debug.Log("First time seeing a rock! Move it out of the way!");
    void ShowCriticalHullWarning() => Debug.Log("Hull is critically damaged! Repair now!");
    void ShowJellyfishMessage() => Debug.Log("First time seeing jellyfish! Use it to charge the battery!");
    void ShowLightOutWarning() => Debug.Log("The light has gone out! Recharge the battery!");
    void ShowLowBatteryWarning() => Debug.Log("Battery is at 20%! Recharge now!");
    void HandlePlayerDeath() => Debug.Log("Player has died.");
    void ShowHullDamageMessage() => Debug.Log("Hull has taken damage! Repair the submarine.");
}
