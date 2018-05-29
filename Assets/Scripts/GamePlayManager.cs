using UnityEngine;
using System.Collections;
using UnityEngine.UI; // adding UI
using System.Collections.Generic;

public class GamePlayManager : MonoBehaviour {

	[Tooltip("playable characters")] public GameObject[] playableCharacters;
	[Tooltip("Score UI board")] public GameObject scoreTable;
	[Tooltip("Health bar")] public GameObject healthBar;
	[Tooltip("next level UI button")] public GameObject nextLevelButton;
	[Tooltip("game object array, which stores game buttons")] public GameObject[] gameButtons;
	[Tooltip("message UI board")] public GameObject messageBar;
	[Tooltip("into text messages array")] public string[] introText;
	[Tooltip("how long will intro messages would be displayed")] public float showTextTime;
	public string failText;
	public string winText;
	[Tooltip("audio clips")] public AudioClip[] audioClips;
	// 0 - fail
	// 1 - win

	[HideInInspector] public List<GameObject> pc; // list if playeable characters
	GameManager gm; // game manager
	GameObject[] inputManagers; // input managers
	float score;
	AudioSource aus;

	[HideInInspector] public int enemiesKilled;

	// Use this for initialization
	void Start () {
		prepareGame (); // define variables
		StartCoroutine ("displayIntroText");
	}
	
	// Update is called once per frame
	void FixedUpdate () {

	}

	public void prepareGame ()
	{
		gm = GameObject.FindGameObjectWithTag ("gameManager").GetComponent<GameManager> ();
		inputManagers = GameObject.FindGameObjectsWithTag ("inputManager");
		aus = GetComponent<AudioSource> ();
		prepareLayers ();
		pc = new List<GameObject> ();
		for (int i = 0; i < playableCharacters.Length; i++) {
			if (playableCharacters [i] != null) {
				pc.Add (playableCharacters [i]);
			}
		}
		if (Application.levelCount - 1 == Application.loadedLevel) {
			Destroy (nextLevelButton);
		}

		int characterNum = PlayerPrefs.GetInt ("characterNum", 0);
		for (int i = 0; i < inputManagers.Length; i++) {
			inputManagers [i].SendMessage ("selectPlayer", playableCharacters [characterNum]); // change fighter in input managers
		} 
		Camera.main.GetComponent<cameraBehaviour> ().changePlayer (playableCharacters [characterNum]); //
		playableCharacters[characterNum].GetComponent<fighterScript>().isMainCharacter = true;
		pc [characterNum].GetComponent<fighterScript> ().getUnderControl (healthBar);
		GameObject.Find ("CutsceneManager").GetComponent<CutsceneManager>().player = playableCharacters [characterNum];

		refreshScoreTable ();
	}

	IEnumerator displayIntroText () // displays intro text
	{
		for (int i = 0; i < introText.Length; i++) 
		{
			displayText(introText[i], showTextTime); //displays text
			yield return new WaitForSeconds(showTextTime + 0.1f); // holds text on a screen
		}
	}

	public void displayText (string message, float time) 
	{
		if (messageBar.GetComponent<Text> ().text == "") {
			messageBar.GetComponent<Text> ().text = message;
			Invoke ("clearText", time);
		}
	}

	public void clearText () 
	{
		messageBar.GetComponent<Text> ().text = "";
	}

	void prepareLayers ()
	{
		Physics2D.IgnoreLayerCollision (8, 12);
		Physics2D.IgnoreLayerCollision (9, 12);
		Physics2D.IgnoreLayerCollision (8, 13);
		Physics2D.IgnoreLayerCollision (9, 13);
		Physics2D.IgnoreLayerCollision (10, 13);
		Physics2D.IgnoreLayerCollision (11, 13);
		Physics2D.IgnoreLayerCollision (12, 13);
		Physics2D.IgnoreLayerCollision (15, 13);
		Physics2D.IgnoreLayerCollision (0, 13);
		Physics2D.IgnoreLayerCollision (10, 10);
	}

	public void applyScore(float plusScore) // plus score points
	{
		score += plusScore; // pluses score points
		scoreTable.GetComponent<Text>().text = "Score: " + score; // refreshes score board
		if (PlayerPrefs.GetFloat ("score") < score) // if new score is a record
		{
			PlayerPrefs.SetFloat ("score", score); // store score
		}
		refreshScoreTable ();
	}

	public void addCoins (float plusCoins) {
		PlayerPrefs.SetFloat ("coins", PlayerPrefs.GetFloat ("coins") + plusCoins);
	}

	void refreshScoreTable ()
	{
		scoreTable.GetComponent<Text> ().text = score + " / " + PlayerPrefs.GetFloat ("score");
	}

	public void win () 
	{
		displayText (winText, showTextTime);
		gm.win ();
		aus.PlayOneShot (audioClips[1]); // plays score audio
	}

	public void fail() // fail
	{
		displayText (failText, showTextTime);
		gm.fail ();
		aus.PlayOneShot (audioClips[0]); // plays score audio
	}

	public void switchGameButtons (bool on) 
	{
		for (int i = 0; i < gameButtons.Length; i++) 
		{
			gameButtons[i].SetActive (on);
		}
	}

	public void applyDeath (GameObject dead) {
		if (dead.GetComponent<fighterScript> ().isMainCharacter) {
			fail ();
		} else if (dead.layer == 9) {
			enemiesKilled++;
			applyScore (dead.GetComponent<fighterScript>().scoreForKilling);
		}
	}

	public void changePlayer () // change player
	{
		if (pc [pc.Count - 1].GetComponent<fighterScript> ().health > 0) { // if fighter is alice
			for (int i = 0; i < inputManagers.Length; i++) {
				inputManagers [i].SendMessage ("selectPlayer", pc [pc.Count - 1]); // change fighter in input managers
			} 
			Camera.main.GetComponent<cameraBehaviour> ().changePlayer (pc [pc.Count - 1]); //
			if (pc.Count > 1) // if more than one fighter is available
			{
				pc[pc.Count - 2].GetComponent<fighterScript>().underControl = false;
			}
			pc[pc.Count - 1].GetComponent<fighterScript>().getUnderControl(healthBar);
		}
		pc.Insert(0, pc[pc.Count - 1]);
		pc.RemoveAt(pc.Count - 1);
	}
	
}
