using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    private static string MASTER_VOLUME_PARAMETER = "MasterVolume";
    private static string SFX_VOLUME_PARAMETER = "SFXVolume";
    private static string MUSIC_VOLUME_PARAMETER = "MusicVolume";

    [SerializeField] private AudioMixerGroup mainMixer;
    [SerializeField] private AudioMixerGroup sfxMixer;
    [SerializeField] private AudioMixerGroup musicMixer;

    public void MasterVolumeSliderChanged(float newValue)
    {
        float actualVolumeValue = (1 - newValue) * -40;
        mainMixer.audioMixer.SetFloat(MASTER_VOLUME_PARAMETER, actualVolumeValue);
    }
    
    public void SFXVolumeSliderChanged(float newValue)
    {
        float actualVolumeValue = (1 - newValue) * -40;
        mainMixer.audioMixer.SetFloat(SFX_VOLUME_PARAMETER, actualVolumeValue);
    }

    public void MusicVolumeSliderChanged(float newValue)
    {
        float actualVolumeValue = (1 - newValue) * -40;
        if (mainMixer != null && mainMixer.audioMixer != null)
            mainMixer.audioMixer.SetFloat(MUSIC_VOLUME_PARAMETER, actualVolumeValue);
    }
}
