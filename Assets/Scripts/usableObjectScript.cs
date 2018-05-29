using UnityEngine;
using System.Collections;

public class usableObjectScript : MonoBehaviour {
	
	[Tooltip("sorting layer offset")] public float sortingLayerOffset;
	[Tooltip("stores arrow game object")] public GameObject arrow;
	[Tooltip("text to be displayed")] public string text;
	[Tooltip("time to display text")] public float textTime;
	[Tooltip("trigger to win")] public bool win;
	[Tooltip("adds score")] public float plusScore;
	[Tooltip("adds health points")] public float plusHealth;

	GamePlayManager gpm;
	SpriteRenderer sr;

	// Use this for initialization
	void Start () {
		prepareObject ();
		changeSortingLayer ();
	}

	public void disableArrow () 
	{
		if (arrow != null) {
			arrow.SetActive (false);
		}
	}

	public void use (GameObject user) 
	{
		if (win) 
		{
			gpm.win();
		}
		user.GetComponent<fighterScript> ().applyHealth (plusHealth);
		gpm.addCoins (plusScore);
	}

	public void changeSortingLayer() 
	{
		sr.sortingOrder = - Mathf.FloorToInt(transform.position.y + sortingLayerOffset);
	}

	public virtual void prepareObject () 
	{
		sr = GetComponent<SpriteRenderer> ();
		gpm = GameObject.FindGameObjectWithTag ("gamePlayManager").GetComponent<GamePlayManager>();
	}

	void OnTriggerEnter2D (Collider2D fighter)
	{
		checkFighter (fighter.gameObject);
	}

	public virtual void checkFighter (GameObject fighter) 
	{
		if (fighter.gameObject.layer == 8) 
		{
			use(fighter.gameObject); 
			gameObject.SetActive (false);
		}
	}

}
