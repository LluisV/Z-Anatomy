using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements; //Assuming you imported the Advertisements from the "Package Manager"
using UnityEngine.UI;
public class AdsRewarded : MonoBehaviour, IUnityAdsListener, IUnityAdsLoadListener
{
    public string gameId = "Your-Google-ID";
    public string mySurfacingId = "Rewarded_Android";
    public bool testMode = true; //Leave this as True UNTIL you release your game!!!

    public Button[] buttons;
    public Animation[] loadingAnimations;
    void Start()
    {
        foreach (Button btn in buttons)
        {
            btn.interactable = false;
        }
        foreach (Animation animation in loadingAnimations)
        {
            animation.Play();
        }
        Advertisement.AddListener(this);    //Used to check if Player COMPLETED the ad
        Advertisement.Initialize(gameId, testMode, true);     // Initialize the Ads listener and service:
        Advertisement.Load(mySurfacingId, this);
    }

    public void ShowRewardedVideo() //Shows The add when this method is called - 
    {   // Check if UnityAds ready before calling Show method:
        if (Advertisement.IsReady(mySurfacingId)) 
            Advertisement.Show(mySurfacingId);
        else
        {
            PopUpManagement.instance.Show("The video is not ready at the moment. Please try again later.", 3);
        }
    }
    public void OnUnityAdsDidFinish(string surfacingId, ShowResult showResult) // Implement IUnityAdsListener interface methods:
    {
        if (showResult == ShowResult.Finished)
        {
            // print("The Ad finished!!!");
            PopUpManagement.instance.Show("Thank you!", 3);
        }
        else if (showResult == ShowResult.Skipped)
        {
          //  print("The Ad was SKIPPED you Cheater...");
        }
        else if (showResult == ShowResult.Failed)
        {
            PopUpManagement.instance.Show("Something went wrong. Please try again later.", 3);
         //   print("The Ad was interrupted or Failed.");
        }

    }
    public void OnUnityAdsReady(string surfacingId) //Activates when ADD is ready
    {// If the ready Ad Unit or legacy Placement is rewarded, show the ad:        
        if (surfacingId == mySurfacingId)
        {
            foreach (Button btn in buttons)
            {
                btn.interactable = true;
            }
            foreach (Animation animation in loadingAnimations)
            {
                animation.gameObject.SetActive(false);
            }
        }
    }
    public void OnUnityAdsDidError(string message) // Log the error.
    {
        PopUpManagement.instance.Show("Something went wrong. Please try again later.", 3);
        //print("Something's wrong, it's... the Ad's not working!!!");
    }
    public void OnUnityAdsDidStart(string surfacingId) // Optional actions to take when the end-users triggers an ad.
    {
        //print("this is extra");
    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
        foreach (Button btn in buttons)
        {
            btn.interactable = true;
        }
        foreach (Animation animation in loadingAnimations)
        {
            animation.gameObject.SetActive(false);
        }
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
       // throw new System.NotImplementedException();
    }
}