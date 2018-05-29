using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraContainerScript : MonoBehaviour {

	[Tooltip("")] public Transform player;
	[Tooltip("")] public float speed;
	public float gap;

	void Start () {
		gap = Screen.width * 0.5f;
	}

	// Update is called once per frame
	void Update () {
		if (player) // if there are player to follow
		followPlayer (); // follows player
	}

	void followPlayer ()
	{
		transform.position = new Vector2 (player.position.x + gap, 0); // puts camera container to a player's position
	}

	public void setPlayer (GameObject newPlayer)
	{
		player = newPlayer.transform;
	}
		

}
