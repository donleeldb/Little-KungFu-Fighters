using UnityEngine;
using System.Collections;
using Facebook.Unity;
using System;

public class SocialMediaManager : MonoBehaviour {

	[Tooltip ("put your google play link here")] public string GooglePlayLink;
	[Tooltip ("put your AppStore link here")] public string AppStoreLink;
	[Tooltip ("put your game's photo link here")] public string photoURL;
	[Tooltip ("put your facebook link here")] public string facebookURL;
	[Tooltip ("put your twitter link here")] public string twitterFollowURL;
	[Tooltip ("put your tumblr link here")] public string tumblrURL;
	[Tooltip ("put your post title here")] public string title;
	[Tooltip ("put your facebook app ID here. (You have to register an app on a facebook developer page to get this ID)")] public string FacebookID;
	[Tooltip ("Time delay between facebook posts")] public int facebookShareHoursInterval;
	[Tooltip ("Time delay between twitter posts")] public int twitterShareHoursInterval;
	[Tooltip ("How much coins would player achieve for facebook share")] public float facebookShareReward;
	[Tooltip ("How much coins would player achieve for twitter share")] public float twitterShareReward;
	[Tooltip ("How much coins would player achieve for twitter follow")] public float twitterFollowReward;
	[Tooltip ("put your twitter hashtags here")] public string[] hashtags;

	MenuManager mm; // menu manager refference

	[HideInInspector]public bool facebookLinkShared;
	[HideInInspector]public bool twitterLinkPosted;

	void Start () {
		mm = gameObject.findMM ();
	}

	void Awake ()
	{
		if (!FB.IsInitialized) { // initializing Facebook sdk
			// Initialize the Facebook SDK
			FB.Init(InitCallback, OnHideUnity);
		} else {
			// Already initialized, signal an app activation App Event
			FB.ActivateApp();
		}
	}

	public bool earnOptions () { // if there are social earn options

		mm = gameObject.findMM ();
		DateTime now = System.DateTime.Now;

		if (now.CompareTo (getTime ("facebookShare")) >= 1) {
			facebookLinkShared = false;
		} else {
			facebookLinkShared = true;
		}

		if (now.CompareTo (getTime ("twitterShare")) >= 1) {
			twitterLinkPosted = false;
		} else {
			twitterLinkPosted = true;
		}

		bool followedOnTwitter = PlayerPrefs.GetInt ("twitterFollow", 0) == 1? true : false;

		if (!facebookLinkShared || !twitterLinkPosted || !followedOnTwitter) {
			return true;
		} else {
			return false;
		}

	}

	public void saveTime (String variable, float plusHours) { // save last post time
		DateTime giftTime = System.DateTime.Now.AddHours (plusHours);
		PlayerPrefs.SetInt (variable + "_day", giftTime.Day);
		PlayerPrefs.SetInt (variable + "_month", giftTime.Month);
		PlayerPrefs.SetInt (variable + "_year", giftTime.Year);
		PlayerPrefs.SetInt (variable + "_hour", giftTime.Hour);
		PlayerPrefs.SetInt (variable + "_minute", giftTime.Minute);
		PlayerPrefs.SetInt (variable + "_second", giftTime.Second);
	}

	DateTime getTime (String variable) // get last post time
	{
		int day = PlayerPrefs.GetInt (variable + "_day");
		int month = PlayerPrefs.GetInt (variable + "_month");
		int year = PlayerPrefs.GetInt (variable + "_year");
		int hour = PlayerPrefs.GetInt (variable + "_hour");
		int minute = PlayerPrefs.GetInt (variable + "_minute");
		int second = PlayerPrefs.GetInt (variable + "_second");
		if (day < 1 || month < 1 || year < 1) 
		{
			return new DateTime (1, 1, 1);
		} else {
			return new DateTime (year, month, day,hour,minute,second);
		}
	}

	private void InitCallback ()
	{
		if (FB.IsInitialized) {
			// Signal an app activation App Event
			FB.ActivateApp();
			// Continue with Facebook SDK
			// ...
		} else {
			
		}
	}

	private void OnHideUnity (bool isGameShown)
	{
		if (!isGameShown) {
			// Pause the game - we will need to hide
			Time.timeScale = 0;
		} else {
			// Resume the game - we're getting focus again
			Time.timeScale = 1;
		}
	}

	public void shareLinkOnFacebook () {
		FB.ShareLink (contentURL: new System.Uri (GooglePlayLink),
			contentTitle: title,
			contentDescription: "Right now I'm playing an awesome beat'em'up game",
			photoURL: new System.Uri (photoURL),
			callback: ShareCallback);
	}

	private void ShareCallback (IShareResult result) {
		mm.addCoins (facebookShareReward);
		facebookLinkShared = true;
		saveTime ("facebookShare", facebookShareHoursInterval);
		mm.refreshEarnOptions ();
	}

	public void shareLinkOnTwitter () {
		string TWITTER_ADDRESS = "http://twitter.com/intent/tweet";
		string TWEET_LANGUAGE = "en"; 

		string twitterPost = "Right now I'm playing an awesome beat'em'up game";

		string tags = "";
		for (int i = 0; i < hashtags.Length; i++) {
			tags += " #" + hashtags [i];
		}

		Application.OpenURL(TWITTER_ADDRESS +
			"?text=" + WWW.EscapeURL(twitterPost + " " + GooglePlayLink + " #android #mobile #zombie") +
			"&amp;lang=" + WWW.EscapeURL(TWEET_LANGUAGE));

		twitterLinkPosted = true;
		saveTime ("twitterShare", twitterShareHoursInterval);
		mm.addCoins (twitterShareReward);
		mm.refreshEarnOptions ();

	}

	public void followOnTwitter () {
		Application.OpenURL (twitterFollowURL);
		mm.addCoins (twitterFollowReward);
		PlayerPrefs.SetInt ("twitterFollowed", 1);
		mm.refreshEarnOptions ();
	}

	public void openTwitterLink () {
		if (twitterFollowURL == "") return;
		Application.OpenURL (twitterFollowURL);
	}

	public void openFacebookLink () {
		if (facebookURL == "") return;
		Application.OpenURL (facebookURL);
	}

	public void openTumblrLink () {
		if (tumblrURL == "") return;
		Application.OpenURL (tumblrURL);
	}

}
