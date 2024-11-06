using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [Header("Current UI Elements")]
    [SerializeField] TextMeshProUGUI currentDepthScore;
    [SerializeField] TextMeshProUGUI currentAlgaeCollected;
    [SerializeField] TextMeshProUGUI currentHolesRepaired;
    [SerializeField] TextMeshProUGUI currentJellyfishCollected;
    [SerializeField] TextMeshProUGUI currentLifeformsScanned;

    [Header("Current Scores")]
    public int depthScore;
    public int algaeCollected;
    public int holesRepaired;
    public int jellyfishCollected;
    public int lifeformsScanned;

    public void Update()
    {
        currentDepthScore.text = depthScore.ToString();
        currentAlgaeCollected.text = algaeCollected.ToString();
        currentHolesRepaired.text = holesRepaired.ToString();
        currentJellyfishCollected.text = jellyfishCollected.ToString();
        currentLifeformsScanned.text = lifeformsScanned.ToString();
    }

    public void AddDepthScore(int amount)
    {
        depthScore += amount;
    }

    public void AddAlgaeCollected(int amount)
    {
        algaeCollected += amount;
    }

    public void AddHolesRepaired(int amount)
    {
        holesRepaired += amount;
    }

    public void AddJellyfishCollected(int amount)
    {
        jellyfishCollected += amount;
    }

    public void AddLifeformsScanned(int amount)
    {
        lifeformsScanned += amount;
    }

    public void ResetGame()
    {
        depthScore = 0;
        algaeCollected = 0;
        holesRepaired = 0;
        jellyfishCollected = 0;
        lifeformsScanned = 0;
    }
}