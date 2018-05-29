using UnityEngine;
using System.Collections;

public class KeyboardInputManager : MonoBehaviour {

	//[Tooltip("")] public CharacterBehaviour p;

	MenuManager gpm;

	float a;
	float b;
	// Use this for initialization
	void Start () 
	{
		gpm = gameObject.findMM ();
	}

	// Update is called once per frame
	void Update () {



	} 

	public void setPlayer (GameObject newPlayer)
	{
	//	p = newPlayer.GetComponent<CharacterBehaviour> ();	
	}

}
