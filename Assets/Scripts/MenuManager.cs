using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {

	[Tooltip("put menu container here")] public GameObject menu;
	[Tooltip("put coins container here")] public GameObject Money;
	[Tooltip("put character selector container here")] public GameObject characterSelector;
	[Tooltip("put gameobject with ad manager script here")] public AdManager adManager;
	[Tooltip("put gameobject with social media manager script here")] public SocialMediaManager socialMediaManager;
	[Tooltip("put earn menu container here")] public GameObject EarnMenu;
	[Tooltip("put earn button here")] public GameObject[] EarnButtons;
	[Tooltip("put everyplay dialog container here")] public GameObject EveryplayDialog;
	[Tooltip("Enable everyplay option dialog?")] public bool enableEveryplay;
	[Tooltip("put share menu container here")] public GameObject shareMenu;
	[Tooltip("put settings menu container here")] public GameObject settingMenu;
	[Tooltip("put promocode intrance menu container here")] public GameObject PromocodeMenu;

	[Tooltip("put gift container here")] public GameObject GiftContainer;

	[Tooltip("put pop up gameboject here")] public GameObject popUp;

	[Tooltip("put characters here")] public character[] characters;

	[Tooltip("put coin sounds here")] public Audio coinSound;

	[Tooltip("put your rate url here")] public string rateUrl;

	[Tooltip("put background music tracks here")] public Audio[] bgm;

	AudioSource au; // stores audio soure
	AudioSource bgmau; // stores background music audiosource

	int characterNum; // number of character in array, which player plays
	Image avatar; // used character's avatar
	GameObject nextCharacterButton; // stores next character button
	GameObject prevCharacterButton;// stores previous character button
	GameObject nextMapButton; // stores next map button
	GameObject prevMapButton; // stores prev map button
	GameObject enableAudioButton; // stores enable audio button
	GameObject disableAudioButton; // stores next character button
	Text coinsBar; // stores coins bar text component


	[HideInInspector] public bool audioDisabled;


	GameObject actualMenu;
	GameObject prevMenu;

	// Use this for initialization
	void Start () {
		characterNum = PlayerPrefs.GetInt ("characterNum"); //gets selected character's number in array
		Time.timeScale = 1f;

		declareVariables (); 

		coinsBar.text = "" + getCoins ();

		prepareCharacters ();

		prepareAudio ();

		activateEarnButtons (socialMediaManager.earnOptions ());

		prevMenu = menu;
		actualMenu = menu;
	}

	void declareVariables ()
	{
		avatar = characterSelector.transform.Find ("avatar").GetComponent<Image> ();
		// avatar object
		nextCharacterButton = characterSelector.transform.Find ("right").gameObject;
		// determines variables
		prevCharacterButton = characterSelector.transform.Find ("left").gameObject;
		//		nextMapButton = mapSelector.transform.FindChild ("right").gameObject;
		//		prevMapButton = mapSelector.transform.FindChild ("left").gameObject;
		disableAudioButton = settingMenu.transform.Find ("DisableAudio").gameObject;
		enableAudioButton = settingMenu.transform.Find ("EnableAudio").gameObject;

		coinsBar = Money.transform.Find ("Number").gameObject.GetComponent<Text> ();
	}

	void prepareAudio ()
	{
		audioDisabled = PlayerPrefs.GetInt ("audioDisabled", 0) == 1 ? true : false;
		disableAudioButton.SetActive (!audioDisabled);
		enableAudioButton.SetActive (audioDisabled);
		au = gameObject.AddComponent<AudioSource> ();
		// creates audiosource for short sounds 
		au.playOnAwake = false;
		au.loop = false;
		bgmau = gameObject.AddComponent<AudioSource> ();
		// creates audiosource for background music
		bgmau.volume = audioDisabled ? 0 : 0.5f;
		bgmau.playOnAwake = false;
		bgmau.loop = true;
	}

	void prepareCharacters () { // determines which characters are brought and which are not
		for (int i = 0; i < characters.Length; i++) {
			if (characters [i].price == 0) {
				PlayerPrefs.SetInt ("character_" + i, 1);
			}
			bool isOpened = PlayerPrefs.GetInt ("character_" + i, 0) == 1 ? true : false;
			characters [i].opened = isOpened;
		}
	}
		
	public void openMenu () // open menu function
	{
		prevMenu = actualMenu;
		actualMenu = menu;

		Time.timeScale = 1f;

		Application.LoadLevel ("game");

	}

	public void goBackToMenu () {
		prevMenu = actualMenu;
		actualMenu = menu;

		prevMenu.SetActive (false);
		menu.SetActive (true);
	}

	public void openInGameMenu () // open in game menu
	{
		Time.timeScale = 0;
		menu.SetActive (false);
		characterSelector.SetActive (false);
		Money.SetActive (true);
		bgmau.Pause();
	}

	public void openCharacterSelection () // open character selection
	{
		prevMenu = actualMenu;
		actualMenu = characterSelector;

		menu.SetActive (false);
		characterSelector.SetActive (true);
		refreshCharacterSelection ();
	}

	public void nextCharacter () // turns to next character in a character selection
	{
		characterNum++;
		refreshCharacterSelection ();
	}

	public void prevCharacter () // turns to prev character in a character selection
	{
		characterNum--;
		refreshCharacterSelection ();
	}

	void refreshCharacterSelection ()
	{
		avatar.sprite = characters [characterNum].avatar;
		if (characterNum >= characters.Length - 1) 
		{
			nextCharacterButton.SetActive (false);
		} else 
		{
			nextCharacterButton.SetActive (true);
		}

		if (characterNum == 0) 
		{
			prevCharacterButton.SetActive (false);
		} else 
		{
			prevCharacterButton.SetActive (true);
		}

		GameObject buyButton = characterSelector.transform.Find ("buy").gameObject;
		GameObject selectButton = characterSelector.transform.Find ("select").gameObject;

		if (!characters [characterNum].opened) 
		{

			selectButton.SetActive (false);
			buyButton.SetActive (true);
			buyButton.transform.Find ("Text").gameObject.GetComponent<Text> ().text = "" + characters [characterNum].price;
		} else 
		{
			selectButton.SetActive (true);
			buyButton.SetActive (false);
		}

	}

	public void buyCharacter () {

		float price = characters [characterNum].price;
		if (getCoins () >= price) { // if player has enough money
			addCoins (-price);
			characters [characterNum].opened = true;
			PlayerPrefs.SetInt ("character_" + characterNum, 1);
			refreshCharacterSelection ();
		}

	}

	float getCoins () { // how much does player have coins
		return PlayerPrefs.GetFloat ("coins");
	}

	public void addCoins (float plusCoins) { // + coins 
		PlayerPrefs.SetFloat ("coins", getCoins () + plusCoins);
		coinsBar.text = "" + getCoins ();
		playOnce (coinSound);

		if (plusCoins > 0) {
			popUpText ("+ " + plusCoins);
		} else if (plusCoins < 0) {
			popUpText ("" + plusCoins);
		}

	}

	public void selectCharacter () // sets character as a player's character
	{
		PlayerPrefs.SetInt ("characterNum", characterNum);
		openMenu ();
	}

	void popUpText (string text) // pops up a bar 
	{
		popUp.SetActive (true); // enables text bar
		GameObject coin = popUp.transform.Find ("Coin").gameObject;
		if (menu.activeSelf || EarnMenu.activeSelf || characterSelector.activeSelf) {
			coin.SetActive (false);
		} else {
			coin.SetActive (true);
		}

		popUp.transform.Find ("Text").GetComponent<Text> ().text = text; // puts text in text bar
		Invoke ("disablePopUp", 1f); // disables pop up in 0.5 secs
	}

	public void backToMenu () // opens main menu
	{
		Application.LoadLevel (Application.loadedLevelName);
	}

	public void quit () // closes app
	{
		Application.Quit ();
	}

	public void disableAudio () // mutes all audio
	{

		audioDisabled = true;
		PlayerPrefs.SetInt ("audioDisabled", 1);

		disableAudioButton.SetActive (false);
		enableAudioButton.SetActive (true);

	}

	public void rate () // leads to your rate url
	{
		Application.OpenURL(rateUrl);
	}

	public void openShareMenu () {
		prevMenu = actualMenu;
		actualMenu = shareMenu;

		menu.SetActive (false);
		shareMenu.SetActive (true);
	}

	public void openHightScore ()
	{
		Application.OpenURL("http://unity3d.com/");
	}

	public void enableAudio () // unmutes all audio
	{
		AudioSource[] allAudioSources = FindObjectsOfType(typeof(AudioSource)) as AudioSource[];

		audioDisabled = false;
		PlayerPrefs.SetInt ("audioDisabled", 0);

		disableAudioButton.SetActive (true);
		enableAudioButton.SetActive (false);

	}

	void playOnce (Audio audio) // play sound, if sound exists
	{
		if (audio != null) {
			au.volume = !audioDisabled ? audio.volume: 0;
			au.PlayOneShot (audio.audio);
		}
	}

	public void openEarnMenu () {
		prevMenu = actualMenu;
		actualMenu = EarnMenu;

		characterSelector.SetActive (false);
		menu.SetActive (false);
		EarnMenu.SetActive (true);

		refreshEarnOptions ();
	}

	public void refreshEarnOptions () { // sets which earn options are available and which are not
		GameObject rewardedVideo = EarnMenu.transform.Find ("RewardedVideo").gameObject;
		GameObject facebookShare = EarnMenu.transform.Find ("facebookShare").gameObject;
		GameObject twitterShare = EarnMenu.transform.Find ("twitterShare").gameObject;
		GameObject twitterFollow = EarnMenu.transform.Find ("twitterFollow").gameObject;

		GameObject NoAds = EarnMenu.transform.Find ("NoAds").gameObject;
		GameObject NoFacebookShare = EarnMenu.transform.Find ("NoFacebookShare").gameObject;
		GameObject NoTwitterShare = EarnMenu.transform.Find ("NoTwitterShare").gameObject;

		rewardedVideo.SetActive (adManager.rewardedVideoIsReady ());
		NoAds.SetActive (!rewardedVideo.activeSelf);

		facebookShare.SetActive (!socialMediaManager.facebookLinkShared);
		NoFacebookShare.SetActive (!facebookShare.activeSelf);

		twitterShare.SetActive (!socialMediaManager.twitterLinkPosted);
		NoTwitterShare.SetActive (!twitterShare.activeSelf);

		bool twitterNotFollowed = PlayerPrefs.GetInt ("twitterFollowed", 0) == 1 ? false : true;
		twitterFollow.SetActive (twitterNotFollowed);

		if (!twitterNotFollowed) {
			float[] pos = new float[] {
				85,
				0,
				-90
			};

			rewardedVideo.GetComponent<RectTransform>().localPosition = new Vector2 (rewardedVideo.GetComponent<RectTransform>().localPosition.x, pos[0]);
			NoAds.GetComponent<RectTransform>().localPosition = new Vector2 (NoAds.GetComponent<RectTransform>().localPosition.x, pos[0]);

			facebookShare.GetComponent<RectTransform>().localPosition = new Vector2 (facebookShare.GetComponent<RectTransform>().localPosition.x, pos[1]);
			NoFacebookShare.GetComponent<RectTransform>().localPosition = new Vector2 (NoFacebookShare.GetComponent<RectTransform>().localPosition.x, pos[1]);

			twitterShare.GetComponent<RectTransform>().localPosition = new Vector2 (twitterShare.GetComponent<RectTransform>().localPosition.x, pos[2]);
			NoTwitterShare.GetComponent<RectTransform>().localPosition = new Vector2 (NoTwitterShare.GetComponent<RectTransform>().localPosition.x, pos[2]);
		}

	}

	public void activateEarnButtons (bool state) { // turns on all earn buttons
		foreach (GameObject button in EarnButtons) {
			button.SetActive (state);
		}
	}

	public void hideAdOption () {
		GameObject NoAds = EarnMenu.transform.Find ("NoAds").gameObject;
		GameObject rewardedVideo = EarnMenu.transform.Find ("RewardedVideo").gameObject;
		NoAds.SetActive (true);
		rewardedVideo.SetActive (false);
	}

	public void openSettingsMenu () {
		prevMenu = actualMenu;
		actualMenu = settingMenu;

		menu.SetActive (false);
		settingMenu.SetActive (true);
	}

	public void openPromocodeMenu () {
		prevMenu = actualMenu;
		actualMenu = PromocodeMenu;

		settingMenu.SetActive (false);
		PromocodeMenu.SetActive (true);
	}

	public void goBack() { // go to previous menu
		GameObject m = actualMenu;
		actualMenu = prevMenu;
		prevMenu = m;

		actualMenu.SetActive (true);
		prevMenu.SetActive (false);
	}

	public void loadLevel (string levelName) {
		Application.LoadLevel (levelName);
	}

}	

public static class MenuExtensionMethods // extension, which allows other scripts to find gameplay script
{
	public static MenuManager findMM(this GameObject go)
	{
		MenuManager gpm = GameObject.Find ("MenuManager").GetComponent<MenuManager>();
		return gpm;
	}
}

[System.Serializable]
public class character // character class
{
	[Tooltip("character's name")] public string name;
	[Tooltip("character's prefab")] public GameObject c;
	[Tooltip("if is available")] public bool opened;
	[Tooltip("character's image")] public Sprite avatar;
	[Tooltip("character's price")] public float price;
}