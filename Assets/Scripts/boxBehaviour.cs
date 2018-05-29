using UnityEngine;
using System.Collections;

public class boxBehaviour : usableObjectScript {

	[Tooltip("how much health would be decreased")] public float damage;
	[Tooltip("direction, where box would fly when it is triggered (not normilized)")] public Vector2 throwPower;
	[Tooltip("prefab of a piece, which would appear after box breaks")] public GameObject piece;
	[Tooltip("number of pieces")] public int numberOfPieces;
	[Tooltip("power of throwing pieces after box breaks")] public float pieceThrowPower;
	[Tooltip("direction, where fighter would fly in case of collision with box (not normilized)")] public Vector2 hitDirection;

	GameObject[] pieces; // array of pieces

	bool throwed; // shows if box is triggered
	float yLevel; // default y coordinate of a box
	int s; // factors, which shows line of sight
	 
	Rigidbody2D rb; // box's rigidbody

	// Use this for initialization
	void Start () {
		prepareObject (); // determinies variables
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (throwed && transform.position.y < yLevel) // checks if box is landed
		{
			land(); //lands box
		}
	}

	void land () //lands box
	{
		rb.gravityScale = 0; // sets gravity back to 0
		rb.velocity = new Vector2 (rb.velocity.x, 0); // stops vertical movement
		yLevel = - 10000; 
		breakBox (); // breaks box
	}

	public void throwBox (int k) // trigger box
	{ 
		s = k; // determine line of sight
		yLevel = transform.position.y - 1; // determine yLevel
		rb.velocity = new Vector2 (throwPower.x * k, throwPower.y); // applies power to a box
		rb.gravityScale = 10; // adds gravity to a box to make it fall
		rb.isKinematic = false; // disables kinematic
		throwed = true; 
	}

	void breakBox () // breaks box
	{
		for (int i = 0; i < numberOfPieces - 1; i++) // throws pieces around 
		{
			pieces[i].transform.position = transform.position; // puts piece to a box position
			pieces[i].SetActive(true); // enables piece
			pieces[i].GetComponent<Rigidbody2D>().velocity = new Vector2 (Random.Range (-pieceThrowPower, pieceThrowPower), Random.Range(-pieceThrowPower, pieceThrowPower)); // applies random force to a piece
		}
		gameObject.SetActive (false); // disables piece
	}

	void preparePieces () // prepare pieces
	{
		pieces = new GameObject[numberOfPieces]; // creates pieces array
		for (int i = 0; i < numberOfPieces - 1; i++) //create pieces
		{
			GameObject newPiece = Instantiate(piece, Vector3.zero, Quaternion.identity) as GameObject; // creates new piece
			pieces[i] = newPiece; // puts new piece in array
			newPiece.SetActive(false); // disables piece
		}
	}

	public override void prepareObject () // determinies variables
	{
		base.prepareObject ();
		rb = GetComponent<Rigidbody2D> ();
		changeSortingLayer();
		preparePieces ();
		yLevel = -1000;
	}

	void OnCollisionEnter2D (Collision2D character) // checks for collision with fighters
	{
		if (character.transform.gameObject.layer == 8 && throwed || character.transform.gameObject.layer == 9 && throwed) // checks if it is a fighter
		{
			character.transform.gameObject.GetComponent<fighterScript>().getAttacked(true, damage, true, s, hitDirection); // calls getAttacked function
		}
	}

}
