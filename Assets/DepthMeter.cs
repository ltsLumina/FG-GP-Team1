using System;
using TMPro;
using UnityEngine;

public class DepthMeter : MonoBehaviour
{
    int Depth => Mathf.FloorToInt(currentDepth);


    public float highScore;
    public float currentDepth;
    private readonly float initialDepth = 20f;

    int MaxDepth
    {
        get => PlayerPrefs.GetInt("MaxDepth", 0);
        set => PlayerPrefs.SetInt("MaxDepth", value);
    }

    [SerializeField]
    TMP_Text depthText;

    [SerializeField]
    TMP_Text maxDepthText;

    [SerializeField]
    TMP_Text highScoreText;

    [SerializeField]
    TMP_Text scoreText;

    void Start()
    {
        currentDepth = 0;
        maxDepthText.text = $"{MaxDepth}m";
        highScoreText.text = $"High Score: {Mathf.Abs(MaxDepth)}m";
        scoreText.text = $"Score: {Depth}m";
    }

    void Update()
    {
        if (GameManager.Instance.ship != null)
            UpdateDepth();

        depthText.text = $"{Depth}m";
        scoreText.text = $"Score: {Depth}m";

        if (Depth > MaxDepth)
        {
            MaxDepth = Depth;
            maxDepthText.text = $"{MaxDepth}m";
            highScoreText.text = $"High Score: {Mathf.Abs(MaxDepth)}m";
        }
    }

    void UpdateDepth()
    {
        currentDepth = GameManager.Instance.ship.transform.position.y * -1 - initialDepth;

        if (currentDepth > highScore)
        {
            highScore = currentDepth;
        }
    }

    // If Max depth wants to be reset use this
    public void ResetMaxDepth()
    {
        PlayerPrefs.DeleteKey("MaxDepth");
        maxDepthText.text = "0m";
        highScoreText.text = "High Score: 0m";
    }

}
