using TMPro;
using UnityEngine;

public class HighScoreManager : MonoBehaviour
{
    ScoreManager scoreManager;

    [Header("High Score Text")]
    [SerializeField] TextMeshProUGUI highScoreDepthText;
    [SerializeField] TextMeshProUGUI highScoreAlgaeText;
    [SerializeField] TextMeshProUGUI highScoreRepairText;
    [SerializeField] TextMeshProUGUI highScoreJellyFishText;
    [SerializeField] TextMeshProUGUI highScoreLifeFormText;

    [Header("High Score Values")]
    [SerializeField] int highScoreDepth;
    public int HighDepthScore => highScoreDepth;

    [SerializeField] int highScoreAlgae;
    public int HighAlgaeCollected => highScoreAlgae;

    [SerializeField] int highScoreRepair;
    public int HighHolesRepaired => highScoreRepair;

    [SerializeField] int highScoreJellyFish;
    public int HighJellyfishCollected => highScoreJellyFish;

    [SerializeField] int highScoreLifeForm;
    public int HighLifeformsScanned => highScoreLifeForm;

    void Start()
    {
        scoreManager = GetComponent<ScoreManager>();
        LoadHighScores();
        highScoreDepthText.text = highScoreDepth.ToString();
        highScoreAlgaeText.text = highScoreAlgae.ToString();
        highScoreRepairText.text = highScoreRepair.ToString();
        highScoreJellyFishText.text = highScoreJellyFish.ToString();
        highScoreLifeFormText.text = highScoreLifeForm.ToString();
    }

    public void SaveHighScores()
    {
        if (scoreManager.depthScore > highScoreDepth) highScoreDepth = scoreManager.depthScore;
        if (scoreManager.algaeCollected > highScoreAlgae) highScoreAlgae = scoreManager.algaeCollected;
        if (scoreManager.holesRepaired > highScoreRepair) highScoreRepair = scoreManager.holesRepaired;
        if (scoreManager.jellyfishCollected > highScoreJellyFish) highScoreJellyFish = scoreManager.jellyfishCollected;
        if (scoreManager.lifeformsScanned > highScoreLifeForm) highScoreLifeForm = scoreManager.lifeformsScanned;

        PlayerPrefs.SetInt("HighDepthScore", highScoreDepth);
        PlayerPrefs.SetInt("HighAlgaeCollected", highScoreAlgae);
        PlayerPrefs.SetInt("HighHolesRepaired", highScoreRepair);
        PlayerPrefs.SetInt("HighJellyfishCollected", highScoreJellyFish);
        PlayerPrefs.SetInt("HighLifeformsScanned", highScoreLifeForm);

        PlayerPrefs.Save();
    }
    private void LoadHighScores()
    {
        if (PlayerPrefs.HasKey("HighDepthScore")) { highScoreDepth = PlayerPrefs.GetInt("HighDepthScore", 0); }
        if (PlayerPrefs.HasKey("HighAlgaeCollected")) { highScoreAlgae = PlayerPrefs.GetInt("HighAlgaeCollected", 0); }
        if (PlayerPrefs.HasKey("HighHolesRepaired")) { highScoreRepair = PlayerPrefs.GetInt("HighHolesRepaired", 0); }
        if (PlayerPrefs.HasKey("HighJellyfishCollected")) { highScoreJellyFish = PlayerPrefs.GetInt("HighJellyfishCollected", 0); }
        if (PlayerPrefs.HasKey("HighLifeformsScanned")) { highScoreLifeForm = PlayerPrefs.GetInt("HighLifeformsScanned", 0); }
    }
}
