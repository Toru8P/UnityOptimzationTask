using UnityEngine;
using UnityEngine.Audio;

namespace MainGame.Managers
{
    public class SoundManager : MonoBehaviour
    {
        private const string MASTER_VOLUME_PARAMETER = "MasterVolume";
        private const string SFX_VOLUME_PARAMETER = "SFXVolume";
    
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
    }
}
