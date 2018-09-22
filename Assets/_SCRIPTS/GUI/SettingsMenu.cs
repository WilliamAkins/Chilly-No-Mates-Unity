using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour {

    public AudioMixer audioMixer;

    public AudioSource musicSource;

    public Dropdown resolutionDropdown;

    public Dropdown qualityDropdown;

    public Toggle fullscreenToggle;

    Resolution[] resolutions;

    string[] qualityNames;

    public float musicFloat;

    private void Start()
    {

        fullscreenToggle.isOn = Screen.fullScreen;
        qualityNames = QualitySettings.names;
        resolutions = Screen.resolutions;

        qualityDropdown.ClearOptions();
        resolutionDropdown.ClearOptions();

        List<string> qualities = new List<string>();

        foreach(string s in qualityNames)
        {
            qualities.Add(s);
        }

        qualityDropdown.AddOptions(qualities);
        qualityDropdown.value = QualitySettings.GetQualityLevel();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i <resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);
            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);

        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        musicFloat = musicSource.volume;

    }
    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
    }
    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume * 0.60f;
        musicFloat = musicSource.volume;
    }

    public void SetQuality(int index)
    {
        QualitySettings.SetQualityLevel(index);
    }

    public void SetFullscreen (bool fullscreen)
    {
        Screen.fullScreen = fullscreen;
    }

    public void SetResolution(int index) 
    {
            Screen.SetResolution(resolutions[index].width, resolutions[index].height, Screen.fullScreen);
    } 
}
