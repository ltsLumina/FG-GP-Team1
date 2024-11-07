using System.Collections;
using System.Collections.Generic;
using Lumina.Essentials.Modules;
using UnityEngine;
using UnityEngine.UI;

public class FuelIndicator : MonoBehaviour
{
#pragma warning disable 0414
    private Train train;
    private Slider slider;
     

    private void Awake()
    {
        train = FindFirstObjectByType<Train>();
        if (train == null) 
            Debug.Log("Fuel indicator couldnt find train object");
            
        slider = GetComponent<Slider>();
        if (train == null) 
            Debug.Log("Fuel indicator couldnt find slider in the components list");
        if (slider == null || train == null) this.enabled = false;    
    }

    private void Update()
    {
        slider.value = train.Fuel / 100f;
    }
}
