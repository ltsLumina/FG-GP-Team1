using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class OptionsMenu : MonoBehaviour
{ 
    //Audio
    [Header("Audio")]
    public TMP_Text masterLabel, musicLabel, sfxLabel;
    public Slider masterSlider, musicSlider, sfxSlider;

    //Resolution
    public List<Resolution> resolutions;

    [Header("Resolution")]
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;
    public Toggle vsyncToggle;

    //Graphics
    [Header("Graphics")]
    public TMP_Dropdown graphicsDropdown;
    public Slider brightnessSlider;
    public Toggle shadowsToggle;

    // UI Contexts
    [Header("Settings UI Contexts")]
    [SerializeField] GameObject mainMenuSettingsUI;
    [SerializeField] GameObject pauseMenuSettingsUI;

    private GameObject currentSettingsUI;

    void Start()
    {
        InitializeSettings();
    }

    void InitializeSettings()
    {
        // Audio Settings

        /*if (PlayerPrefs.HasKey("MasterVol"))
        {
            volume = PlayerPrefs.GetFloat("MasterVol");
        }
        else
        {
            PlayerPrefs.SetFloat("MasterVol", volume);
        }*/
        masterSlider.value = PlayerPrefs.GetFloat("MasterVol");
        masterLabel.text = Mathf.RoundToInt(masterSlider.value * 100).ToString();
        SetMasterMixerVolume(masterSlider.value);

        /*if (PlayerPrefs.HasKey("MusicVol"))
        {
            volume = PlayerPrefs.GetFloat("MusicVol");
        }
        else
        {
            PlayerPrefs.SetFloat("MusicVol", volume);
        }*/
        musicSlider.value = PlayerPrefs.GetFloat("MusicVol");
        musicLabel.text = Mathf.RoundToInt(musicSlider.value * 100).ToString();
        SetMusicMixerVolume(musicSlider.value);

        /*if (PlayerPrefs.HasKey("SFXVol"))
        {
            volume = PlayerPrefs.GetFloat("SFXVol");
        }
        else
        {
            PlayerPrefs.SetFloat("SFXVol", volume);
        }*/
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVol");
        sfxLabel.text = Mathf.RoundToInt(sfxSlider.value * 100).ToString();
        SetSFXMixerVolume(sfxSlider.value);

        // Resolution Settings
        resolutions = new List<Resolution>(Screen.resolutions);
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Count; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = PlayerPrefs.GetInt("ResolutionIndex", currentResolutionIndex);
        resolutionDropdown.RefreshShownValue();

        fullscreenToggle.isOn = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);

        vsyncToggle.isOn = PlayerPrefs.GetInt("VSync", 1) == 1;
        vsyncToggle.onValueChanged.AddListener(SetVSync);

        // Graphics Settings
        graphicsDropdown.value = PlayerPrefs.GetInt("GraphicsQuality", PlayerPrefs.GetInt("Graphics"));
        graphicsDropdown.onValueChanged.AddListener(SetGraphics);

        brightnessSlider.value = PlayerPrefs.GetFloat("Brightness", 1f);
        brightnessSlider.onValueChanged.AddListener(SetBrightness);

        shadowsToggle.isOn = PlayerPrefs.GetInt("Shadows", 1) == 1;
        shadowsToggle.onValueChanged.AddListener(SetShadows);

        ApplySavedSettings();
    }

    public void ShowSettingsForMainMenu()
    {
        if (pauseMenuSettingsUI != null) pauseMenuSettingsUI.SetActive(false);
        if (mainMenuSettingsUI != null) mainMenuSettingsUI.SetActive(true);

        currentSettingsUI = mainMenuSettingsUI;
    }

    public void ShowSettingsForPauseMenu()
    {
        if (mainMenuSettingsUI != null) mainMenuSettingsUI.SetActive(false);
        if (pauseMenuSettingsUI != null) pauseMenuSettingsUI.SetActive(true);

        currentSettingsUI = pauseMenuSettingsUI;
    }

    public void HideSettings()
    {
        if (currentSettingsUI != null) currentSettingsUI.SetActive(false);
    }

    //Audio
    public void SetMasterVolume()
    {
        masterLabel.text = Mathf.RoundToInt(masterSlider.value * 100).ToString();
        SetMasterMixerVolume(masterSlider.value);
        PlayerPrefs.SetFloat("MasterVol", masterSlider.value);
        //PlayerPrefs.Save();
    }

    public void SetMusicVolume()
    {
        musicLabel.text = Mathf.RoundToInt(musicSlider.value * 100).ToString();
        SetMusicMixerVolume(musicSlider.value);
        PlayerPrefs.SetFloat("MusicVol", musicSlider.value);
        //PlayerPrefs.Save();
    }

    public void SetSFXVolume()
    {
        sfxLabel.text = Mathf.RoundToInt(sfxSlider.value * 100).ToString();
        SetSFXMixerVolume(sfxSlider.value);
        PlayerPrefs.SetFloat("SFXVol", sfxSlider.value);
        //PlayerPrefs.Save();
    }

    void SetMasterMixerVolume(float value)
    {
        //TODO: has to be turned on before turn in!
        //FMODUnity.RuntimeManager.GetVCA("vca:/Master").setVolume(value);
    }
    void SetMusicMixerVolume(float value)
    {
        //FMODUnity.RuntimeManager.GetVCA("vca:/Music").setVolume(value);
    }
    void SetSFXMixerVolume(float value)
    {
        //FMODUnity.RuntimeManager.GetVCA("vca:/SFX").setVolume(value);
    }

    //Resolution
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        PlayerPrefs.SetInt("ResolutionIndex", resolutionIndex);
    }

    public void SetVSync(bool isEnabled)
    {
        QualitySettings.vSyncCount = isEnabled ? 1 : 0;
        PlayerPrefs.SetInt("VSync", isEnabled ? 1 : 0);
    }

    //Graphics
    public void SetGraphics(int graphicsIndex)
    {
        QualitySettings.SetQualityLevel(graphicsIndex);
        PlayerPrefs.SetInt("Graphics", graphicsIndex);
    }
    private void SetBrightness(float brightness)
    {
        RenderSettings.ambientLight = Color.white * brightness;
        PlayerPrefs.SetFloat("Brightness", brightness);
    }
    private void SetShadows(bool enableShadows)
    {
        QualitySettings.shadows = enableShadows ? ShadowQuality.All : ShadowQuality.Disable;
        PlayerPrefs.SetInt("Shadows", enableShadows ? 1 : 0);
    }

    void ApplySavedSettings()
    {
        // Vsync
        int savedVSync = PlayerPrefs.GetInt("VSync", 1);
        QualitySettings.vSyncCount = savedVSync == 1 ? 1 : 0;

        // Graphics
        QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("GraphicsQuality", QualitySettings.GetQualityLevel()));
        RenderSettings.ambientLight = Color.white * PlayerPrefs.GetFloat("Brightness", 1f);

        // Shadows
        bool shadowsEnabled = PlayerPrefs.GetInt("Shadows", 1) == 1;
        QualitySettings.shadows = shadowsEnabled ? ShadowQuality.All : ShadowQuality.Disable;
    }
}