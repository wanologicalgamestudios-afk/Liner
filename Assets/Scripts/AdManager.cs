using System;
using UnityEngine;
using GoogleMobileAds.Api;

public class AdManager : MonoBehaviour
{
    [SerializeField]
    private bool AreTheseTestAds;

    public static AdManager Instance;

    [Header("Android Test Unit Ids")]
    [SerializeField]
    private string androidTestBannerAdUnitId = "ca-app-pub-3940256099942544/6300978111";
    [SerializeField]
     private string androidTestInterstitialAdUnitId = "ca-app-pub-3940256099942544/1033173712";
    [SerializeField]
    private string androidTestRewardedAdUnitId = "ca-app-pub-3940256099942544/5224354917";
     [SerializeField]
     private string androidTestRewardedInterstitialAdUnitId = "ca-app-pub-3940256099942544/5354046379";
     [SerializeField]
     private string androidTestAppOpenAdUnitId = "ca-app-pub-3940256099942544/3419835294";

    [Header("iOS Test Unit Ids")]
      [SerializeField]
      private string iOSTestBannerAdUnitId = "ca-app-pub-3940256099942544/2934735716";
      [SerializeField]
      private string iOSTestInterstitialAdUnitId = "ca-app-pub-3940256099942544/4411468910";
    [SerializeField]
    private string iOSTestRewardedAdUnitId = "ca-app-pub-3940256099942544/1712485313";
       [SerializeField]
      private string iOSTestRewardedInterstitialAdUnitId = "ca-app-pub-3940256099942544/6978759866";
      [SerializeField]
      private string iOSTestAppOpenAdUnitId = "ca-app-pub-3940256099942544/5662855259";

    [Header("Android Unit Ids")]
      [SerializeField]
      private string androidBannerAdUnitId = "ca-app-pub-3940256099942544/2934735716";
      [SerializeField]
      private string androidInterstitialAdUnitId = "ca-app-pub-3940256099942544/4411468910";
    [SerializeField]
    private string androidRewardedAdUnitId = "ca-app-pub-3940256099942544/1712485313";
       [SerializeField]
      private string androidRewardedInterstitialAdUnitId = "ca-app-pub-3940256099942544/6978759866";
      [SerializeField]
      private string androidAppOpenAdUnitId = "ca-app-pub-3940256099942544/5662855259";

    [Header("iOS Unit Ids")]
    [SerializeField]
    private string iOSBannerAdUnitId = "ca-app-pub-3940256099942544/2934735716";
    [SerializeField]
    private string iOSInterstitialAdUnitId = "ca-app-pub-3940256099942544/4411468910";
    [SerializeField]
    private string iOSRewardedAdUnitId = "ca-app-pub-3940256099942544/1712485313";
    [SerializeField]
    private string iOSRewardedInterstitialAdUnitId = "ca-app-pub-3940256099942544/6978759866";
    [SerializeField]
    private string iOSAppOpenAdUnitId = "ca-app-pub-3940256099942544/5662855259";


    private string bannerAdUnitId;
    private string interstitialAdUnitId;
    private string rewardedAdUnitId;
    private string rewardedInterstitialAdUnitId;
    private string appOpenAdUnitId;


    private BannerView bannerView;
    private InterstitialAd interstitialAd;
    private RewardedAd rewardedAd;
    private RewardedInterstitialAd rewardedInterstitialAd;
    private AppOpenAd appOpenAd;


    Action OnRewaredVideoRewardGiven;
    Action OnRewaredVideoNotReady;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SetIds();
        Initialization();
    }

    private void SetIds()
    {
        if (AreTheseTestAds)
        {
            #if UNITY_ANDROID
            bannerAdUnitId = androidTestBannerAdUnitId;
            interstitialAdUnitId = androidTestInterstitialAdUnitId;
            rewardedAdUnitId = androidTestRewardedAdUnitId;
            rewardedInterstitialAdUnitId = androidTestRewardedInterstitialAdUnitId;
            appOpenAdUnitId = androidTestAppOpenAdUnitId;

            #elif UNITY_IPHONE
            bannerAdUnitId = iOSTestBannerAdUnitId;
            interstitialAdUnitId = iOSTestInterstitialAdUnitId;
            rewardedAdUnitId = iOSTestRewardedAdUnitId;
            rewardedInterstitialAdUnitId = iOSTestRewardedInterstitialAdUnitId;
            appOpenAdUnitId = iOSTestAppOpenAdUnitId;

            #else
          //  adUnitID = "unused";
            #endif
        }
        else 
        {
            #if UNITY_ANDROID
            bannerAdUnitId = androidBannerAdUnitId;
            interstitialAdUnitId = androidInterstitialAdUnitId;
            rewardedAdUnitId = androidRewardedAdUnitId;
            rewardedInterstitialAdUnitId = androidRewardedInterstitialAdUnitId;
            appOpenAdUnitId = androidAppOpenAdUnitId;

            #elif UNITY_IPHONE
            bannerAdUnitId = iOSBannerAdUnitId;
            interstitialAdUnitId = iOSInterstitialAdUnitId;
            rewardedAdUnitId = iOSRewardedAdUnitId;
            rewardedInterstitialAdUnitId = iOSRewardedInterstitialAdUnitId;
            appOpenAdUnitId = iOSAppOpenAdUnitId;

            #else
          //  adUnitID = "unused";
            #endif
        }
    }

    void Initialization()
    {
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(HandleInitCompleteAction);

       // LoadBannerAd();
      //  LoadInterstitialAd();
        LoadRewardedAd();
      //  LoadRewardedInterstitialAd();
      //  LoadAppOpenAd();
        //RequestBanner();
        Debug.Log("intiallize");
    }

    #region Banner
    public void LoadBannerAd()
    {
        if (bannerView != null)
        {
            bannerView.Destroy();
        }

        bannerView = new BannerView(bannerAdUnitId, AdSize.Banner, AdPosition.Bottom);
        AdRequest request = new AdRequest();
        bannerView.LoadAd(request);
    }
    #endregion

    #region Interstitial
    public void LoadInterstitialAd()
    {
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
        }

        var adRequest = new AdRequest();

        InterstitialAd.Load(interstitialAdUnitId, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.LogError("Failed to load interstitial: " + error);
                    return;
                }
                interstitialAd = ad;
            });
    }

    public void ShowInterstitialAd()
    {
        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            interstitialAd.Show();
        }
        else
        {
            Debug.Log("Interstitial not ready.");
        }
    }
    #endregion

    #region Rewarded
    private void LoadRewardedAd()
    {
        CancelInvoke(nameof(LoadRewardedAd));

        if (rewardedAd != null)
        {
            DestroyAd();
        }

        var adRequest = new AdRequest();


        RewardedAd.Load(rewardedAdUnitId, adRequest, (RewardedAd ad, LoadAdError error) =>
        {
            // If the operation failed with a reason.
            if (error != null)
            {
                Debug.LogError("Rewarded ad failed to load an ad with error : " + error);
                Invoke(nameof(rewardedAd), 10f);
                return;
            }
            // If the operation failed for unknown reasons.
            // This is an unexpected error, please report this bug if it happens.
            if (ad == null)
            {
                Debug.LogError("Unexpected error: Rewarded load event fired with null ad and null error.");
                Invoke(nameof(rewardedAd), 10f);
                return;
            }

            // The operation completed successfully.
            Debug.Log("Rewarded ad loaded with response : " + ad.GetResponseInfo());
            rewardedAd = ad;

            // Register to ad events to extend functionality.
            RegisterEventHandlers(ad);
        });
    }

    private void ShowRewardedAd()
    {
        Debug.Log("here we will show ad");

        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            Debug.Log("Showing rewarded ad.");
            rewardedAd.Show((Reward reward) =>
            {
                Debug.Log(String.Format("Rewarded ad granted a reward: {0} {1}",
                                        reward.Amount,
                                        reward.Type));
                HandleOnAdCloseRBVA();
            });
        }
        else
        {
            Debug.LogError("Rewarded ad is not ready yet.");
            
            OnRewaredVideoNotReady();
            LoadRewardedAd();
        }

        // Inform the UI that the ad is not ready.
        //AdLoadedStatus?.SetActive(false);
    }

    public void ShowRewardedAd(Action _onRewardGiven = null, Action _onAdNotReady = null)
    {
        OnRewaredVideoRewardGiven = _onRewardGiven;
        OnRewaredVideoNotReady = _onAdNotReady;
        ShowRewardedAd();
    }

    private void DestroyAd()
    {
        if (rewardedAd != null)
        {
            Debug.Log("Destroying rewarded ad.");
            rewardedAd.Destroy();
            rewardedAd = null;
        }

        // Inform the UI that the ad is not ready.
        //AdLoadedStatus?.SetActive(false);
    }

    public void HandleOnAdLoadedRBVA(object sender, EventArgs args)
    {
        Debug.Log("HandleOnAdLoadedRBVA");
    }
    public void HandleOnAdFailedToLoadRBVA(object sender, EventArgs args)
    {
        Debug.Log("HandleOnAdFailedToLoadRBVA");
    }
    public void HandleOnAdOppingRBVA(object sender, EventArgs args)
    {
        Debug.Log("HandleOnAdOppingRBVA");
    }
    public void HandleOnAdStartedRBVA(object sender, EventArgs args)
    {
        Debug.Log("HandleOnAdStartedRBVA");
    }
    public void HandleOnAdCloseRBVA()
    {
        Debug.Log("HandleOnAdCloseRBVA");
        OnRewaredVideoRewardGiven();
        LoadRewardedAd();
    }
    public void HandleOnAdRewardedRBVA(object sender, EventArgs args)
    {
        Debug.Log("HandleOnAdRewardedRBVA");
    }
    public void HandleOnAdLeavingApplicationRBVA(object sender, EventArgs args)
    {
        Debug.Log("HandleOnAdLeavingApplicationRBVA");
    }
    public void HandleOnAdCompeletedRBVA(object sender, EventArgs args)
    {
        Debug.Log("HandleOnAdCompeletedRBVA");
    }
    private void HandleInitCompleteAction(InitializationStatus initstatus)
    {
        MonoBehaviour.print("Initialization complete");
        //this.nativeAdLoaded = false;
        //this.RequestNativeAd();
    }
    private void RegisterEventHandlers(RewardedAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Rewarded ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Rewarded ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            Debug.Log("Rewarded ad was clicked.");
        };
        // Raised when the ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Rewarded ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded ad full screen content closed.");
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded ad failed to open full screen content with error : "
                + error);
        };
    }
    #endregion

    #region Rewarded Interstitial
    public void LoadRewardedInterstitialAd()
    {
        var adRequest = new AdRequest();
        RewardedInterstitialAd.Load(rewardedInterstitialAdUnitId, adRequest,
            (RewardedInterstitialAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.LogError("Failed to load rewarded interstitial: " + error);
                    return;
                }
                rewardedInterstitialAd = ad;
            });
    }

    public void ShowRewardedInterstitialAd(Action rewardCallback = null)
    {
        if (rewardedInterstitialAd != null && rewardedInterstitialAd.CanShowAd())
        {
            rewardedInterstitialAd.Show((Reward reward) =>
            {
                Debug.Log($"User rewarded: {reward.Type}, amount: {reward.Amount}");
                rewardCallback?.Invoke();
            });
        }
        else
        {
            Debug.Log("Rewarded interstitial not ready.");
        }
    }
    #endregion

    #region App Open
    //public void LoadAppOpenAd()
    //{
    //    var adRequest = new AdRequest.Builder().Build();
    //    AppOpenAd.Load(appOpenAdUnitId, ScreenOrientation.Portrait, adRequest,
    //        (AppOpenAd ad, LoadAdError error) =>
    //        {
    //            if (error != null || ad == null)
    //            {
    //                Debug.LogError("Failed to load app open: " + error);
    //                return;
    //            }
    //            appOpenAd = ad;
    //        });
    //}

    //public void ShowAppOpenAd()
    //{
    //    if (appOpenAd != null && appOpenAd.CanShowAd())
    //    {
    //        appOpenAd.Show();
    //    }
    //    else
    //    {
    //        Debug.Log("App open ad not ready.");
    //    }
    //}
    #endregion
}
