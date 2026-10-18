using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class AdsRewarded : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    public string gameId = "Your-Google-ID";
    public string mySurfacingId = "Rewarded_Android";
    public bool testMode = true;

    public Button[] buttons;
    public Animation[] loadingAnimations;

    void Start()
    {
        if (buttons != null)
        {
            foreach (Button btn in buttons)
            {
                if (btn != null) btn.interactable = false;
            }
        }
        if (loadingAnimations != null)
        {
            foreach (Animation animation in loadingAnimations)
            {
                if (animation != null) animation.Play();
            }
        }
        Advertisement.Initialize(gameId, testMode);
        Advertisement.Load(mySurfacingId, this);
    }

    public void ShowRewardedVideo()
    {
        Advertisement.Show(mySurfacingId, this);
    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
        if (buttons != null)
        {
            foreach (Button btn in buttons)
            {
                if (btn != null) btn.interactable = true;
            }
        }
        if (loadingAnimations != null)
        {
            foreach (Animation animation in loadingAnimations)
            {
                if (animation != null) animation.gameObject.SetActive(false);
            }
        }
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.LogWarning($"Unity Ads failed to load: {message}");
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        if (PopUpManagement.instance != null)
        {
            PopUpManagement.instance.Show("Something went wrong. Please try again later.", 3);
        }
    }

    public void OnUnityAdsShowStart(string placementId) { }
    public void OnUnityAdsShowClick(string placementId) { }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        if (showCompletionState == UnityAdsShowCompletionState.COMPLETED)
        {
            if (PopUpManagement.instance != null)
            {
                PopUpManagement.instance.Show("Thank you!", 3);
            }
        }
    }
}