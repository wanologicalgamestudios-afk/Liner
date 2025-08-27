using UnityEngine;
using System.Collections;

/// <summary>
/// Centralized Sound Manager
/// Usage:
///     SoundManager.Instance.PlaySfx(clip);
///     SoundManager.Instance.PlayMusic(customClip, loop: true);
/// </summary>
public class SoundManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource backgroundAudioSource;
    [SerializeField] private AudioSource effectAudioSource;

    [Header("Clips")]
    [SerializeField] private AudioClip backgroundSound;
    [SerializeField] private AudioClip levelFailSound;
    [SerializeField] private AudioClip levelDoneSound;
    [SerializeField] private AudioClip levelCompleteUIConfettiSound;

    public static bool isSound = true;
    public static bool isMusic = true;


    private void Start()
    {
        if (backgroundSound != null)
            PlayMusic(backgroundSound, true);
    }

    #region Public Functions

    /// <summary>
    /// Play a one-shot sound effect.
    /// </summary>
    public void PlaySfx(AudioClip clip)
    {
        if (clip == null || !isSound) return;
        effectAudioSource.PlayOneShot(clip);
    }

    /// <summary>
    /// Play background music (loop optional).
    /// </summary>
    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (clip == null || !isMusic) return;

        backgroundAudioSource.clip = clip;
        backgroundAudioSource.loop = loop;
        backgroundAudioSource.Play();
    }

    /// <summary>
    /// Stop background music.
    /// </summary>
    public void StopMusic()
    {
        backgroundAudioSource.Stop();
    }

    /// <summary>
    /// Toggle SFX On/Off.
    /// </summary>
    public void ToggleSound(bool enable)
    {
        isSound = enable;
    }

    /// <summary>
    /// Toggle Music On/Off.
    /// </summary>
    public void ToggleMusic(bool enable)
    {
        isMusic = enable;

        if (!isMusic && backgroundAudioSource.isPlaying)
            backgroundAudioSource.Stop();
        else if (isMusic && !backgroundAudioSource.isPlaying && backgroundSound != null)
            PlayMusic(backgroundSound, true);
    }

    #endregion

    #region Helper Wrappers for Common Sounds
    public void PlayLevelFail() => PlaySfx(levelFailSound);
    public void PlayLevelDone() => PlaySfx(levelDoneSound);
    public void PlayConfetti() => PlaySfx(levelCompleteUIConfettiSound);
    #endregion
}
