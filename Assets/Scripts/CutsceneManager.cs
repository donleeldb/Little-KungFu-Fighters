using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CutsceneManager : MonoBehaviour {

	[Tooltip("Put dialog window here")] public GameObject dialogWindow;

	[Tooltip ("How much coins would player achieve for facebook share")] public GameObject player;
	[Tooltip ("Create cutscenes here")] public Cutscene[] cutscenes;
	int cutscenesDone;
	GamePlayManager gpm;

	Cutscene actualCutscene;

	// Use this for initialization
	void Start () {
		gpm = GameObject.Find ("gamePlayManager").GetComponent<GamePlayManager>();
	}
	
	// Update is called once per frame
	void Update () {
		checkForReadiness (); // if some of cutscenes are ready to be called
	}

	void checkForReadiness ()
	{
		for (int i = 0; i < cutscenes.Length; i++) {
			Cutscene cutscene = cutscenes [i];
			if (cutscene.distanceTrigger.enabled) {
				if (player.transform.position.x > cutscene.distanceTrigger.x) {
				}
				else {
					return;
				}
			}
			if (cutscene.enemyKilledTrigger.enabled) {
				if (gpm.enemiesKilled >= cutscene.enemyKilledTrigger.enemyKilled) {
				}
				else {
					return;
				}
			}
			if (cutscene.called == false) {
				cutscene.called = true;
				StartCoroutine ("playCutscene", cutscene);
			}
		}
	}

	IEnumerator playCutscene (Cutscene cutscene) {

		dialogWindow.SetActive (true);

		for (int i = 0; i < cutscene.dialogs.speaches.Length; i++) {
			dialogWindow.transform.Find ("Portrait").gameObject.GetComponent<Image>().sprite = cutscene.dialogs.speaches[i].portrait;
			dialogWindow.transform.Find ("Speach").gameObject.GetComponent<Text>().text = cutscene.dialogs.speaches[i].text;
			yield return new WaitForSeconds (1.5f);
		}

		dialogWindow.SetActive (false);

		if (cutscene.win) {
			gpm.win ();
		} else if (cutscene.fail) {
			gpm.fail ();
		}

	}

}

[System.Serializable]
public class Cutscene { // cutscene class
	public string name; // cutscene name
	public DistanceTrigger distanceTrigger; // called in a proper place
	public enemyKilledTrigger enemyKilledTrigger; // called after killing number of enemies
	public Scene dialogs; // dialog speaches array
	[HideInInspector] public bool called;
	public bool win; // win after callscene
	public bool fail; //fail after callscene
}

[System.Serializable]
public class Scene {
	public string name;
	public Dialog[] speaches;
	[HideInInspector] public bool showed;
}

[System.Serializable]
public class Trigger {
	public string name;
	public bool enabled;
}

[System.Serializable]
public class DistanceTrigger : Trigger {
	public float x;
}

[System.Serializable]
public class enemyKilledTrigger : Trigger {
	public int enemyKilled;
}

[System.Serializable]
public class CharacterAct {
	public GameObject character;
	public Vector2 position;
	public bool enabled;
}

[System.Serializable]
public class Dialog {
	public Sprite portrait;
	public string text;
}