using UnityEngine;

public class UISFXPlayer : MonoBehaviour
{
    [Header("SFX")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip[] buttonClicks;
    [SerializeField] private AudioClip[] sliderClicks;

    [Header("Announcer")]
    [SerializeField] private AudioSource announcerSource;
    [SerializeField] private AudioClip[] announcerTests;
    [SerializeField] private AudioClip rareAnnouncerTest;

    private void PlayRandomSound(AudioClip[] options, AudioSource audioSource)
    {
        audioSource.PlayOneShot(options[Random.Range(0, options.Length)]);
    }

    public void PlayButtonClick()
    {
        PlayRandomSound(buttonClicks, sfxSource);
    }

    public void PlaySliderClick()
    {
        PlayRandomSound(sliderClicks, sfxSource);
    }
    
    public void PlayToggleClick()
    {
        PlayRandomSound(buttonClicks, sfxSource);
    }

    public void PlayAnnouncerTest()
    {
        if (Random.Range(0f, 1f) < 0.05f)
            announcerSource.PlayOneShot(rareAnnouncerTest);
        else
            PlayRandomSound(announcerTests, announcerSource);
    }
}
