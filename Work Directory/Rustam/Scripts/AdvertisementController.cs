using System.Collections;
using System.Collections.Generic;
using EasyMobile;
using UnityEngine;

public class AdvertisementController : MonoBehaviour
{
    public static AdvertisementController Instance;

    private void Awake()
    {
        if(Instance != null)
            Destroy(this);
        Instance = this;
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        if (!RuntimeManager.IsInitialized())
            RuntimeManager.Init();

        Advertising.AdColonyClient.Init();

        Advertising.UnityAdsClient.LoadInterstitialAd();
        
        Advertising.UnityAdsClient.LoadRewardedAd();
        Advertising.AdColonyClient.LoadInterstitialAd();
        Advertising.AdColonyClient.LoadRewardedAd();
        Advertising.RewardedAdCompleted += AdvertisingOnRewardedAdCompleted;

    }



    private void AdvertisingOnRewardedAdCompleted(RewardedAdNetwork arg1, AdPlacement arg2)
    {
        const int rewardedAdTimeBonus = 600;
        AvailabilityVar.Instance.AddTimeInSeconds(rewardedAdTimeBonus);
    }


    public float lastInterstitialShowTime = 0f;
    
    public void ShowInterstitialAd()
    {
        
        
        if (Time.realtimeSinceStartup - lastInterstitialShowTime > 600f)
        {
            //AdColony
        
            if (Advertising.IsInterstitialAdReady(InterstitialAdNetwork.AdColony, AdLocation.Default))
            {
                Advertising.ShowInterstitialAd(InterstitialAdNetwork.AdColony, AdLocation.Default);
            }

            //UnityAds
        
            else if (Advertising.IsInterstitialAdReady(InterstitialAdNetwork.UnityAds, AdLocation.Achievements))
            {
                Advertising.ShowInterstitialAd(InterstitialAdNetwork.UnityAds, AdLocation.Achievements);
            }
            
            else
            {
                Advertising.AdColonyClient.LoadInterstitialAd();
                Advertising.UnityAdsClient.LoadInterstitialAd();
            }
            
            lastInterstitialShowTime = Time.realtimeSinceStartup;
        }
    }

    public void ShowRewardedAd()
    {
        //AdColony
        
        if (Advertising.IsRewardedAdReady(RewardedAdNetwork.AdColony, AdLocation.Default))
        {
            Advertising.ShowRewardedAd(RewardedAdNetwork.AdColony, AdLocation.Default);
            //Advertising.AdColonyClient.LoadRewardedAd();
        }
        
        
        //UnityAds
        
        else if (Advertising.IsRewardedAdReady(RewardedAdNetwork.UnityAds, AdLocation.Achievements))
        {
            Advertising.ShowRewardedAd(RewardedAdNetwork.UnityAds, AdLocation.Achievements);
            //Advertising.UnityAdsClient.LoadRewardedAd();
        }
        else
        {
            Advertising.AdColonyClient.LoadRewardedAd();
            Advertising.UnityAdsClient.LoadRewardedAd();
            Debug.Log("Rewarded ad not ready yet");
          //  return false;
        }
        
        //return true;
    }
}
