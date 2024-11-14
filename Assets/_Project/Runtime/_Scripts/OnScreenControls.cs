using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class OnScreenControls : MonoBehaviour
{
    [SerializeField] Image img;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(25);
        img.gameObject.SetActive(false);
    }
}