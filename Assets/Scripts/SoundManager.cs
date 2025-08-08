using UnityEngine;
using System.Collections;
/*
 * This is SoundManager
 * In other script, you just need to call SoundManager.PlaySfx(AudioClip) to play the sound
*/
public class SoundManager : MonoBehaviour
{
	public static SoundManager Instance;
	public AudioClip beginSoundInMainMenu;
	[Tooltip("Play music clip when start")]
	public AudioClip musicsGame;
	public AudioClip musicLevel;
	[Range(0, 1)]
	public float musicsGameVolume = 0.5f;

	[Tooltip("Place the sound in this to call it in another script by: SoundManager.PlaySfx(soundname);")]
	public AudioClip soundClick;

	[Header("Game State")]
	public AudioClip soundAchieviedRequiredNumber;
	public AudioClip soundFail;
	public AudioClip soundPause;
	public AudioClip soundShowStagePanel;
	public AudioClip soundDiceRoll;

	[Header("ADD POWERUP")]
	public AudioClip soundAddPowerUp;

	[Header("Shop")]
	public AudioClip soundPurchased;

	[Header("Level Cleared")]
	public AudioClip soundVictory;

	private AudioSource musicAudio;
	private AudioSource soundFx;

	public static bool isSound = true;
	public static bool isMusic = true;

	//float SavedMusicVolume;
	//float SavedSfxVolume;

	//public AudioClip switchPlayerSound;

	//public AudioClip soundCheckpoint;
	public void PauseMusic(bool isPause)
	{
		if (isPause)
			Instance.musicAudio.mute = true;
		//			Instance.musicAudio.Pause ();
		else
			Instance.musicAudio.mute = false;
		//			Instance.musicAudio.UnPause ();
	}
	//GET and SET
	public static float MusicVolume
	{

		set { Instance.musicAudio.volume = value; SavedMusicVolume = value; }
		get { return Instance.musicAudio.volume; }
	}
	public static float SoundVolume
	{
		set { Instance.soundFx.volume = value; SavedSfxVolume = value; }
		get { return Instance.soundFx.volume; }
	}
	// Use this for initialization
	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			//DontDestroyOnLoad(gameObject);
		}
		else
		{
			//Destroy(gameObject);
		}
		musicAudio = gameObject.AddComponent<AudioSource>();
		musicAudio.loop = true;
		musicAudio.volume = SavedMusicVolume;
		soundFx = gameObject.AddComponent<AudioSource>();
		soundFx.volume = SavedSfxVolume;

	}
	void Start()
	{
        //Check auido and sound

        if (SavedMusicVolume > 0f)
            isMusic = true;
        else
            isMusic = false;

        if (SavedSfxVolume > 0f)
            isSound = true;
        else
            isSound = false;

        //Check auido and sound
        PlayMusic(musicsGame, MusicVolume);
    }

	public static void Click()
	{
		PlaySfx(Instance.soundClick);
	}

	public void ClickBut()
	{
		PlaySfx(soundClick);
	}

	public static void PlaySfx(AudioClip clip)
	{
		if (Instance != null)
		{
			Instance.PlaySound(clip, Instance.soundFx);
		}
	}

	public static void PlaySfx(AudioClip clip, float volume)
	{
		if (Instance != null)
			Instance.PlaySound(clip, Instance.soundFx, volume);
	}

	public static void PlaySfx(AudioClip[] clips)
	{
		if (Instance != null && clips.Length > 0)
			Instance.PlaySound(clips[Random.Range(0, clips.Length)], Instance.soundFx);
	}

	public static void PlaySfx(AudioClip[] clips, float volume)
	{
		if (Instance != null && clips.Length > 0)
			Instance.PlaySound(clips[Random.Range(0, clips.Length)], Instance.soundFx, volume);
	}

	public static void PlayMusic(AudioClip clip)
	{
		Instance.PlaySound(clip, Instance.musicAudio);
	}

	public static void PlayMusic(AudioClip clip, float volume)
	{
		Instance.PlaySound(clip, Instance.musicAudio, volume);
	}

	public static float SavedMusicVolume
	{
		get { return PlayerPrefs.GetFloat("Music Volume", 0.5f); }
		set
		{
			PlayerPrefs.SetFloat("Music Volume", value);
		}
	}
	public static float SavedSfxVolume
	{
		get { return PlayerPrefs.GetFloat("Sfx Volume", 1f); }
		set
		{
			PlayerPrefs.SetFloat("Sfx Volume", value);
		}
	}

	private void PlaySound(AudioClip clip, AudioSource audioOut)
	{
		if (clip == null)
		{
			//			Debug.Log ("There are no audio file to play", gameObject);
			return;
		}

		if (Instance == null)
			return;

		if (audioOut == musicAudio)
		{
			audioOut.clip = clip;
			audioOut.Play();
		}
		else
			audioOut.PlayOneShot(clip, SoundVolume);
	}

	private void PlaySound(AudioClip clip, AudioSource audioOut, float volume)
	{
		if (clip == null)
		{
			//			Debug.Log ("There are no audio file to play", gameObject);
			return;
		}

		if (audioOut == musicAudio)
		{
			//if (!GlobalValue.isMusic) return;
			//audioOut.volume = GlobalValue.isMusic ? volume : 0;
			audioOut.clip = clip;
			audioOut.Play();
		}
		else
		{
			//if (!GlobalValue.isSound) return;
			audioOut.PlayOneShot(clip, SoundVolume * volume);
		}
	}
}
