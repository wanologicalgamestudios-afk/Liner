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
    [SerializeField] private AudioClip buttonClickSound;

    public static bool isSound = true;
    public static bool isMusic = true;


    private void Start()
    {
        CheckSoundAndMusic();
    }

    private void CheckSoundAndMusic() 
    {
        if (UIManager.GetInstance().GameManager.IsSoundOff == 0)
        {
            ToggleSound(true);
        }
        else 
        {
            ToggleSound(false);
        }

        if (UIManager.GetInstance().GameManager.IsMusicOff == 0)
        {
            ToggleMusic(true);
        }
        else 
        {
            ToggleMusic(false);
        }
    }

    #region Public Functions

    /// <summary>
    /// Play a one-shot sound effect.
    /// </summary>
    private void PlaySfx(AudioClip clip)
    {
        if (clip == null || !isSound) return;
        effectAudioSource.PlayOneShot(clip);
    }

    /// <summary>
    /// Play background music (loop optional).
    /// </summary>
    private void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (clip == null || !isMusic) return;

        backgroundAudioSource.clip = clip;
        backgroundAudioSource.loop = loop;
        backgroundAudioSource.Play();
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

        if (!isMusic)
            backgroundAudioSource.Stop();
        else if (isMusic)
            PlayMusic(backgroundSound, true);
    }

    #endregion

    #region Helper Wrappers for Common Sounds
    public void PlayLevelFail() => PlaySfx(levelFailSound);
    public void PlayLevelDone() => PlaySfx(levelDoneSound);
    public void PlayConfetti() => PlaySfx(levelCompleteUIConfettiSound);
    public void StopConfetti() => effectAudioSource.Stop();
    public void PlayButton() => PlaySfx(buttonClickSound);
    #endregion
}
