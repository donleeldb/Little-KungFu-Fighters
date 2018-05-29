using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LevelButtonController : MonoBehaviour {

	public string levelName;
	Button button;

	LevelManager lm;

	// Use this for initialization
	void Start () {
		lm = GameObject.Find ("LevelManager").GetComponent<LevelManager>();

		button = gameObject.GetComponent<Button> ();
		button.onClick.AddListener (
			() => { loadLevel ();}
		);
	}
	

	void loadLevel () {
		lm.loadLevel (levelName);
	}

}
