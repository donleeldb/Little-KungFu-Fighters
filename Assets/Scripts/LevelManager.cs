using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour {

	[Tooltip ("How much game levels does game contains of")] public int numberOfLevels;
	[HideInInspector] public Level[] levels;
	[Tooltip ("Put level button prefab here")] public GameObject buttonPrefab;

	[Tooltip ("Position, from which button grid starts")] public Vector2 startPos;
	public float step;
	public int rowSize;
	int stepDown;


	// Use this for initialization
	void Start () {

		Transform canvas = GameObject.Find ("Canvas").transform;

		levels = new Level [numberOfLevels]; // creates level buttons array
		for (int i = 0; i < numberOfLevels; i++) { // fills array with buttons
			levels [i] = new Level ();
			levels [i].number = i + 1;
			levels [i].name = "level_" + levels [i].number;
			levels [i].opened = PlayerPrefs.GetInt ("level_" + i, 0) == 1  || i == 0 ? true : false;

			if (i % rowSize == 0) {
				stepDown++;
			}

			Vector2 pos = startPos + (Vector2.right * step * i - Vector2.right * step * stepDown * rowSize) - Vector2.up * step * stepDown;

			GameObject newButton = Instantiate (buttonPrefab) as GameObject;
			newButton.name = "Level_" + i;
			newButton.transform.SetParent (canvas);
			int num = i + 1;
			newButton.GetComponent<LevelButtonController> ().levelName = "level_" + num;
			newButton.transform.Find ("Text").gameObject.GetComponent<Text> ().text = "" + num;
			newButton.GetComponent<RectTransform> ().localPosition = pos;

		}
	}

	public void loadLevel (string levelName) {
		for (int i = 0; i < numberOfLevels; i++) {
			if (levels [i].name == levelName) {
				if (levels [i].opened) {
					Application.LoadLevel (levelName);
				}
			}
		}
	}

	public void levelFinished (int i) {
		i++;
		PlayerPrefs.SetInt ("level_" + i, 1);
		levels [i].opened = true;
	}

}

[System.Serializable]
public class Level { // level class
	public string name; // its name
	public int number; // its number
	public bool opened; // is is available
}
