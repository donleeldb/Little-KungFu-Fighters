using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager: MonoBehaviour {
	 
	[Tooltip("Buttons, which opens in-game menu")] public GameObject inGameMenuButton;
	[Tooltip("GameObject, which stores in-game menu's buttons")] public GameObject inGameMenu;
	[Tooltip("Menu off/Menu on sprites")] public Sprite[] inGameMenuButtonSprites;

	GamePlayManager gpm; // stores game play manager script

	void Start () 
	{
		if (GameObject.FindGameObjectWithTag ("gamePlayManager")) {
			gpm = GameObject.FindGameObjectWithTag ("gamePlayManager").GetComponent<GamePlayManager> (); // determine gamePlayManager
		}
		Time.timeScale = 1;  
	}

    public void stop () // function stopping time by freezing physincs and courutine time counters
    {
        if (Time.timeScale == 1f)
        {
            Time.timeScale = 0;
        } else
        {
            Time.timeScale = 1f;
        }
    }

    public void closeApp () // function closes app
    {
        Application.Quit(); 
    }

	public void loadLevel (string scene) // function loads level
    {
        Application.LoadLevel(scene);
    }

	public void restartLevel () // function restarts level
	{
		Application.LoadLevel(Application.loadedLevel);
	}

	public void loadNextLevel() // function loads next level
	{
		if (Application.levelCount - 1 != Application.loadedLevel) {
			Application.LoadLevel (Application.loadedLevel + 1);
		}
	}

    public void rate () // functions opens url in default browser
    {
		Time.timeScale = 0;
		if (!inGameMenu.activeSelf) {
			inGameMenu.SetActive (true); // enables
			inGameMenuButton.GetComponent<Image> ().sprite = inGameMenuButtonSprites [1]; // sets sprite
			inGameMenuButton.GetComponent<RectTransform> ().sizeDelta = new Vector2 (inGameMenuButton.GetComponent<RectTransform> ().sizeDelta.x, inGameMenuButton.GetComponent<RectTransform> ().sizeDelta.y * 2); // fixes size
			gpm.switchGameButtons (false);
		}
		Application.OpenURL("https://www.assetstore.unity3d.com/#!/content/48923"); // change URL to yours
    }

	public void facebookShare () // facebook sharer
	{
		Application.OpenURL("https://www.facebook.com/sharer/sharer.php?u=https%3A//play.google.com/store/apps/details?id=com.Fomedevelopment.Sharksfood");
	}

	public void twitterShare () // twitter sharer
	{
		Application.OpenURL("https://twitter.com/home?status=https%3A//play.google.com/store/apps/details?id=com.Fomedevelopment.Sharksfood");
	}

	public void openInGameMenu () // functions enabling in-game menu if it is disabled and vice versa
    {
        if (!inGameMenu.activeSelf) // checks is in game menu is off
        {
            inGameMenu.SetActive(true); // enables
			inGameMenuButton.GetComponent<Image>().sprite = inGameMenuButtonSprites[1]; // sets sprite
			gpm.switchGameButtons(false);
        }
        else
        {
            inGameMenu.SetActive(false);
			inGameMenuButton.GetComponent<Image>().sprite = inGameMenuButtonSprites[0];
			gpm.switchGameButtons(true);
		}
    }

    public void fail () // fail
    {
        openInGameMenu(); // opens in game menu
		inGameMenuButton.SetActive(false); // we disable menu button to prevent the opportunity to continue the game with dead main character
    }

	public void win () // finish
	{
		stop ();
		openInGameMenu();
		inGameMenuButton.SetActive(false); // we disable menu button to prevent the opportunity to continue the game with dead main character
	}

	public void deselectButton() // Event system puts last pressed button to a special variable and calls it after any event. We set this variable to zero to prevent unwanted calls.
    {
        GameObject myEventSystem = GameObject.Find("EventSystem"); // Finding event system
		myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null); // setting variable to null
    }

}
