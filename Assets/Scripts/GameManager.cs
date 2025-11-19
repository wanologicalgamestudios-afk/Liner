using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Testing Settings")]
    [Tooltip("Enable this if you want to play a specific level for testing.")]
    public bool overrideLevelForTesting = false;

    [Tooltip("The level to load if override is enabled.")]
    public int testLevel = 1;

    private void Awake()
    {
        SetPlayerPrefsOnVeryFirstRun();

        // Allow manual override for testing
        if (overrideLevelForTesting)
        {
            Debug.Log($"[GameManager] Overriding current level for testing: Level {testLevel}");
            CurerntLevel = testLevel;
        }
    }

    private void SetPlayerPrefsOnVeryFirstRun()
    {
        if (PlayerPrefs.GetInt("isFirstRun") == 0)
        {
            // Default settings
            PlayerPrefs.SetInt("isFirstTimeHowToPlayShown", 0);
            PlayerPrefs.SetInt("isSoundOff", 0);
            PlayerPrefs.SetInt("currentLevel", 1);
            PlayerPrefs.SetInt("isVibrationOff", 0);
            PlayerPrefs.SetInt("isMusicOff", 0);
            PlayerPrefs.SetString("lastLevelCompletionTime", string.Empty);
            PlayerPrefs.SetInt("isFirstRun", 1);
        }
    }

    public bool IsInternetAvailable()
    {
        return Application.internetReachability != NetworkReachability.NotReachable;
    }

    public string AuthenticationToken
    {
        get => PlayerPrefs.GetString("authToken");
        set => PlayerPrefs.SetString("authToken", value);
    }

    public int CurerntLevel
    {
        get => PlayerPrefs.GetInt("currentLevel");
        set => PlayerPrefs.SetInt("currentLevel", value);
    }

    public int IsFirstTimeHowToPlayShown
    {
        get => PlayerPrefs.GetInt("isFirstTimeHowToPlayShown", 0);
        set => PlayerPrefs.SetInt("isFirstTimeHowToPlayShown", value);
    }

    public int IsVibrationOff
    {
        get => PlayerPrefs.GetInt("isVibrationOff", 0);
        set => PlayerPrefs.SetInt("isVibrationOff", value);
    }

    public string LastLevelCompletionTime
    {
        get => PlayerPrefs.GetString("lastLevelCompletionTime", string.Empty);
        set => PlayerPrefs.SetString("lastLevelCompletionTime", value);
    }

    public int IsSoundOff
    {
        get => PlayerPrefs.GetInt("isSoundOff", 0);
        set
        {
            PlayerPrefs.SetInt("isSoundOff", value);
            UIManager.GetInstance().SoundManager.ToggleSound(value == 0);
        }
    }

    public int IsMusicOff
    {
        get => PlayerPrefs.GetInt("isMusicOff", 0);
        set
        {
            PlayerPrefs.SetInt("isMusicOff", value);
            UIManager.GetInstance().SoundManager.ToggleMusic(value == 0);
        }
    }
}
