using UnityEngine;
using System.Collections;

public class cameraBehaviour : MonoBehaviour {

	Transform player; // stores player transform component
	[Tooltip("camera's movement speed")] public float cameraSpeed;

	Rigidbody2D rb;

	[Tooltip("The width of a camera")] public float width; 

	// Use this for initialization
	void Start () {
		if (width != 0) // This condition is delivered to prevent setting camera's size to zero when width is not set
		{
			// camera's orthographic size (height) * camera's aspect == camera's width  
			Camera.main.orthographicSize = width / Camera.main.aspect; 
			resizeCameraPhysics (); // sets box collider sizes to camera's sizes
		}
		rb = GetComponent<Rigidbody2D> (); // determinies rigidbody
	}

	void Update () 
	{
		if (player != null) {
			followPlayer (); // camera follows player
		}
	}

	public void changePlayer (GameObject newPlayer) // change a character to follow
	{
		player = newPlayer.transform;
	}

	void followPlayer () 
	{
		rb.MovePosition(Vector2.MoveTowards (transform.position, player.position, cameraSpeed)); // moves forward player's position
	}

	void resizeCameraPhysics () 
	{
		if (GetComponent<BoxCollider2D> ()) {
			GetComponent<BoxCollider2D> ().size = new Vector2 (Camera.main.orthographicSize * Camera.main.aspect, Camera.main.orthographicSize) * 2;
		}
	}

}
