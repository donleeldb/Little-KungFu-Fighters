using UnityEngine;
using System.Collections;

public class touchInputManager : MonoBehaviour {

	public float moveTrashhold;

	MenuManager gpm;
	fighterScript fighter;
	Vector3 direction;

	void Start () 
	{
		gpm = GameObject.FindGameObjectWithTag ("gamePlayManager").GetComponent<MenuManager>();
	}

	// Update is called once per frame
	void Update () {
		if (Input.touchCount > 0) { // checks if person has put finger on a screen
			if (Input.GetTouch (0).deltaPosition.magnitude > moveTrashhold) {
				direction = Input.GetTouch (0).deltaPosition.normalized;
			}
			fighter.move (direction.x, direction.y);
		} else {
			fighter.move (0, 0);
			direction = Vector3.zero;
		}
	}

	public void selectPlayer (GameObject newPlayer) 
	{
		fighter = newPlayer.GetComponent<fighterScript> ();
	}

	public void attack () 
	{
		fighter.attack ();
	}

	public void block () 
	{
		fighter.StartCoroutine ("block");
	}

	public void jump () 
	{
		fighter.jump ();
	}

	public void use () 
	{
		fighter.checkForUsableObjects ();
	}

}
