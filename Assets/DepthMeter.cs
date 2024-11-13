using System;
using TMPro;
using UnityEngine;

public class DepthMeter : MonoBehaviour
{
    //Depth rounded down to the nearest int
    int Depth => Mathf.FloorToInt(GameManager.Instance.currentDepth);
    int MaxDepth => Mathf.FloorToInt(GameManager.Instance.highScore);

    [SerializeField]
    TMP_Text depthText;

    [SerializeField]
    TMP_Text maxDepthText;

    // Update is called once per frame
    void Update()
    {
        depthText.text = $"{Depth}m";
        maxDepthText.text = $"{MaxDepth}m";
    }
}
