using UnityEngine;
using UnityEngine.Audio;

public class MixLevels : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    public void SetAnnouncerVolume(float level)
    {
        audioMixer.SetFloat("AnnouncerVolume", Mathf.Log10(level <= 0 ? 0.001f : level) * 40f);
    }

    public void SetMasterVolume(float level)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(level <= 0 ? 0.001f : level) * 40f);
    }

    public void SetMusicVolume(float level)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(level <= 0 ? 0.001f : level) * 40f);
    }

    public void SetSFXVolume(float level)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(level <= 0 ? 0.001f : level) * 40f);
    }
}
