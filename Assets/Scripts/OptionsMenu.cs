using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [Header("UI References")]
    public Slider musicSlider;
    public Toggle fullscreenToggle;
    public Dropdown resolutionDropdown;

    private Resolution[] resolutions;

    void Start()
    {
        // Load saved settings
        float savedVolume = PlayerPrefs.GetFloat("musicVolume", 1f);
        musicSlider.value = savedVolume;
        AudioListener.volume = savedVolume;
        fullscreenToggle.isOn = PlayerPrefs.GetInt("fullscreen", 1) == 1;

        AudioListener.volume = musicSlider.value;

        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        int currentResolutionIndex = 0;
        var options = new System.Collections.Generic.List<string>();

        for (int i = 0; i < resolutions.Length; i++)
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
        resolutionDropdown.value = PlayerPrefs.GetInt("resolutionIndex", currentResolutionIndex);
        resolutionDropdown.RefreshShownValue();

        ApplyResolution(resolutionDropdown.value);
        musicSlider.onValueChanged.AddListener(SetVolume);
    }

    public void SetVolume(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("musicVolume", value);
    }

    public void OnMusicVolumeChanged(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("musicVolume", value);
    }

    public void OnFullscreenToggle(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("fullscreen", isFullscreen ? 1 : 0);
    }

    public void OnResolutionChanged(int index)
    {
        ApplyResolution(index);
        PlayerPrefs.SetInt("resolutionIndex", index);
    }

    private void ApplyResolution(int index)
    {
        Resolution res = resolutions[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
    }

    public void OnBackClicked(GameObject panel)
    {
        PlayerPrefs.Save();
        panel.SetActive(false);
    }
}
