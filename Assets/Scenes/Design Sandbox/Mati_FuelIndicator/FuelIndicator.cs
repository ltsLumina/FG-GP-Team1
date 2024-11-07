#region
using UnityEngine;
using UnityEngine.UI;
#endregion

public class FuelIndicator : MonoBehaviour
{
#pragma warning disable 0414
    Train train;
    Slider slider;

    void Awake()
    {
        train = FindFirstObjectByType<Train>();
        if (train == null) Debug.Log("Fuel indicator couldnt find train object");

        slider = GetComponent<Slider>();
        if (train == null) Debug.Log("Fuel indicator couldnt find slider in the components list");
        if (slider == null || train == null) enabled = false;
    }

    void Update() => slider.value = train.Fuel / 100f;
}
