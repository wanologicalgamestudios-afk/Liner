
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    private void Awake()
    {
        SetPlayerPrefsOnVeryFirstRun();
    }
    private void SetPlayerPrefsOnVeryFirstRun()
    {
        if (PlayerPrefs.GetInt("isFirstRun") == 0)
        {
            // Settings
            PlayerPrefs.SetInt("isSoundOff", 0);
            PlayerPrefs.SetInt("currentLevel", 1);
            PlayerPrefs.SetInt("isVibrationOff", 0);
            //////////////////////
            PlayerPrefs.SetInt("isFirstRun", 1);
        }
    }

    public bool IsInternetAvailable()
    {
        bool isInternetAvailable;
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            isInternetAvailable = false;
        }
        else
        {
            isInternetAvailable = true;
        }

        return isInternetAvailable;
    }
    public string AuthenticationToken
    {
        get { return PlayerPrefs.GetString("authToken"); }
        set { PlayerPrefs.SetString("authToken", value); }
    }
    public int CurerntLevel
    {
        get { return PlayerPrefs.GetInt("currentLevel"); }
        set { PlayerPrefs.SetInt("currentLevel", value); }
    }
    public void GiveAndSaveDailyReward()
    {
        //this.signupLoginMetadata = new SignupLoginMetadata();
        //signupLoginMetadata.authenticationToken = AuthToken;

        //if (GetRewadDay() == 1)
        //{
        //    signupLoginMetadata.totalCoins = (playerProfileMetaData.user_profile_data.totalCoins += 100).ToString();
        //    //SaveCoins(100);
        //}
        //else if (GetRewadDay() == 2)
        //{
        //    signupLoginMetadata.totalCoins = (playerProfileMetaData.user_profile_data.totalCoins += 150).ToString();
        //    signupLoginMetadata.totalDiamonds = (playerProfileMetaData.user_profile_data.totalDiamonds += 2).ToString();

        //    //SaveCoins(150);
        //    //SaveDiamonds(2);
        //}
        //else if (GetRewadDay() == 3)
        //{
        //    signupLoginMetadata.totalCoins = (playerProfileMetaData.user_profile_data.totalCoins += 200).ToString();
        //    signupLoginMetadata.totalDiamonds = (playerProfileMetaData.user_profile_data.totalDiamonds += 6).ToString();

        //    //SaveCoins(200);
        //    //SaveDiamonds(6);
        //}

        //PlayerPrefs.SetInt("rewardDay", PlayerPrefs.GetInt("rewardDay") + 1);
        //if (PlayerPrefs.GetInt("rewardDay", 1) > 3)
        //{
        //    PlayerPrefs.SetInt("rewardDay", 1);
        //}
        //PlayerPrefs.SetString("savedDateTime", DateTime.Today.AddDays(1).ToBinary().ToString());

        //Authentication(signupLoginMetadata, "/update-ludo-user", HTTPMethods.Post, CallbackResourcesUpdate);
    }
    public void DeductDiamonds(int amount)
    {
        //if (!isGuestUser)
        //{
        //    this.signupLoginMetadata = new SignupLoginMetadata();
        //    signupLoginMetadata.authenticationToken = AuthToken;

        //    signupLoginMetadata.totalDiamonds = (playerProfileMetaData.user_profile_data.totalDiamonds -= amount).ToString();
        //    Authentication(signupLoginMetadata, "/update-ludo-user", HTTPMethods.Post, CallbackResourcesUpdate);
        //}
        //else
        //{
        //    GuestUserTotalDiamonds -= amount;
        //}
    }
    public void AddDiamonds(int amount)
    {
        //if (!isGuestUser)
        //{
        //    this.signupLoginMetadata = new SignupLoginMetadata();
        //    signupLoginMetadata.authenticationToken = AuthToken;

        //    signupLoginMetadata.totalDiamonds = (playerProfileMetaData.user_profile_data.totalDiamonds += amount).ToString();
        //    Authentication(signupLoginMetadata, "/update-ludo-user", HTTPMethods.Post, OnProfileUpdated);
        //}
        //else
        //{
        //    GuestUserTotalDiamonds += amount;
        //    OnProfileUpdated?.Invoke();
        //}
    }
    public void AddCoins(int amount)
    {
        //if (!isGuestUser)
        //{
        //    this.signupLoginMetadata = new SignupLoginMetadata();
        //    signupLoginMetadata.authenticationToken = AuthToken;

        //    signupLoginMetadata.totalCoins = (playerProfileMetaData.user_profile_data.totalCoins += amount).ToString();
        //    Authentication(signupLoginMetadata, "/update-ludo-user", HTTPMethods.Post, OnProfileUpdated);
        //}
        //else
        //{
        //    GuestUserTotalCoins += amount;
        //    OnProfileUpdated?.Invoke();
        //}
    }
}
