using UnityEngine;
using System.Collections;

public class keyboardInputManager : MonoBehaviour {

	fighterScript fighter;
	MenuManager gpm;
	float speed;
	Rigidbody2D rb;
	// Use this for initialization
	void Start () {
		gpm = GameObject.FindGameObjectWithTag ("gamePlayManager").GetComponent<MenuManager> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey(KeyCode.A))
		{
			fighter.move(-1, rb.velocity.y / speed);
		}
		if (Input.GetKey(KeyCode.D))
		{
			fighter.move(1, rb.velocity.y / speed);
		}
		if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
		{
			fighter.move(0, rb.velocity.y / speed);
		}
		if (Input.GetKey (KeyCode.W))
		{
			fighter.move(rb.velocity.x / speed ,1);
		}
		if (Input.GetKey (KeyCode.S))
		{
			fighter.move(rb.velocity.x / speed ,-1);
		}
		if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S))
		{
			fighter.move(rb.velocity.x / speed , 0);
		}
		if (Input.GetKeyDown (KeyCode.Space))
		{
			fighter.jump ();
		}
		if (Input.GetKeyDown (KeyCode.J))
		{
			fighter.attack ();
		}
		if (Input.GetKeyDown (KeyCode.K))
		{
			fighter.checkForUsableObjects ();
		}
		if (Input.GetKeyDown (KeyCode.L))
		{
			fighter.StartCoroutine ("block");
		}
	}

	public void selectPlayer(GameObject newPlayer) 
	{
		fighter = newPlayer.GetComponent<fighterScript> ();
		speed = fighter.speed;
		rb = newPlayer.GetComponent<Rigidbody2D>();
	}

}
