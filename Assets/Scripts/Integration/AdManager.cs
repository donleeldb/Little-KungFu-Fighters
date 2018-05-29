using UnityEngine;
using System.Collections;

public class AdManager : MonoBehaviour {

	[Tooltip("How much coins would player achieve for watching video")] public float adReward;

	string appKey = "";

	MenuManager mm;

	public GameObject EarnButton;

	// Use this for initialization
	void Start () {

		mm = gameObject.findMM ();

		if (rewardedVideoIsReady ()) {
			EarnButton.SetActive (true);
		}

	}

	public void showInterstitial () {
		
	}

	public void showBanner () {
		
	}

	public void showRewardedVideo () {
		
	}

	public void showNonSkippableVideo () {
		
	}

	public bool rewardedVideoIsReady ()
	{
		
		return false;
	}

	public bool nonSkippableVideoIsReady () {
		
		return false;
	}


	// callbacks

	public void onInterstitialLoaded() { 
		
	}
	public void onInterstitialFailedToLoad() { 
		
	}
	public void onInterstitialShown() { 
		
	}
	public void onInterstitialClosed() { 

	}
	public void onInterstitialClicked() { 
		
	}


	public void onSkippableVideoLoaded() { 
		
	}
	public void onSkippableVideoFailedToLoad() { 

	}
	public void onSkippableVideoShown() { 

	}
	public void onSkippableVideoFinished() { 

	}
	public void onSkippableVideoClosed() { 

	}


	public void onNonSkippableVideoLoaded() { 
		
	}
	public void onNonSkippableVideoFailedToLoad() { 

	}
	public void onNonSkippableVideoShown() { 

	}
	public void onNonSkippableVideoFinished() { 

	}
	public void onNonSkippableVideoClosed() { 

	}


	public void onRewardedVideoLoaded() { 
		mm.activateEarnButtons (true);
		mm.refreshEarnOptions ();
	}
	public void onRewardedVideoFailedToLoad() { 
		
	}
	public void onRewardedVideoShown() { 

	}
	public void onRewardedVideoClosed() { 

	}
	public void onRewardedVideoFinished(int amount, string name) { 
		mm.addCoins (adReward);
		mm.hideAdOption ();
		Invoke ("refresh", Random.Range (15, 25));
	}

	public void onBannerLoaded() { 
		
	}
	public void onBannerFailedToLoad() {
		
	}
	public void onBannerShown() { 
		
	}
	public void onBannerClicked() { 
		
	}

} 
