using TMPro;
using UnityEngine;

public class DepthMeter : MonoBehaviour
{
    [SerializeField]
    float depth => GameManager.Instance.currentDepth;
    float maxDepth => GameManager.Instance.highScore;

    [SerializeField]
    TMP_Text depthText;

    [SerializeField]
    TMP_Text maxDepthText;

    // Update is called once per frame
    void Update()
    {
        depthText.text = $"{depth}m";
        maxDepthText.text = $"{maxDepth}m";
    }
}
