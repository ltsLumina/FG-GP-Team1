using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-10)]
public class OnScreenControls : MonoBehaviour
{
    [SerializeField] Image img;

    void Start()
    {
        img.gameObject.SetActive(false);
    }

    public void StartFOO()
    {
        StartCoroutine(FOO());
    }
    
    public IEnumerator FOO()
    {
        img.gameObject.SetActive(true);
        yield return new WaitForSeconds(25);
        img.gameObject.SetActive(false);
    }
}